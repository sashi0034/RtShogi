using System;
using Cysharp.Threading.Tasks;
using RtShogi.Scripts.Battle.UI;
using RtShogi.Scripts.Online;
using UniRx;
using UnityEngine;

namespace RtShogi.Scripts.Lobby
{
    public enum ELobbyResetOption
    {
        AfterInit,
        AfterBattle
    }
    
    public class LobbyCanvas : MonoBehaviour
    {
        [SerializeField] private InputPlayerName inputPlayerName;
        public InputPlayerName InputPlayerName => inputPlayerName;

        [SerializeField] private ButtonStartMatching buttonStartMatching;
        public ButtonStartMatching ButtonStartMatching => buttonStartMatching;

        [SerializeField] private MatchMakingManager matchMakingManagerRef;
        public MatchMakingManager MatchMakingManagerRef => matchMakingManagerRef;

        [SerializeField] private ChartWinLose chartWinLose;
        public ChartWinLose ChartWinLose => chartWinLose;

        [SerializeField] private LabelRating labelRating;
        public LabelRating LabelRating => labelRating;

        [SerializeField] private TextAfterBattle textAfterBattle;
        public TextAfterBattle TextAfterBattle => textAfterBattle;
        
        
        
        public void ResetBeforeLobby(GameRoot gameRoot, ELobbyResetOption resetOption)
        {
            this.gameObject.SetActive(true);
            buttonStartMatching.ResetBeforeLobby();
            chartWinLose.ResetBeforeLobby(gameRoot.SaveData);
            inputPlayerName.ResetBeforeLobby(gameRoot.SaveData);

            switch (resetOption)
            {
            case ELobbyResetOption.AfterInit:
                labelRating.ResetBeforeBattle(gameRoot.SaveData);
                break;
            case ELobbyResetOption.AfterBattle:
                buttonStartMatching.transform.localScale = Vector3.zero;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(resetOption), resetOption, null);
            }
        }

        public void SleepOutLobby()
        {
            this.gameObject.SetActive(false);
        }

        public async UniTask PerformAfterBattle(BattleResultForRating battleResult)
        {
            await labelRating.PerformAfterBattle(textAfterBattle, battleResult);
        }
        
        
        public async UniTask ProcessLobby()
        {
            await buttonStartMatching.OnCompletedMatchMaking.Take(1);
        }
    }
}