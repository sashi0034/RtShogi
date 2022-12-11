using System.Threading;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using UniRx;

namespace RtShogi.Scripts.Online
{
    public enum MatchMakingResult
    {
        Succeeded,
        Failed,
    }

    public class MatchMakingManager : MonoBehaviourPunCallbacks
    {
        private static readonly string[] keysForLobby = new[] { MatchPlayerRankUtil.KeyRank };

        private readonly Subject<Unit> _onFinishedConnectedToMaster = new Subject<Unit>();

        private readonly Subject<Unit> _onLeftRoom = new Subject<Unit>();
        private readonly Subject<MatchMakingResult> _onConnectedToRoom = new Subject<MatchMakingResult>();
        public const int MaxPlayerInRoom = 2;


        /// <summary>
        /// 対戦までの接続の流れ
        /// </summary>
        public async UniTask<MatchMakingResult> ProcessConnectAllFlows(
            MatchPlayerRank playerRank,
            string playerName,
            int sessionTimeSec)
        {
            await ProcessConnectToMaster(playerRank, playerName);
            return await ProcessConnectToJoinRoom(playerRank, sessionTimeSec);
        }

        public async UniTask ProcessConnectToMaster(MatchPlayerRank playerRank, string playerName)
        {
            Logger.Print("init connect settings");

            PhotonNetwork.OfflineMode = false;
            PhotonNetwork.NickName = playerName;

            // 入力値から自分のランクを設定
            PhotonNetwork.LocalPlayer.SetCustomProperties(MatchPlayerRankUtil.CreateRankProps(playerRank));

            // マスターへ接続
            Logger.Print("start connect...");
            PhotonNetwork.ConnectUsingSettings();
            await _onFinishedConnectedToMaster.Take(1).GetAwaiter();
        }
        
        public async UniTask<MatchMakingResult> ProcessConnectToJoinRoom(
            MatchPlayerRank playerRank,
            int sessionTimeSec)
        {
            // 最適なルームへランダムに参加する
            var joinedResult = await joinSuitableRoomAsync(playerRank, sessionTimeSec);
            if (joinedResult == MatchMakingResult.Succeeded) return MatchMakingResult.Succeeded;

            // 他のプレイヤーが見つからなかった
            Logger.Print("failed join suitable room");
            return MatchMakingResult.Failed;
        }


        private async UniTask<MatchMakingResult> joinSuitableRoomAsync(MatchPlayerRank playerRank, int sessionTimeSec)
        {
            // プレイヤーと同じランクの部屋があれば入る
            MatchMakingResult joinResult = await tryJoinRoomAsync(playerRank);
            if (joinResult == MatchMakingResult.Succeeded) return MatchMakingResult.Succeeded;

            // 近いランクの部屋がないなら、新規に部屋を作成
            createNewRoom(playerRank);

            // ほかのプレイヤーが来るまで待つ
            var taskWaitInRoomCancellation = new CancellationTokenSource();
            var taskWaitInRoom = waitUntilOtherPlayerJoinRoom(taskWaitInRoomCancellation.Token);
            int maxTimeWaitInRoom = sessionTimeSec * 1000;

            int taskWaitedIndex = await UniTask.WhenAny(taskWaitInRoom, UniTask.Delay(maxTimeWaitInRoom));
            if (taskWaitedIndex == 0) return MatchMakingResult.Succeeded;
            taskWaitInRoomCancellation.Cancel();

            // 待ち時間が過ぎても人が来ないから部屋をいったん抜ける
            PhotonNetwork.LeaveRoom();
            await _onLeftRoom.Take(1).GetAwaiter();

            // 部屋を抜けた後の再接続処理
            PhotonNetwork.ConnectUsingSettings();
            await _onFinishedConnectedToMaster.Take(1).GetAwaiter();

            // プレイヤーと同じランクの部屋が無い場合、近いランクの部屋があれば入る
            MatchMakingResult reJoinResult = await tryJoinRoomNearPlayerRank(playerRank);
            if (reJoinResult == MatchMakingResult.Succeeded) return MatchMakingResult.Succeeded;
            
            return MatchMakingResult.Failed;
        }

        private async UniTask<MatchMakingResult> tryJoinRoomNearPlayerRank(MatchPlayerRank rank)
        {
            Logger.Print("try join by near rank: " + rank);

            var nearRanks = rank.GetNearRanks();

            foreach (var near in nearRanks)
                if (await tryJoinRoomAsync(near) == MatchMakingResult.Succeeded)
                    return MatchMakingResult.Succeeded;

            return MatchMakingResult.Failed;
        }

        private async UniTask<MatchMakingResult> tryJoinRoomAsync(MatchPlayerRank roomRank)
        {
            Logger.Print("try joining room... (rank: " + roomRank + ")");
            var expectedProps = MatchPlayerRankUtil.CreateRankProps(roomRank);
            PhotonNetwork.JoinRandomRoom(expectedProps, MaxPlayerInRoom);
            var connectionResult = await _onConnectedToRoom.Take(1).GetAwaiter();
            return connectionResult;
        }

        public void ForceDisconnect()
        {
            PhotonNetwork.Disconnect();
        }


        public override void OnConnectedToMaster()
        {
            Logger.Print("connected to master");

            _onFinishedConnectedToMaster.OnNext(Unit.Default);
        }

        public override void OnLeftRoom()
        {
            Logger.Print("left room");

            _onLeftRoom.OnNext(Unit.Default);
        }

        // ランダムで参加できるルームが存在しないなら、新規でルームを作成する
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Logger.Print("failed to join the room");

            _onConnectedToRoom.OnNext(MatchMakingResult.Failed);
        }

        private void createNewRoom(MatchPlayerRank rank)
        {
            // ルームのカスタムプロパティの初期値に、自身と同じランクを設定する
            var initialProps = MatchPlayerRankUtil.CreateRankProps(rank);

            // ルーム設定を行う
            var roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = MaxPlayerInRoom;
            roomOptions.CustomRoomProperties = initialProps;
            roomOptions.CustomRoomPropertiesForLobby = keysForLobby;

            Logger.Print("created new room! (rank: " + rank + ")");
            PhotonNetwork.CreateRoom(null, roomOptions);
        }


        public override void OnJoinedRoom()
        {
            Logger.Print("joined the room successfully!");

            // ルームが満員になったら、以降そのルームへの参加を不許可にする
            if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
            }

            _onConnectedToRoom.OnNext(MatchMakingResult.Succeeded);
        }


        private async UniTask waitUntilOtherPlayerJoinRoom(CancellationToken cancellationToken)
        {
            await UniTask.WaitUntil(() =>
                PhotonNetwork.CurrentRoom is { PlayerCount: MaxPlayerInRoom }, cancellationToken: cancellationToken);
        }
    }
}