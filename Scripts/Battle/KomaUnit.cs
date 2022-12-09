using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using JetBrains.Annotations;
using RtShogi.Scripts.Battle;
using RtShogi.Scripts.Battle.UI;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RtShogi.Scripts.Battle
{
    public class KomaUnit : MonoBehaviour
    {
        [SerializeField] private MeshFilter viewMeshFilter;
        [SerializeField] private MeshRenderer viewMeshRenderer;
        [SerializeField] private Material matEnemyBody;
        
        [SerializeField] private MeshRenderer meshRenderer;
        public MeshRenderer MeshRenderer => meshRenderer;

        public const string NameMatBody = "body";
        
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

        private KomaManager _komaManager;

        public void ResetMountedPiece(BoardPiece piece)
        {
            _mountedPiece = piece;
        }

        public void InitProps(KomaManager komaManager, KomaViewProps props, ETeam team, KomaId id)
        {
            viewMeshFilter.sharedMesh = props.Mesh;
            if (props.Materials is { Length: > 0 }) viewMeshRenderer.materials = props.Materials;
            _team = team;
            if (team==ETeam.Ally) transform.Rotate(new Vector3(0, 180, 0));
            if (team==ETeam.Enemy) changeToEnemyView();
            _originalKind = props.Kind;
            _kind = props.Kind;
            _id = id;
            _komaManager = komaManager;
        }

        private void changeToEnemyView()
        {
            meshRenderer.materials = meshRenderer.sharedMaterials
                .Select(material => (material.name == NameMatBody) ? matEnemyBody : material)
                .ToArray();
        }

        [FromBattleRpcaller]
        public void BecomeFormed()
        {
            var formed = new KomaKind(_kind).ToFormed();
            Debug.Assert(formed!=null);
            _kind = formed.Value;
            transform.DORotate(new Vector3(0, 0, 180), 0.3f)
                .SetEase(Ease.OutQuad)
                .SetRelative(true);
        }

        [Button]
        public void StartAnimKilled()
        {
            AnimKilled().Forget();
        }

        public async UniTask AnimKilled()
        {
            const float animDuration = 0.5f;
            const float moveY = 5.0f;
            // const float animScaleDuration = 0.2f;

            this.transform.DOMoveY(moveY, animDuration)
                .SetRelative(true).SetEase(Ease.OutCubic);
            
            const int numRotate = 5;
            this.transform.DORotate(new Vector3(360, 0, 0), animDuration / numRotate)
                .SetRelative(true).SetEase(Ease.Linear).SetLoops(numRotate);

            // await this.transform.DOScale(0, animScaleDuration).SetEase(Ease.InOutBack);
            // await UniTask.Delay((animDuration-animScaleDuration).ToIntMilli());
            
            await UniTask.Delay((animDuration).ToIntMilli());

            Util.DestroyGameObject(this.gameObject);
        }
    }
}