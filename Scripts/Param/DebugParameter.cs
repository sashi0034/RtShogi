using Sirenix.OdinInspector;
using UnityEngine;

namespace RtShogi.Scripts.Param
{
    [CreateAssetMenu(fileName = nameof(DebugParameter), menuName = "ScriptableObjects/Create" + nameof(DebugParameter))]
    public class DebugParameter : SingletonScriptableObject<DebugParameter>
    {
        private const string tagBuildIn = "BuildIn";
        
        [FoldoutGroup(tagBuildIn)][SerializeField] private bool isClearDebug = false;
        public bool IsClearDebug => isClearDebug;
        
        
#if UNITY_EDITOR 
        [SerializeField] private bool isDebugBattleOfflineMode = false;
        public bool IsDebugBattleOfflineMode => isDebugBattleOfflineMode;
        
        [SerializeField] private bool isNoCooldownTime = false;
        public bool IsNoCooldownTime => isNoCooldownTime;
        
        
#endif
    }
}