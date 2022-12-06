using DG.Tweening;
using JetBrains.Annotations;
using RtShogi.Scripts.Battle;
using UnityEngine;

namespace RtShogi.Scripts.Battle
{
    public class KomaUnit : MonoBehaviour
    {
        [SerializeField] private MeshFilter viewMeshFilter;
        [SerializeField] private MeshRenderer viewMeshRenderer;
        
        private EKomaKind _kind;
        
        public EKomaKind Kind => _kind;

        private EKomaKind _originalKind;
        public EKomaKind OriginalKind => _originalKind;
        
        private ETeam _team;
        public ETeam Team => _team;
        private BoardPiece _mountedPiece;
        public BoardPiece MountedPiece => _mountedPiece;

        private KomaId _id;
        public KomaId Id => _id;

        public void ResetMountedPiece(BoardPiece piece)
        {
            _mountedPiece = piece;
        }

        public void InitProps(KomaViewProps props, ETeam team, KomaId id)
        {
            viewMeshFilter.sharedMesh = props.Mesh;
            if (props.Materials is { Length: > 0 }) viewMeshRenderer.materials = props.Materials;
            _team = team;
            if (team==ETeam.Ally) transform.Rotate(new Vector3(0, 180, 0));
            _originalKind = props.Kind;
            _kind = props.Kind;
            _id = id;
        }

        public void FormSelf()
        {
            var formed = new KomaKind(_kind).ToFormed();
            Debug.Assert(formed!=null);
            _kind = formed.Value;
            transform.DORotate(new Vector3(0, 0, 180), 0.3f)
                .SetEase(Ease.OutQuad)
                .SetRelative(true);
        }
    }
}