using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Michsky.MUIP;
using UniRx;
using UnityEngine;
using Image = UnityEngine.UI.Image;

namespace RtShogi.Scripts.Lobby
{
    public interface IPopUp
    {
        public void Setup();
        public GameObject gameObject { get; }
        public Transform transform { get; }
        public IObservable<Unit> OnExit { get; }
    }
    
    public class ButtonShowPopUpBase : MonoBehaviour
    {
        [SerializeField] private Image triangle;

        [EventFunction]
        private void Start()
        {
            DOTween.Sequence(triangle)
                .Append(triangle.transform.DOLocalMoveX(10f, 0.3f).SetEase(Ease.InOutSine).SetRelative(true))
                .Append(triangle.transform.DOLocalMoveX(-10f, 0.6f).SetEase(Ease.InOutSine).SetRelative(true))
                .SetLoops(-1);
        }

        public static async UniTask PerformPopUp(
            ButtonManager buttonManager,
            Image popUpBackGround,
            IPopUp popUp)
        {
            SeManager.Instance.PlaySe(SeManager.Instance.SePopUp);

            popUp.gameObject.SetActive(true);
            popUp.Setup();
            popUp.transform.localScale = Vector3.zero;
            popUp.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack);

            buttonManager.enabled = false;
            // transform.DOScale(0.9f, 0.3f).SetEase(Ease.InOutBack);
            
            popUpBackGround.gameObject.SetActive(true);

            var (hasExit, _) = await UniTask.WhenAny(
                popUp.OnExit.Take(1).ToUniTask(),
                // ポップアップ出ていてマッチングがしたとき抜けられるために
                UniTask.WaitWhile(() => buttonManager.gameObject.activeInHierarchy));
            
            popUpBackGround.gameObject.SetActive(false);
            buttonManager.enabled = true;

            if (hasExit) await performExit(popUp);
            
            popUp.gameObject.SetActive(false);
        }

        private static async UniTask performExit(IPopUp popUp)
        {
            SeManager.Instance.PlaySe(SeManager.Instance.SeOk);

            await popUp.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack);
        }
    }
}