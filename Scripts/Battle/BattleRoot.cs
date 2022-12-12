using System;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using RtShogi.Scripts.Online;
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
        
        public async UniTask ProcessBattle(GameRoot gameRoot)
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
        }

        public void InvokeStartBattle()
        {
            playerCommander.ProcessPlayer().Forget();
            komaManager.SetupAllAllyKomaOnBoard();
        }
    }
}