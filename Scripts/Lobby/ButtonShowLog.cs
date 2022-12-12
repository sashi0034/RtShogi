using Cysharp.Threading.Tasks;
using DG.Tweening;
using Michsky.MUIP;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace RtShogi.Scripts.Lobby
{
    public class ButtonShowLog : MonoBehaviour
    {
        [SerializeField] private ButtonManager buttonManager;
        [SerializeField] private PopUpBattleLog popUpBattleLog;
        [SerializeField] private Image popUpBackGround;

        [EventFunction]
        public void OnPushButton()
        {
            performPopUp().Forget();
        }

        private async UniTask performPopUp()
        {
            popUpBattleLog.gameObject.SetActive(true);
            popUpBattleLog.Setup();
            popUpBattleLog.transform.localScale = Vector3.zero;
            popUpBattleLog.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack);

            buttonManager.enabled = false;
            // transform.DOScale(0.9f, 0.3f).SetEase(Ease.InOutBack);
            
            popUpBackGround.gameObject.SetActive(true);

            await popUpBattleLog.OnExit.Take(1);
            
            popUpBackGround.gameObject.SetActive(false);
            
            buttonManager.enabled = true;
            
            await popUpBattleLog.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack);
            popUpBattleLog.gameObject.SetActive(false);
        }
    }
}