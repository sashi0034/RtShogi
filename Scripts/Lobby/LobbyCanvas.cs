using Cysharp.Threading.Tasks;
using RtShogi.Scripts.Online;
using UniRx;
using UnityEngine;

namespace RtShogi.Scripts.Lobby
{
    public class LobbyCanvas : MonoBehaviour
    {
        [SerializeField] private InputPlayerName inputPlayerName;
        public InputPlayerName InputPlayerName => inputPlayerName;

        [SerializeField] private ButtonStartMatching buttonStartMatching;
        public ButtonStartMatching ButtonStartMatching => buttonStartMatching;

        [SerializeField] private MatchMakingManager matchMakingManagerRef;
        public MatchMakingManager MatchMakingManagerRef => matchMakingManagerRef;

        public void ResetBeforeLobby()
        {
            this.gameObject.SetActive(true);
            buttonStartMatching.ResetBeforeLobby();
        }

        public void SleepOutLobby()
        {
            this.gameObject.SetActive(false);
        }
        
        public async UniTask ProcessLobby()
        {
            await buttonStartMatching.OnCompletedMatchMaking.Take(1);
        }
    }
}