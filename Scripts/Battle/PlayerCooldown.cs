using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.ProBuilder;

namespace RtShogi.Scripts.Battle
{
    public static class PlayerCooldown
    {
        public static async UniTask StartCooldown(
            UI.CooldownBar cooldownBar,
            float maxSeconds)
        {
            cooldownBar.SetValues(0, maxSeconds);
            await cooldownBar.StartAppear();
            
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