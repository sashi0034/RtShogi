#nullable enable

using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace RtShogi.Scripts.Battle
{
    public class KomaUnit : MonoBehaviour
    {
        // [SerializeField] private MeshFilter viewMeshFilter;
        // [SerializeField] private MeshRenderer viewMeshRenderer;
        
        [SerializeField] private Material matEnemyBody;

        [SerializeField] private MeshRenderer meshFront;
        public MeshRenderer MeshFront => meshFront;
        
        [SerializeField] private MeshRenderer? meshBack;
        public MeshRenderer? MeshBack => meshBack; 
        
        [SerializeField] private MeshRenderer meshBody;
        public MeshRenderer MeshBody => meshBody;

        public const string NameMatBody = "Body";
        
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

        private BattleRoot _battleRoot;

        public void ResetMountedPiece(BoardPiece piece)
        {
            _mountedPiece = piece;
        }

        public void InitProps(BattleRoot root, KomaViewProps props, ETeam team, KomaId id)
        {
            meshFront.material.mainTexture = props.TexFront;
            
            Debug.Assert(meshBack != null);
            if (props.TexBack != null)
                meshBack.material.mainTexture = props.TexBack; 
            else 
                Util.DestroyGameObject(meshBack.GameObject());
            
            _team = team;
            if (team==ETeam.Ally) transform.Rotate(new Vector3(0, 180, 0));
            if (team==ETeam.Enemy) changeToEnemyView();
            _originalKind = props.Kind;
            _kind = props.Kind;
            _id = id;
            _battleRoot = root;
        }

        private void changeToEnemyView()
        {
            meshBody.materials = meshBody.sharedMaterials
                .Select(material => (material.name == NameMatBody) ? matEnemyBody : material)
                .ToArray();
        }

        [FromBattleRpcaller]
        public void BecomeFormed()
        {
            var formed = new KomaKind(_kind).ToFormed();
            Debug.Assert(formed != null);
            _kind = formed.Value;
            animFormed().Forget();
        }

        private async UniTask animFormed()
        {
            var effect = _battleRoot.EffectManager.Produce(_battleRoot.EffectManager.EffectBecomeFormed);
            if (effect != null)
            {
                effect.transform.position = _mountedPiece.transform.position.FixY(0);
                await UniTask.Delay(0.3f.ToIntMilli());
            }
            
            transform.DORotate(new Vector3(0, 0, 180), 0.7f)
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