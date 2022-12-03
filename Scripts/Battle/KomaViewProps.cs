﻿using System;
using JetBrains.Annotations;
using UnityEngine;

namespace RtShogi.Scripts.Battle
{
    
    [Serializable]
    public class KomaViewProps
    {
        [SerializeField] private EKomaKind kind;
        public EKomaKind Kind => kind; 
        
        [SerializeField] private Mesh mesh;
        public Mesh Mesh => mesh;
        
        [SerializeField] [CanBeNull] private Material[] materials;
        [CanBeNull] public Material[] Materials => materials;

        [CanBeNull] [SerializeField] private Sprite sprIcon;
        [CanBeNull] public Sprite SprIcon => sprIcon;
    }
}