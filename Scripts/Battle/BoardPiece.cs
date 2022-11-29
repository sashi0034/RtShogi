#nullable enable
using Unity.Collections;
using UnityEngine;

namespace RtShogi.Scripts.Battle
{
    public class BoardPiece : MonoBehaviour
    {
        [SerializeField] private GameObject highlightObject;
        
        [SerializeField, ReadOnly] private Vector2Int installedPoint;
        public BoardPoint Point => new BoardPoint(installedPoint.x, installedPoint.y);
        
        private KomaUnit? holdingKoma = null;
        public KomaUnit? Holding => holdingKoma;
        
        public const float KomaPosY = 0.7f;
        
        public void PutKoma(KomaUnit koma)
        {
            var pos = transform.position;
            koma.transform.position = new Vector3(pos.x, KomaPosY, pos.z);
            koma.ResetMountedPiece(this);
            holdingKoma = koma;
        }

        public void Initialize(Vector2Int point)
        {
            installedPoint = point;
        }

        public void EnableHighlight(bool isActive)
        {
            highlightObject.SetActive(isActive);
        }
    }
}