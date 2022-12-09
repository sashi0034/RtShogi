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
        [SerializeField] private Transform leftBottom;

        private Vector3 _startPos;
        private bool _isAppeared = false;
        private KomaUnit? _currentTarget = null;
        private ButtonManager? _buttonManager;
        private CancellationTokenSource? _cancelDisappearByTimeOut = null;
        private Tween? _tweenMoveInAppear;

        [EventFunction]
        private void Awake()
        {
            gameObject.SetActive(false);
            _startPos = transform.position;
            _buttonManager = GetComponent<ButtonManager>();
        }
        

        // 駒を成れるようにボタンを有効化
        public async UniTask EnableFormAbleKoma(KomaUnit koma, float enableTime)
        {
            _currentTarget = koma;
            _cancelDisappearByTimeOut?.Cancel();
            if (!_isAppeared) await startAppear();
            
            await transform.DOScale(1.3f, 0.3f).SetEase(Ease.OutBack);
            await transform.DOScale(1.0f, 0.3f).SetEase(Ease.OutBack);

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
            gameObject.SetActive(true);

            if (_buttonManager != null) _buttonManager.Interactable(true);
            
            transform.position = getScreenOutPos();
            transform.localScale = Vector3.one;

            if (_tweenMoveInAppear is { active: true }) _tweenMoveInAppear.Kill();
            await (_tweenMoveInAppear = 
                transform.DOMove(_startPos, 0.5f).SetEase(Ease.InOutBack));
            
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
            if (_buttonManager != null) _buttonManager.Interactable(false);

            if (_tweenMoveInAppear is { active: true }) _tweenMoveInAppear.Kill();
            await (_tweenMoveInAppear = transform.DOMove(getScreenOutPos(), 0.5f).SetEase(Ease.InOutBack));

            // 途中でstartAppearが呼ばれたときのために _isAppeared でActive判定
            gameObject.SetActive(_isAppeared);
            
            Logger.Print("ButtonBecomeFormed end disappear");
        }

        private Vector2 getScreenOutPos()
        {
            const float padY = -50f;
            return new Vector2(_startPos.x, leftBottom.position.y + padY);
        }

        [EventFunction]
        public void OnClicked()
        {
            if (!_isAppeared) return;
            if (_currentTarget == null) return;

            _currentTarget.FormSelf();
            disappearAfterClicked().Forget();
        }

        private async UniTask disappearAfterClicked()
        {
            _isAppeared = false;

            await transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack);
        }

    }
}