using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RtShogi.Scripts.Battle.UI
{
    public class ButtonBecomeFormed : MonoBehaviour
    {
        [SerializeField] private Transform leftBottom;

        private Vector3 _startPos;
        
        [EventFunction]
        private void Awake()
        {
            gameObject.SetActive(false);
            _startPos = transform.position;
        }

        [Button]
        public void TestAppear()
        {
            StartAppear().Forget();
        }

        public async UniTask StartAppear()
        {
            transform.position = getScreenOutPos();
            
            gameObject.SetActive(true);
            await transform.DOMove(_startPos, 0.5f).SetEase(Ease.InOutBack);
        }

        [Button]
        public void TestDisappear()
        {
            StartDisappear().Forget();
        }

        public async UniTask StartDisappear()
        {
            await transform.DOMove(getScreenOutPos(), 0.5f).SetEase(Ease.InOutBack);
            
            gameObject.SetActive(false);
        }

        private Vector2 getScreenOutPos()
        {
            const float padY = -50f;
            return new Vector2(_startPos.x, leftBottom.position.y + padY);
        }

    }
}