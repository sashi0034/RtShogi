using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Photon.Pun;
using RtShogi.Scripts.Battle;
using RtShogi.Scripts.Lobby;
using RtShogi.Scripts.Online;
using RtShogi.Scripts.Param;
using Unity.VisualScripting;
using UnityEngine;

namespace RtShogi.Scripts
{
    public class GameRoot : MonoBehaviour
    {
        [SerializeField] private BattleCanvas battleCanvas;
        [SerializeField] private BattleRoot battleRoot;

        [SerializeField] private LobbyCanvas lobbyCanvas;

        [SerializeField] private MatchingDebugger matchingDebugger;
        
        public void Start()
        {
            lobbyCanvas.SleepOutLobby();
            battleRoot.SleepOutBattle();
            
#if UNITY_EDITOR
            if (DebugParameter.Instance.IsStartBattleImmediately)
            {
                debugBattleImmediately();
                return;
            }
#endif
            processGame().Forget();
        }

#if UNITY_EDITOR

        private void debugBattleImmediately()
        {
            battleRoot.ResetBeforeBattle();
            matchingDebugger.StartDebug();
        }
#endif

        private async UniTask processGame()
        {
            lobbyCanvas.ResetBeforeLobby();
            
            while (gameObject != null)
            {
                await processLobby();

                await processBattleBetweenLobby();
            }
        }
        
        private async UniTask processLobby()
        {
            Util.ResetScaleAndActivate(lobbyCanvas);
            await lobbyCanvas.ProcessLobby();
        }
        
        private async UniTask processBattleBetweenLobby()
        {
            battleRoot.ResetBeforeBattle();

            await lobbyCanvas.transform.DOScale(0f, 0.5f).SetEase(Ease.InBack);
            lobbyCanvas.SleepOutLobby();

            await battleRoot.ProcessBattle();
            
            lobbyCanvas.ResetBeforeLobby();
            
            await lobbyCanvas.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack);

            battleRoot.SleepOutBattle();
        }

    }
}