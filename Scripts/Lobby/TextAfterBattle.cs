using System;
using RtShogi.Scripts.Battle.UI;
using TMPro;
using UnityEngine;

namespace RtShogi.Scripts.Lobby
{
    public class TextAfterBattle : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textRatingDelta;
        public TextMeshProUGUI TextRatingDelta => textRatingDelta;

        [SerializeField] private TextMeshProUGUI textWinLose;
        public TextMeshProUGUI TextWinLose => textWinLose;

        [SerializeField] private Color colorWin;
        [SerializeField] private Color colorLose;
        [SerializeField] private Color colorDisconnected;

        
        public void ChangeStyle(BattleResultForRating battleResult)
        {
            var newColor = battleResult.WinLose switch
            {
                EWinLoseDisconnected.Win => colorWin,
                EWinLoseDisconnected.Disconnected => colorDisconnected,
                EWinLoseDisconnected.Lose => colorLose,
                _ => throw new ArgumentOutOfRangeException()
            };
            textWinLose.color = newColor;
            textRatingDelta.color = newColor;

            switch (battleResult.WinLose)
            {
            case EWinLoseDisconnected.Win:
                textWinLose.text = "Win";
                break;
            case EWinLoseDisconnected.Lose:
                textRatingDelta.text = "Lose";
                break;
            case EWinLoseDisconnected.Disconnected:
                textWinLose.text = "Disconnected";
                break;
            default:
                throw new ArgumentOutOfRangeException();
            }
        }
    }
}