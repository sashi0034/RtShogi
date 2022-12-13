using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using RtShogi.Scripts.Storage;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace RtShogi.Scripts.Lobby
{
    public class PopUpShowTips : MonoBehaviour, IPopUp
    {
        [SerializeField] private Button buttonExit;
        [SerializeField] private VerticalLayoutGroup viewportContent;

        private readonly Subject<Unit> _onExit = new();
        public IObservable<Unit> OnExit => _onExit;

        public void Setup()
        {
            buttonExit.transform.localScale = Vector3.one;
        }

        [EventFunction]
        public void OnPushedExit()
        {
            onPushedExitInternal().Forget();
        }

        private async UniTask onPushedExitInternal()
        {
            buttonExit.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack);
            await UniTask.Delay(0.1f.ToIntMilli());
            _onExit.OnNext(Unit.Default);
        }

        public void ResetBeforeBattle()
        {
            gameObject.SetActive(false);
        }
    }
}