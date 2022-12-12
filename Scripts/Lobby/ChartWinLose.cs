using System;
using DG.Tweening;
using Michsky.MUIP;
using RtShogi.Scripts.Storage;
using UnityEngine;

namespace RtShogi.Scripts.Lobby
{
    public class ChartWinLose : MonoBehaviour
    {
        [SerializeField] private PieChart pieChart;
        [SerializeField] private int indexDisconnected = 0;
        [SerializeField] private int indexWin = 1;
        [SerializeField] private int indexLose = 2;

        [EventFunction]
        private void Start()
        {
            DOTween.Sequence(pieChart)
                .Append(pieChart.transform.DORotate(new Vector3(0, 0, 45), 4.5f)).SetEase(Ease.InOutQuint)
                .Append(pieChart.transform.DORotate(Vector3.zero, 1.5f)).SetEase(Ease.InOutQuint)
                .SetLoops(-1);
        }

        public void ResetBeforeLobby(SaveData saveData)
        {
            pieChart.chartData[indexWin].value = saveData.MatchResultCount.NumWin;
            pieChart.chartData[indexLose].value = saveData.MatchResultCount.NumLose;
            pieChart.chartData[indexDisconnected].value = saveData.MatchResultCount.NumDisconnected;
            pieChart.UpdateIndicators();
        }
    }
}