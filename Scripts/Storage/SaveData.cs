using System;
using RtShogi.Scripts.Lobby;
using UnityEngine;

namespace RtShogi.Scripts.Storage
{
    [Serializable]
    public class SaveData
    {
        [SerializeField] private int playerRating = 1000;
        public int PlayerRating => playerRating;

        [SerializeField] private MatchResultCount matchResultCount = new MatchResultCount();
        public MatchResultCount MatchResultCount => matchResultCount;

        [SerializeField] private string playerName = "";
        public string PlayerName => playerName;

        public void UpdateByCopyDataFromGame(GameRoot gameRoot)
        {
            playerName = gameRoot.LobbyCanvas.InputPlayerName.PlayerName;
        }

        public void SetPlayerRating(PlayerRating rating)
        {
            playerRating = rating.Value;
        }
    }
}