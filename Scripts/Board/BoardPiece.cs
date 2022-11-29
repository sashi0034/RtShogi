using JetBrains.Annotations;
using RtShogi.Scripts.Koma;
using UnityEngine;

namespace RtShogi.Scripts.Board
{
    public class BoardPiece : MonoBehaviour
    {
        [SerializeField] private GameObject highlightObject;
        
        [CanBeNull] private KomaUnit holdingKoma = null;
        [CanBeNull] public KomaUnit Holding => holdingKoma;
        
        public const float KomaPosY = 0.7f;
        
        public void PutKoma(KomaUnit koma)
        {
            var pos = transform.position;
            koma.transform.position = new Vector3(pos.x, KomaPosY, pos.z);
            koma.ResetMountedPiece(this);
            holdingKoma = koma;
        }

        public void EnableHighlight(bool isActive)
        {
            highlightObject.SetActive(isActive);
        }
    }
}