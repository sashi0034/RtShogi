using System;
using System.Collections.Generic;
using System.Linq;
using RtShogi.Scripts.Battle.UI;
using RtShogi.Scripts.Lobby;
using RtShogi.Scripts.Param;
using UnityEngine;

namespace RtShogi.Scripts.Storage
{
    [Serializable]
    public class BattleLogElement
    { 
        [SerializeField] private int rating;
        public int Rating => rating;

        [SerializeField] private string playerName;
        public string PlayerName => playerName;

        [SerializeField] private string date;
        public string Date => date;

        [SerializeField] private EWinLoseDisconnected winLose;
        public EWinLoseDisconnected WinLose => winLose;

        public BattleLogElement(int rating, string playerName, string date, EWinLoseDisconnected winLose)
        {
            this.rating = rating;
            this.playerName = playerName;
            this.date = date;
            this.winLose = winLose;
        }
    }
    
    [Serializable]
    public class SaveData
    {
        [SerializeField] private int playerRating = 1000;
        public int PlayerRating => playerRating;

        [SerializeField] private MatchResultCount matchResultCount = new MatchResultCount();
        public MatchResultCount MatchResultCount => matchResultCount;

        [SerializeField] private string playerName = "";
        public string PlayerName => playerName;

        [SerializeField] private List<BattleLogElement> battleLogList = new List<BattleLogElement>();
        public List<BattleLogElement> BattleLogList => battleLogList;
        

        public void UpdateByCopyDataFromSomeObjects(GameRoot gameRoot)
        {
            playerName = gameRoot.LobbyCanvas.InputPlayerName.PlayerName;
        }

        public void SetPlayerRating(PlayerRating rating)
        {
            playerRating = rating.Value;
        }

        public void PushBattleLog(BattleLogElement newLog)
        {
            battleLogList.Insert(0, newLog);
            if (battleLogList.Count < ConstParameter.Instance.MaxSavableBattleLog) return;
            battleLogList.RemoveAt(battleLogList.Count - 1);
        }
    }
}