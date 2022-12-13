using Cysharp.Threading.Tasks;
using DG.Tweening;
using Michsky.MUIP;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace RtShogi.Scripts.Lobby
{
    public class ButtonShowTips : ButtonShowPopUpBase
    {
        [SerializeField] private ButtonManager buttonManager;
        [SerializeField] private PopUpShowTips popUpShowTips;
        [SerializeField] private Image popUpBackGround;

        [EventFunction]
        public void OnPushButton()
        {
            ButtonShowPopUpBase.PerformPopUp(
                buttonManager,
                popUpBackGround,
                popUpShowTips).Forget();
        }
    }
}