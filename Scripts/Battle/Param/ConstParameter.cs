using UnityEngine;

namespace RtShogi.Scripts.Battle.Param
{
    [CreateAssetMenu(fileName = nameof(ConstParameter), menuName = "ScriptableObjects/Create" + nameof(ConstParameter))]
    public class ConstParameter : SingletonScriptableObject<ConstParameter>
    {
        [SerializeField] private float komaFormAbleEffectiveTime = 3f;
        public float KomaFormAbleEffectiveTime => komaFormAbleEffectiveTime;
        
        
    }
}