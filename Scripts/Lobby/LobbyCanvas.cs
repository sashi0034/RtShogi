using RtShogi.Scripts.Online;
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
        
    }
}