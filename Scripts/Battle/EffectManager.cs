#nullable enable

using RtShogi.Scripts.Battle.Effects;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RtShogi.Scripts.Battle
{
    public class EffectBase : MonoBehaviour { };

    public class EffectManager : MonoBehaviour
    {
        private const string effectTag = "Effects";
        
        [FoldoutGroup(effectTag)][SerializeField] private EffectBirth effectBirth;
        public EffectBirth EffectBirth => effectBirth;
        

        public T? Produce<T>(T effect) where T : EffectBase
        {
            var result = Instantiate(effect, transform) as T;
            return result;
        } 
    }
}