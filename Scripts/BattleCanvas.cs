using RtShogi.Scripts.Battle.UI;
using UnityEngine;

namespace RtShogi.Scripts
{
    public class BattleCanvas : MonoBehaviour
    {
        [SerializeField] private CooldownBar cooldownBar;
        public CooldownBar CooldownBar => cooldownBar;

        [SerializeField] private ObtainedKomaGroup obtainedKomaGroup;
        public ObtainedKomaGroup ObtainedKomaGroup => obtainedKomaGroup;
    }
}