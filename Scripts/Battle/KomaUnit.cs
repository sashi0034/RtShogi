using JetBrains.Annotations;
using RtShogi.Scripts.Battle;
using UnityEngine;

namespace RtShogi.Scripts.Battle
{
    public class KomaUnit : MonoBehaviour
    {
        [SerializeField] private MeshFilter viewMeshFilter;
        [SerializeField] private MeshRenderer viewMeshRenderer;
        private EKomaTeam teamKind;
        private BoardPiece mountedPiece;

        public void ResetMountedPiece(BoardPiece piece)
        {
            mountedPiece = piece;
        }

        public void InitProps(KomaViewProps props, EKomaTeam team)
        {
            viewMeshFilter.sharedMesh = props.Mesh;
            if (props.Materials is { Length: > 0 }) viewMeshRenderer.materials = props.Materials;
            teamKind = team;
            if (team==EKomaTeam.Ally) transform.Rotate(new Vector3(0, 180, 0));
        }
    }
}