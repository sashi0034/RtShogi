using Cysharp.Threading.Tasks;
using DG.Tweening;
using RtShogi.Scripts.Battle.UI;
using UnityEngine;

namespace RtShogi.Scripts.Battle.Player
{
    public static class CooldownAnimation
    {
        public static async UniTask ShowCooldown(
            UI.CooldownBar cooldownBar,
            float maxSeconds)
        {
            cooldownBar.SetValues(maxSeconds, maxSeconds);
            await cooldownBar.StartAppear();
        }

        public static async UniTask GoDownToZeroCooldown(CooldownBar cooldownBar, float maxSeconds)
        {
            float currValue = maxSeconds;
            await DOTween.To(() => currValue, value =>
            {
                currValue = value;
                cooldownBar.SetValues(currValue, maxSeconds);
            }, 0, 0.3f).SetEase(Ease.OutQuad);
        }

        public static async UniTask ChargeThenHideCooldown(
            UI.CooldownBar cooldownBar,
            float maxSeconds)
        {
            float currentSec = maxSeconds;
            while (currentSec > 0)
            {
                await UniTask.DelayFrame(1);
                currentSec = Mathf.Max(currentSec - Time.deltaTime, 0);
                cooldownBar.SetValues(maxSeconds - currentSec, maxSeconds);
            }
            
            cooldownBar.StartDisappear().Forget();
        }

    }
}