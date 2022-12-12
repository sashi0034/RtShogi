#nullable enable
using RtShogi.Scripts.Param;
using Unity.Collections;
using UnityEngine;

namespace RtShogi.Scripts.Battle
{
    public class BoardPieceHighlightIntensity
    {
        public readonly int Value;
        
        public BoardPieceHighlightIntensity(int value)
        {
            Debug.Assert(
                new IntRange(0, ConstParameter.Instance.BoardPieceHighlightColors.Count - 1)
                .IsInRange(value));
            Value = value;
        }

        public Color GetHighlightColor()
        {
            return ConstParameter.Instance.BoardPieceHighlightColors[Value];
        }

        public PlayerCooldownTime GetCooldownTime()
        {
            var range = new IntRange(0, ConstParameter.Instance.CooldownTimeList.Length);
            Debug.Assert(range.IsInRange(Value));
            return new PlayerCooldownTime(ConstParameter.Instance.CooldownTimeList[range.RoundInRange(Value)]);
        }
    }
    
    public class BoardPiece : MonoBehaviour
    {
        [SerializeField] private MeshRenderer highlightObject;

        [SerializeField, ReadOnly] private Vector2Int installedPoint;
        public BoardPoint Point => new BoardPoint(installedPoint.x, installedPoint.y);
        
        private KomaUnit? holdingKoma = null;
        public KomaUnit? Holding => holdingKoma;

        private BoardPieceHighlightIntensity? _highlightIntensity = null;
        public BoardPieceHighlightIntensity? HighlightIntensity => _highlightIntensity;
        
        public const float KomaPosY = 0.55f;
        
        public void PutKoma(KomaUnit koma)
        {
            koma.ResetMountedPiece(this);
            holdingKoma = koma;
        }

        public Vector3 GetKomaPos()
        {
            var pos = transform.position;
            return new Vector3(pos.x, KomaPosY, pos.z);
        }

        public void RemoveKoma()
        {
            Debug.Assert((holdingKoma != null ? holdingKoma.MountedPiece : null)==this);
            holdingKoma = null;
        }

        public void ResetBeforeBattle()
        {
            holdingKoma = null;
            DisableHighlight();
        }

        public void Initialize(Vector2Int point)
        {
            installedPoint = point;
        }

        public void EnableHighlight(BoardPieceHighlightIntensity intensity)
        {
            highlightObject.gameObject.SetActive(true);
            highlightObject.material.color = intensity.GetHighlightColor();
            _highlightIntensity = intensity;
        }

        public void DisableHighlight()
        {
            highlightObject.gameObject.SetActive(false);
            _highlightIntensity = null;
        }

        public bool IsActiveHighlight()
        {
            return _highlightIntensity != null;
        }
    }
}