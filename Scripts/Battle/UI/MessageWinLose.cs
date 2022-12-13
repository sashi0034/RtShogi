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
    // public enum EWinLose
    // {
    //     Win,
    //     Lose
    // }
    //
    // public static class WinLoseUtil
    // {
    //     public static EWinLoseDisconnected ToIncludeDisconnected(EWinLose winLose, bool isDisconnected)
    //     {
    //         return winLose == EWinLose.Win
    //             ? EWinLoseDisconnected.Win
    //             : isDisconnected
    //                 ? EWinLoseDisconnected.Disconnected
    //                 : EWinLoseDisconnected.Lose;
    //     }
    // }

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
        [SerializeField] private TextMeshProUGUI textWinByDisconnected;
        
        [SerializeField] private TextMeshProUGUI textLose;
        [SerializeField] private TextMeshProUGUI textLoseByDisconnected;

        private Subject<EWinLoseDisconnected> _onCompletedWinOrLose = new();
        public IObservable<EWinLoseDisconnected> OnCompletedWinOrLose => _onCompletedWinOrLose;

        public void ResetBeforeBattle()
        {
            gameObject.SetActive(false);
        }

        [Button]
        public void TestPerformWin()
        {
            PerformWin(false).Forget();
        }
        [Button]
        public void TestPerformLose()
        {
            PerformLose(false).Forget();
        }

        public async UniTask PerformWin(bool isDisconnected)
        {
            gameObject.SetActive(true);
            textLose.gameObject.SetActive(false);
            textLoseByDisconnected.gameObject.SetActive(isDisconnected);
            await performMessage(textWin);
            _onCompletedWinOrLose.OnNext(EWinLoseDisconnected.Win);
        }

        public async UniTask PerformLose(bool isDisconnected)
        {
            gameObject.SetActive(true);
            textWin.gameObject.SetActive(false);
            textLoseByDisconnected.gameObject.SetActive(isDisconnected);
            await performMessage(textLose);
            _onCompletedWinOrLose.OnNext(isDisconnected 
                ? EWinLoseDisconnected.Disconnected
                : EWinLoseDisconnected.Lose);
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