using System;
using System.Globalization;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using RtShogi.Scripts.Battle.UI;
using RtShogi.Scripts.Lobby;
using RtShogi.Scripts.Online;
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
            // TODO: ちゃんとしたバトル同期
            await UniTask.Delay(3000);

            // バトル開始
            InvokeStartBattle();

            var winLose = await battleCanvasRef.MessageWinLose.OnCompletedWinOrLose.Take(1);
            gameRoot.SaveData.MatchResultCount.IncAfterBattle(winLose);

            // バトル終了
            await UniTask.Delay(3f.ToIntMilli());

            if (PhotonNetwork.IsConnected) PhotonNetwork.Disconnect();

            // 対戦結果を返す
            var winLoseResult = WinLoseUtil.ToIncludeDisconnected(winLose, false);
            return (
                new BattleResultForRating(
                    winLoseResult,
                    new PlayerRating(gameRoot.SaveData.PlayerRating).CalcNext(winLoseResult)),
                new BattleLogElement(
                    1000,
                    "OPPOPNENT",
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