using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using RtShogi.Scripts.Battle.UI;
using RtShogi.Scripts.Matching;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

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
        
        [SerializeField] private PopUpBattleLog popUpBattleLog;
        public PopUpBattleLog PopUpBattleLog => popUpBattleLog;

        [SerializeField] private Image popUpBackground;
        public Image PopUpBackground => popUpBackground;
        


        public void ResetBeforeLobby(GameRoot gameRoot, ELobbyResetOption resetOption)
        {
            this.gameObject.SetActive(true);
            buttonStartMatching.ResetBeforeLobby();
            chartWinLose.ResetBeforeLobby(gameRoot.SaveData);
            inputPlayerName.ResetBeforeLobby(gameRoot.SaveData);
            popUpBattleLog.ResetBeforeBattle(gameRoot.SaveData);
            popUpBackground.gameObject.SetActive(false);

            switch (resetOption)
            {
            case ELobbyResetOption.AfterInit:
                labelRating.ResetBeforeLobby(gameRoot.SaveData);
                textAfterBattle.gameObject.SetActive(false);
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
            buttonStartMatching.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
        }
        
        
        public async UniTask ProcessLobby()
        {
            await buttonStartMatching.OnCompletedMatchMaking.Take(1);
        }
    }
}