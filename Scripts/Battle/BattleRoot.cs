using System;
using System.Globalization;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using RtShogi.Scripts.Battle.UI;
using RtShogi.Scripts.Lobby;
using RtShogi.Scripts.Matching;
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
            await UniTask.WaitUntil(() => setupRpcallerRef.Opponent.HasReceivedPlayerData);

            // 敵の情報を表示
            await battleCanvasRef.PopUpEnemyInfo.PerformPopUpEnemyInfo(battleCanvasRef, setupRpcallerRef);

            setupRpcallerRef.RpcallNotifyHasSetupJustBeforeOnlineBattle();
            await UniTask.WaitUntil(() => setupRpcallerRef.Opponent.HasSetupJustBeforeOnlineBattle);

            // バトル開始
            InvokeStartBattle();

            // バトル終了
            var winLose = await battleCanvasRef.MessageWinLose.OnCompletedWinOrLose.Take(1);
            
            await UniTask.Delay(3f.ToIntMilli());

            // 終了処理
            if (PhotonNetwork.IsConnected) PhotonNetwork.Disconnect();

            // 対戦結果を返す
            var winLoseResult = WinLoseUtil.ToIncludeDisconnected(winLose, false);
            gameRoot.SaveData.MatchResultCount.IncAfterBattle(winLoseResult);
            return (
                new BattleResultForRating(
                    winLoseResult,
                    new PlayerRating(gameRoot.SaveData.PlayerRating).CalcNext(winLoseResult)),
                new BattleLogElement(
                    setupRpcallerRef.Opponent.PlayerRating,
                    setupRpcallerRef.Opponent.PlayerName,
                    DateTime.Now.ToString(CultureInfo.InvariantCulture),
                    winLoseResult));
        }

        public void InvokeStartBattle()
        {
            playerCommander.ProcessPlayer().Forget();
            komaManager.SetupAllAllyKomaOnBoard();
        }
    }
}