using System;
using RtShogi.Scripts.Battle.UI;
using UnityEngine;

namespace RtShogi.Scripts.Battle
{
    public class BattleCanvas : MonoBehaviour
    {
        [SerializeField] private CooldownBar cooldownBar;
        public CooldownBar CooldownBar => cooldownBar;

        [SerializeField] private ObtainedKomaGroup obtainedKomaGroup;
        public ObtainedKomaGroup ObtainedKomaGroup => obtainedKomaGroup;

        [SerializeField] private Canvas _parentCanvas;
        public Canvas ParentCanvas => _parentCanvas;

        private RectTransform _rectTransform;
        public RectTransform RectTransform => _rectTransform;

        [EventFunction]
        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }
    }
}