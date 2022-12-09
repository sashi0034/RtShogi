using RtShogi.Scripts.Battle.Param;
using UnityEngine;

namespace RtShogi.Scripts
{
    public class ParamReference : MonoBehaviour
    {
        [SerializeField] private DebugParameter debugParameter;
        public DebugParameter DebugParameter => debugParameter;

        [SerializeField] private ConstParameter constParameter;
        public ConstParameter ConstParameter => constParameter;
        
    }
}