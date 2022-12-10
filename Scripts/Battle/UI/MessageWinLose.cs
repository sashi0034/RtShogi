using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace RtShogi.Scripts.Battle.UI
{
    public class MessageWinLose : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textWin;
        [SerializeField] private TextMeshProUGUI textLose;

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
            textLose.gameObject.SetActive(false);
            await performMessage(textWin);
        }

        public async UniTask PerformLose()
        {
            textWin.gameObject.SetActive(false);
            await performMessage(textLose);
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