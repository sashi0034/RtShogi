using DG.Tweening;
using Michsky.MUIP;
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
            popUpBattleLog.gameObject.SetActive(true);
            popUpBattleLog.transform.localScale = Vector3.zero;
            popUpBattleLog.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack);

            buttonManager.enabled = false;
            // transform.DOScale(0.9f, 0.3f).SetEase(Ease.InOutBack);
            
            popUpBackGround.gameObject.SetActive(true);
        }
    }
}