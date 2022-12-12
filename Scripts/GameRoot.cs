using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using RtShogi.Scripts.Battle;
using RtShogi.Scripts.Lobby;
using Unity.VisualScripting;
using UnityEngine;

namespace RtShogi.Scripts
{
    public class GameRoot : MonoBehaviour
    {
        [SerializeField] private BattleCanvas battleCanvas;
        [SerializeField] private BattleRoot battleRoot;

        [SerializeField] private LobbyCanvas lobbyCanvas;
        
        public void Start()
        {
            processGame().Forget();
        }

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
            battleCanvas.gameObject.SetActive(false);
            battleRoot.gameObject.SetActive(false);

            Util.ResetScaleAndActivate(lobbyCanvas);
            await lobbyCanvas.ProcessLobby();
        }
        
        private async UniTask processBattleBetweenLobby()
        {
            battleRoot.gameObject.SetActive(true);
            battleCanvas.gameObject.SetActive(true);
            battleRoot.ResetBeforeBattle();

            await lobbyCanvas.transform.DOScale(0f, 0.5f).SetEase(Ease.InBack);
            lobbyCanvas.gameObject.SetActive(false);

            await battleRoot.ProcessBattle();
            
            lobbyCanvas.ResetBeforeLobby();
            lobbyCanvas.gameObject.SetActive(true);
            await lobbyCanvas.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack);

            battleRoot.gameObject.SetActive(false);
            lobbyCanvas.gameObject.SetActive(false);
        }

    }
}