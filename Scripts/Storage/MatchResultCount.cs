using System;
using RtShogi.Scripts.Battle.UI;
using UnityEngine;

namespace RtShogi.Scripts.Storage
{
    [Serializable]
    public class MatchResultCount
    {
        [SerializeField] private int numWin = 0;
        public int NumWin => numWin;

        [SerializeField] private int numLose = 0;
        public int NumLose  => numLose;

        [SerializeField] private int numDisconnected = 0;
        public int NumDisconnected => numDisconnected;

        public void IncAfterBattle(EWinLoseDisconnected winLose)
        {
            switch (winLose)
            {
            case EWinLoseDisconnected.Win:
                IncWin();
                break;
            case EWinLoseDisconnected.Lose:
                IncLose();
                break;
            case EWinLoseDisconnected.Disconnected:
                IncDisconnected();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(winLose), winLose, null);
            }
        }

        public void IncWin()
        {
            numWin++;
        }
        public void IncLose()
        {
            numLose++;
        }

        public void IncDisconnected()
        {
            numDisconnected++;
        }
    }}