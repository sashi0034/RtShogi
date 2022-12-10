using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace RtShogi.Scripts.Battle.Effects
{
    public class EffectBirth : EffectBase
    {
        public async UniTask AnimAppear(Vector3 pos)
        {
            transform.position = pos;
            transform.localScale = Vector3.zero;
            await transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
        }

        public async UniTask AnimDisappear()
        {
            await transform.DOScale(0, 0.3f).SetEase(Ease.InQuart);
            Util.DestroyGameObject(gameObject);
        }
    }
}