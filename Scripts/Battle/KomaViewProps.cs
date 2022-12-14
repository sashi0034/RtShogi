#nullable enable

using System;
using JetBrains.Annotations;
using UnityEngine;

namespace RtShogi.Scripts.Battle
{
    
    [Serializable]
    public class KomaViewProps
    {
        [SerializeField] private EKomaKind kind;
        public EKomaKind Kind => kind;

        [SerializeField] private Texture texFront;
        public Texture TexFront => texFront;

        [SerializeField] private Texture? texBack;
        public Texture? TexBack => texBack;

        [SerializeField] private Sprite? sprIcon;
        public Sprite? SprIcon => sprIcon;
    }
}