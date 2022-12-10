using UnityEngine;

namespace RtShogi.Scripts.Param
{
    public class ParamReference : MonoBehaviour
    {
        [SerializeField] private DebugParameter debugParameter;
        public DebugParameter DebugParameter => debugParameter;

        [SerializeField] private ConstParameter constParameter;
        public ConstParameter ConstParameter => constParameter;
        
    }
}