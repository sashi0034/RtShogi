#nullable enable

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Michsky.MUIP;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RtShogi.Scripts.Battle.UI
{
    public class ButtonBecomeFormed : MonoBehaviour
    {
        [SerializeField] private BattleCanvas canvasRef;
        [SerializeField] private Transform leftBottom;
        [SerializeField] private ButtonManager buttonManager;
        
        private Transform buttonTransform => buttonManager.transform;

        private readonly Vector3 buttonStartLocalPos = Vector3.zero;
        private bool _isAppeared = false;
        private KomaUnit? _currentTarget = null;
        private CancellationTokenSource? _cancelDisappearByTimeOut = null;
        private Tween? _animAppeared;

        [EventFunction]
        private void Start()
        {
            // _buttonStartLocalPos = Vector3.zero;
        }

        public void ResetBeforeBattle()
        {
            setActive(false);
            buttonTransform.localPosition = buttonStartLocalPos;
        }

        private void setActive(bool isActive)
        {
            gameObject.SetActive(isActive);
            buttonTransform.gameObject.SetActive(isActive);
        }

        // 駒を成れるようにボタンを有効化
        public async UniTask EnableFormAbleKoma(KomaUnit koma, float enableTime)
        {
            _currentTarget = koma;
            _cancelDisappearByTimeOut?.Cancel();
            if (!_isAppeared) await startAppear();
            
            await buttonTransform.DOScale(1.3f, 0.3f).SetEase(Ease.OutBack);
            await buttonTransform.DOScale(1.0f, 0.3f).SetEase(Ease.OutBack);

            _cancelDisappearByTimeOut = new CancellationTokenSource();
            disappearByTimeOut(_cancelDisappearByTimeOut.Token, enableTime).Forget();
        }

        private async UniTask disappearByTimeOut(CancellationToken cancel, float enableTime)
        {
            await UniTask.Delay(enableTime.ToIntMilli(), cancellationToken: cancel);
            
            if (cancel.IsCancellationRequested) return;
            
            startDisappear().Forget();
        }

        [Button]
        public void TestAppear()
        {
            startAppear().Forget();
        }

        private async UniTask startAppear()
        {
            Logger.Print("ButtonBecomeFormed start appear");
            
            _isAppeared = true;
            setActive(true);

            if (buttonManager != null) buttonManager.Interactable(true);
            
            buttonTransform.localPosition = getScreenOutPos();
            buttonTransform.localScale = Vector3.one;

            if (_animAppeared is { active: true }) _animAppeared.Kill();
            await changeAnimAppeared(buttonTransform.DOLocalMove(buttonStartLocalPos, 0.5f).SetEase(Ease.InOutBack));
            
            Logger.Print("ButtonBecomeFormed end appear");
        }

        [Button]
        public void TestDisappear()
        {
            startDisappear().Forget();
        }

        private async UniTask startDisappear()
        {
            Logger.Print("ButtonBecomeFormed start disappear");
            
            _isAppeared = false;
            if (buttonManager != null) buttonManager.Interactable(false);

            await changeAnimAppeared(buttonTransform.DOLocalMove(getScreenOutPos(), 0.5f).SetEase(Ease.InOutBack));

            // 途中でstartAppearが呼ばれたときのために _isAppeared でActive判定
            setActive(_isAppeared);
            
            Logger.Print("ButtonBecomeFormed end disappear");
        }

        // TODO: これをクラスにしてアニメーションコントローラーを作れそう
        private async UniTask changeAnimAppeared(Tween newAnim)
        {
            if (_animAppeared is { active: true }) _animAppeared.Kill();
            await (_animAppeared = newAnim);
        }

        private Vector2 getScreenOutPos()
        {
            const float padY = -50f;
            return new Vector2(buttonStartLocalPos.x, padY);
        }

        [EventFunction]
        public void OnClicked()
        {
            if (!_isAppeared) return;
            if (_currentTarget == null) return;

            SeManager.Instance.PlaySe(SeManager.Instance.SePopUp);
            canvasRef.RootRef.Rpcaller.RpcallBecomeFormed(_currentTarget);
            disappearAfterClicked().Forget();
        }

        private async UniTask disappearAfterClicked()
        {
            _isAppeared = false;

            await changeAnimAppeared(buttonTransform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack));
        }

    }
}