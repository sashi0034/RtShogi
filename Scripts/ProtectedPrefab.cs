using System;
using UnityEngine;

namespace RtShogi.Scripts
{
    [Serializable]
    public class ProtectedPrefab
    {
        [SerializeField] private GameObject prefab;
        public GameObject Birth(Transform parent)
        {
            return GameObject.Instantiate(prefab, parent);
        }
    }
}