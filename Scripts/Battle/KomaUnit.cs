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
        
        private ETeam _team;
        private BoardPiece _mountedPiece;

        public void ResetMountedPiece(BoardPiece piece)
        {
            _mountedPiece = piece;
        }

        public void InitProps(KomaViewProps props, ETeam team)
        {
            viewMeshFilter.sharedMesh = props.Mesh;
            if (props.Materials is { Length: > 0 }) viewMeshRenderer.materials = props.Materials;
            _team = team;
            if (team==ETeam.Ally) transform.Rotate(new Vector3(0, 180, 0));
            _kind = props.Kind;
        }
    }
}