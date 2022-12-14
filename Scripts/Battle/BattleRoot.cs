using System;
using System.Globalization;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using RtShogi.Scripts.Battle.UI;
using RtShogi.Scripts.Lobby;
using RtShogi.Scripts.Matching;
using RtShogi.Scripts.Param;
using RtShogi.Scripts.Storage;
using UniRx;
using UnityEngine;

namespace RtShogi.Scripts.Battle
{
    public class BattleRoot : MonoBehaviour
    {
        public static BattleRoot Instance;
        public BattleRoot()
        {
            Instance = this;
        }
        
        [SerializeField] private KomaManager komaManager;
        public KomaManager KomaManager => komaManager;

        [SerializeField] private PlayerCommander playerCommander;
        public PlayerCommander PlayerCommander => playerCommander;

        [SerializeField] private BoardManager boardManager;
        public BoardManager BoardManager => boardManager;

        [SerializeField] private BattleCanvas battleCanvasRef;
        public BattleCanvas BattleCanvasRef => battleCanvasRef;

        [SerializeField] private BattleRpcaller rpcaller;
        public BattleRpcaller Rpcaller => rpcaller;

        [SerializeField] private EffectManager effectManager;
        public EffectManager EffectManager => effectManager;

        [SerializeField] private SetupRpcaller setupRpcallerRef;
        public SetupRpcaller SetupRpcallerRef => setupRpcallerRef;


        public void ResetBeforeBattle()
        {
            this.gameObject.SetActive(true);
            battleCanvasRef.gameObject.SetActive(true);

            komaManager.ResetBeforeBattle();
            boardManager.ResetBeforeBattle();
            battleCanvasRef.ResetBeforeBattle();
        }

        public void SleepOutBattle()
        {
            this.gameObject.SetActive(false);
            battleCanvasRef.gameObject.SetActive(false);
        }

        public async UniTask<(BattleResultForRating, BattleLogElement)> ProcessBattle(GameRoot gameRoot)
        {
            var opponent = setupRpcallerRef.Opponent;

            SeManager.Instance.PlaySe(SeManager.Instance.SeStartBattle);
            
            // 敵の情報を表示
            await battleCanvasRef.PopUpEnemyInfo.PerformPopUpEnemyInfo(battleCanvasRef, setupRpcallerRef);

            setupRpcallerRef.RpcallNotifyHasSetupJustBeforeOnlineBattle();
            await UniTask.WaitUntil(() => setupRpcallerRef.Opponent.HasSetupJustBeforeOnlineBattle || 
                                          SetupRpcaller.IsInvalidOnlineRoomNow());

            // バトル開始
            InvokeStartBattle();

            // バトル終了
            var winLose = await battleCanvasRef.MessageWinLose.OnCompletedWinOrLose.Take(1);
            
            await UniTask.Delay(3f.ToIntMilli());

            // 終了処理
            if (PhotonNetwork.IsConnected) PhotonNetwork.Disconnect();

            // 対戦結果を返す
            gameRoot.SaveData.MatchResultCount.IncAfterBattle(winLose);
            return (
                new BattleResultForRating(
                    winLose,
                    new PlayerRating(gameRoot.SaveData.PlayerRating)
                        .CalcNext(winLose, new PlayerRating(opponent.PlayerRating))),
                new BattleLogElement(
                    opponent.PlayerRating,
                    opponent.PlayerName,
                    DateTime.Now.ToString(CultureInfo.InvariantCulture),
                    winLose));
        }

        public void InvokeStartBattle()
        {
            playerCommander.ProcessPlayer().Forget();
            komaManager.SetupAllAllyKomaOnBoard();
#if UNITY_EDITOR
            if (DebugParameter.Instance.IsDebugBattleOfflineMode == false)
                komaManager.CheckDisconnectionAsync().Forget();
#else
            komaManager.CheckDisconnectionAsync().Forget();
#endif
        }
    }
}