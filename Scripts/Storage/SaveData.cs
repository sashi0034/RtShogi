using System;
using System.Collections.Generic;
using System.Globalization;
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
        // 敵のレート
        [SerializeField] private int rating;
        public int Rating => rating;

        // 敵の名前
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
    public class LastBattleOpponentCache
    {
        [SerializeField] private string opponentName = "";
        public string OpponentName => opponentName;

        [SerializeField] private int opponentRating = 0;
        public int OpponentRating => opponentRating;

        [SerializeField] private string dateTime = "";
        public string DateTime => dateTime;
        

        public LastBattleOpponentCache() {}
        public LastBattleOpponentCache(string opponentName, int opponentRating, DateTime date)
        {
            this.opponentName = opponentName;
            this.opponentRating = opponentRating;
            this.dateTime = date.ToString(CultureInfo.InvariantCulture);
        }
    }
    

    [Serializable]
    public class SaveData
    {
        [SerializeField] private int playerRating = ConstParameter.InitialPlayerRating.Value;
        public int PlayerRating => playerRating;

        [SerializeField] private MatchResultCount matchResultCount = new MatchResultCount();
        public MatchResultCount MatchResultCount => matchResultCount;

        [SerializeField] private string playerName = "";
        public string PlayerName => playerName;

        [SerializeField] private List<BattleLogElement> battleLogList = new List<BattleLogElement>();
        public List<BattleLogElement> BattleLogList => battleLogList;

        // 通信切断処理に実装
        [SerializeField] private bool isEnteredBattle;
        public bool IsEnteredBattle => isEnteredBattle;

        [SerializeField] private LastBattleOpponentCache lastOpponent = new LastBattleOpponentCache();
        public LastBattleOpponentCache LastOpponent => lastOpponent;
        
        
        

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

        public void EnterBeforeBattle(LastBattleOpponentCache opponent)
        {
            isEnteredBattle = true;
            this.lastOpponent = opponent;
        }

        public void LeaveAfterBattle()
        {
            isEnteredBattle = false;
        }
    }
}