using Sirenix.OdinInspector;
using UnityEngine;

namespace RtShogi.Scripts.Param
{
    public class ParamReference : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField] private DebugParameter debugParameter;
        public DebugParameter DebugParameter => debugParameter;

        [SerializeField] private ConstParameter constParameter;
        public ConstParameter ConstParameter => constParameter;

        [SerializeField] private ScriptableObject photonServerSettings;

        private const string urlPhotonConsole = "https://dashboard.photonengine.com/ja-JP/";

        [Button]
        public void OpenPhotonConsole()
        {
            Application.OpenURL(urlPhotonConsole);
        }
#endif
    }
}