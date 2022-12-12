using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using RtShogi.Scripts.Lobby;
using Sirenix.OdinInspector;
using TMPro;
using UniRx;
using UnityEngine;

namespace RtShogi.Scripts.Battle.UI
{
    public enum EWinLose
    {
        Win,
        Lose
    }

    public static class WinLoseUtil
    {
        public static EWinLoseDisconnected ToIncludeDisconnected(EWinLose winLose, bool isDisconnected)
        {
            return winLose == EWinLose.Win
                ? EWinLoseDisconnected.Win
                : isDisconnected
                    ? EWinLoseDisconnected.Disconnected
                    : EWinLoseDisconnected.Lose;
        }
    }

    public enum EWinLoseDisconnected
    {
        Win,
        Lose,
        Disconnected
    }

    public record BattleResultForRating(
        EWinLoseDisconnected WinLose,
        PlayerRating NewPlayerRating);
    
    
    public class MessageWinLose : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textWin;
        [SerializeField] private TextMeshProUGUI textLose;

        private Subject<EWinLose> _onCompletedWinOrLose = new();
        public IObservable<EWinLose> OnCompletedWinOrLose => _onCompletedWinOrLose;

        public void ResetBeforeBattle()
        {
            gameObject.SetActive(false);
        }

        [Button]
        public void TestPerformWin()
        {
            PerformWin().Forget();
        }
        [Button]
        public void TestPerformLose()
        {
            PerformLose().Forget();
        }

        public async UniTask PerformWin()
        {
            gameObject.SetActive(true);
            textLose.gameObject.SetActive(false);
            await performMessage(textWin);
            _onCompletedWinOrLose.OnNext(EWinLose.Win);
        }

        public async UniTask PerformLose()
        {
            gameObject.SetActive(true);
            textWin.gameObject.SetActive(false);
            await performMessage(textLose);
            _onCompletedWinOrLose.OnNext(EWinLose.Lose);
        }

        private static async UniTask performMessage(TextMeshProUGUI text)
        {
            text.gameObject.SetActive(true);
            text.transform.localScale = Vector3.zero;
            await text.transform.DOScale(1.3f, 0.5f).SetEase(Ease.OutBack);
            await text.transform.DOScale(1.0f, 1.0f).SetEase(Ease.InQuad);
            
            animScaleLoop(text).Forget();
        }

        private static async UniTask animScaleLoop(TextMeshProUGUI text)
        {
            while (text.gameObject.activeSelf)
            {
                await text.transform.DOScale(1.1f, 0.5f).SetEase(Ease.InOutSine);
                await text.transform.DOScale(1.0f, 1.0f).SetEase(Ease.InOutSine);
            }
        }
    }
}