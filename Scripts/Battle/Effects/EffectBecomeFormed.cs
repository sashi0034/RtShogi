using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace RtShogi.Scripts.Battle.Effects
{
    public class EffectBecomeFormed : EffectBase
    {
        private ParticleSystem _particleSystem;
        
        [EventFunction]
        private void Start()
        {
            _particleSystem = GetComponentInChildren<ParticleSystem>();

            playSe().Forget();
        }

        private static async UniTask playSe()
        {
            await UniTask.Delay(0.2f.ToIntMilli());
            SeManager.Instance.PlaySe(SeManager.Instance.SeKomaForm);
        }

        [EventFunction]
        private void Update()
        {
            if (_particleSystem.isStopped) Util.DestroyGameObject(gameObject);
        }
    }
}