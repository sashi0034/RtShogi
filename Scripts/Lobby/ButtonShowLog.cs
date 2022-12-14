using Cysharp.Threading.Tasks;
using DG.Tweening;
using Michsky.MUIP;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace RtShogi.Scripts.Lobby
{
    public class ButtonShowLog : ButtonShowPopUpBase
    {
        [SerializeField] private ButtonManager buttonManager;
        [SerializeField] private PopUpBattleLog popUpBattleLog;
        [SerializeField] private Image popUpBackGround;

        [EventFunction]
        public void OnPushButton()
        {
            ButtonShowPopUpBase.PerformPopUp(
                buttonManager,
                popUpBackGround,
                popUpBattleLog).Forget();
        }
    }
}