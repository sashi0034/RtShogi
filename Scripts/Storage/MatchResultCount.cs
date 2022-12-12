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

        public void IncAfterBattle(EWinLose winLose)
        {
            if (winLose == EWinLose.Win) 
                IncWin();
            else
                IncLose();
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