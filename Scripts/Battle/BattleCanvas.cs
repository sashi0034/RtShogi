using System;
using RtShogi.Scripts.Battle.UI;
using UnityEngine;
using UnityEngine.Serialization;

namespace RtShogi.Scripts.Battle
{
    public class BattleCanvas : MonoBehaviour
    {
        [SerializeField] private CooldownBar cooldownBar;
        public CooldownBar CooldownBar => cooldownBar;

        [SerializeField] private ObtainedKomaGroup obtainedKomaAlly;
        public ObtainedKomaGroup ObtainedKomaAlly => obtainedKomaAlly;

        [SerializeField] private ObtainedKomaGroup obtainedKomaEnemy;
        public ObtainedKomaGroup ObtainedKomaEnemy => obtainedKomaEnemy;

        [SerializeField] private Canvas parentCanvas;
        public Canvas ParentCanvas => parentCanvas;

        private RectTransform _rectTransform;
        public RectTransform RectTransform => _rectTransform;

        [EventFunction]
        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        public ObtainedKomaGroup GetObtainedKomaGroup(ETeam team)
        {
            return team switch
            {
                ETeam.Ally => obtainedKomaAlly,
                ETeam.Enemy => obtainedKomaEnemy,
                _ => throw new ArgumentOutOfRangeException(nameof(team), team, null)
            };
        }
    }
}