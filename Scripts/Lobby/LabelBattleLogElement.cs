using RtShogi.Scripts.Battle.UI;
using RtShogi.Scripts.Storage;
using TMPro;
using UnityEngine;

namespace RtShogi.Scripts.Lobby
{
    public class LabelBattleLogElement : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textRating;
        public TextMeshProUGUI TextRating => textRating;

        [SerializeField] private TextMeshProUGUI textPlayerName;
        public TextMeshProUGUI TextPlayerName => textPlayerName;

        [SerializeField] private TextMeshProUGUI textDate;
        public TextMeshProUGUI TextDate => textDate;

        [SerializeField] private RectTransform iconWin;
        public RectTransform IconWin => iconWin;

        [SerializeField] private RectTransform iconLose;
        public RectTransform IconLose => iconLose;

        [SerializeField] private RectTransform iconDisconnected;
        public RectTransform IconDisconnected => iconDisconnected;

        
        public void SetupView(BattleLogElement log)
        {
            textRating.text = log.Rating.ToString();
            textPlayerName.text = log.PlayerName;
            textDate.text = log.Date;
            iconWin.gameObject.SetActive(log.WinLose == EWinLoseDisconnected.Win);
            iconLose.gameObject.SetActive(log.WinLose == EWinLoseDisconnected.Lose);
            iconDisconnected.gameObject.SetActive(log.WinLose == EWinLoseDisconnected.Disconnected);
        }
    }
}