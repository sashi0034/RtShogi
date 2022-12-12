using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace RtShogi.Scripts.Battle.UI
{
    public class CooldownBar : MonoBehaviour
    {
        [SerializeField] private Michsky.MUIP.ProgressBar progressBar;
        [SerializeField] private TextMeshProUGUI textSecondsCurrent;
        [SerializeField] private TextMeshProUGUI textSecondsMax;
        [SerializeField] private Transform rightTop;
        [SerializeField] private Transform center;
        [SerializeField] private float animDuration = 0.1f;

        private Vector3 startPos => center.position;
        
        public void RestBeforeBattle()
        {
            // gameObject.SetActive(false);
            transform.position = getHidePos();
        }
        
        [Button]
        public async UniTask StartAppear()
        {
            // gameObject.SetActive(true);
            gameObject.transform.position = getHidePos();

            await transform.DOMove(startPos, animDuration).SetEase(Ease.OutSine);
        }
        
        [Button]
        public async UniTask StartDisappear()
        {
            await transform.DOMove(getHidePos(), animDuration).SetEase(Ease.OutSine);
            
            // gameObject.SetActive(false);
        }

        public void SetValues(float current, float max)
        {
            textSecondsCurrent.text = current.ToString("F1");
            textSecondsMax.text = "/ " + max.ToString("F1") + " sec";
            progressBar.loadingBar.fillAmount = current / max;
        }

        public Vector3 getHidePos()
        {
            float paddingY = 0;
            return new Vector3(startPos.x, rightTop.position.y, startPos.z) + Vector3.up * paddingY;
        }
        
        
        
    }
}