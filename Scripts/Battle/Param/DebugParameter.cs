using UnityEngine;
using UnityEngine.Serialization;

namespace RtShogi.Scripts.Battle.Param
{
    [CreateAssetMenu(fileName = nameof(DebugParameter), menuName = "ScriptableObjects/Create" + nameof(DebugParameter))]
    public class DebugParameter : SingletonScriptableObject<DebugParameter>
    {
#if UNITY_EDITOR 
        [SerializeField] private bool isDebugBattleOfflineMode = false;
        public bool IsDebugBattleOfflineMode => isDebugBattleOfflineMode;
        
        [SerializeField] private bool isNoCooldownTime = false;
        public bool IsNoCooldownTime => isNoCooldownTime;
        
        
        
        
        

#endif
    }
}