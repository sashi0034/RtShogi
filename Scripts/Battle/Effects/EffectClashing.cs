using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace RtShogi.Scripts.Battle.Effects
{
    public class EffectClashing : EffectBase
    {
        private ParticleSystem _particleSystem;
        
        [EventFunction]
        private void Start()
        {
            _particleSystem = GetComponentInChildren<ParticleSystem>();
        }

        [EventFunction]
        private void Update()
        {
            if (_particleSystem.isStopped) Util.DestroyGameObject(gameObject);
        }
        
    }
}