using Cysharp.Threading.Tasks;
using DG.Tweening;
using RtShogi.Scripts.Matching;
using TMPro;
using UnityEngine;

namespace RtShogi.Scripts.Battle.UI
{
    public class PopUpEnemyInfo : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textName;
        public TextMeshProUGUI TextName => textName;

        [SerializeField] private TextMeshProUGUI textRating;
        public TextMeshProUGUI TextRating => textRating;

        public void ResetBeforeBattle()
        {
            gameObject.SetActive(false);
        }

        public async UniTask PerformPopUpEnemyInfo(BattleCanvas canvas, SetupRpcaller rpcaller)
        {
            string name = rpcaller.Opponent.PlayerName;
            int rating = rpcaller.Opponent.PlayerRating;
            
            canvas.LabelEnemyInfo.TextEnemy.text = name;
            textName.text = name;
            textRating.text = rating.ToString();
            
            gameObject.SetActive(true);
            transform.localScale = transform.localScale.FixX(0);
            
            await transform.DOScaleX(1f, 0.5f).SetEase(Ease.OutBounce);
            await UniTask.Delay(1.5f.ToIntMilli());
            
            fade().Forget();
        }

        private async UniTask fade()
        {
            await transform.DOScaleX(0f, 0.5f).SetEase(Ease.InBack);
            gameObject.SetActive(false);
        }
    }
}