using System;
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
        }

        [EventFunction]
        private void Update()
        {
            if (_particleSystem.isStopped) Util.DestroyGameObject(gameObject);
        }
    }
}