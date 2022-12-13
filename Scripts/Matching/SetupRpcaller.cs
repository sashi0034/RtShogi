#nullable enable

using System;
using Photon.Pun;
using Photon.Realtime;
using RtShogi.Scripts.Battle;

namespace RtShogi.Scripts.Matching
{
    public interface ISetupRpcallerState
    {
        public int PlayerRating { get; }
        public string PlayerName { get; }
        public bool HasReceivedPlayerData { get; }
        public bool HasSetupJustBeforeOnlineBattle { get; }
    }

    public class SetupRpcallerState : ISetupRpcallerState
    {
        public int PlayerRating { get; set; } = 0;
        public string PlayerName { get; set; } = "";
        public bool HasReceivedPlayerData { get; set; } = false;
        public bool HasSetupJustBeforeOnlineBattle { get; set; } = false;
    }

    /// <summary>
    /// オンライン対戦で相手とのバトル前同期などを取る
    /// </summary>
    public class SetupRpcaller : MonoBehaviourPunCallbacks
    {
        private SetupRpcallerState _opponent = new SetupRpcallerState();
        public ISetupRpcallerState Opponent => _opponent;

        public void ResetParam()
        {
            _opponent = new SetupRpcallerState();
        }

        /// <summary>
        /// プレイヤー情報を登録
        /// </summary>
        public void RpcallSetupPlayerData(int playerRating, string playerName)
        {
            photonView.RPC(nameof(setupLeaderData), RpcTarget.All,
                PhotonNetwork.LocalPlayer.ActorNumber, // int photonActorNumber,
                playerRating, // int playerRating,
                playerName // string playerName
            );
        }

        [PunRPC]
        private void setupLeaderData(
            int photonActorNumber,
            int playerRating,
            string playerName)
        {
            modifyState(photonActorNumber, state =>
            {
                state.PlayerRating = playerRating;
                state.PlayerName = playerName;
                state.HasReceivedPlayerData = true;
            });
        }

        /// <summary>
        /// バトルがすぐに始めらることを通知
        /// </summary>
        public void RpcallNotifyHasSetupJustBeforeOnlineBattle()
        {
            photonView.RPC(nameof(notifyHasSetupJustBeforeOnlineBattle), RpcTarget.All,
                PhotonNetwork.LocalPlayer.ActorNumber // int photonActorNumber,
            );
        }

        [PunRPC]
        private void notifyHasSetupJustBeforeOnlineBattle(int photonActorNumber)
        {
            modifyState(photonActorNumber,
                state => state.HasSetupJustBeforeOnlineBattle = true);
        }

        /// <summary>
        /// null安全にstateを変更
        /// </summary>
        private void modifyState(int photonActorNumber, Action<SetupRpcallerState> action)
        {
            var state = getState(photonActorNumber);
            if (state == null) return;
            action(state);
        }

        private SetupRpcallerState? getState(int photonActorNumber)
        {
            return IsLocalPhotonActor(photonActorNumber) ? null : _opponent;
        }

        public static bool IsLocalPhotonActor(int photonActorNumber)
        {
            return BattleRpcaller.IsLocalPhotonActor(photonActorNumber);
        }
        
        /// <summary>
        /// 自分や対戦相手が抜けたりしていたらtrue
        /// </summary>
        /// <returns></returns>
        public static bool IsInvalidOnlineRoomNow()
        {
            if (PhotonNetwork.CurrentRoom == null) return true;
            if (PhotonNetwork.CurrentRoom.PlayerCount != MatchMakingManager.MaxPlayerInRoom) return true;

            // valid room
            return false;
        }


    }
}
