using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using RtShogi.Scripts.Storage;
using UniRx;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

namespace RtShogi.Scripts.Lobby
{
    public class PopUpBattleLog : MonoBehaviour
    {
        [SerializeField] private Button buttonExit;
        [SerializeField] private LabelBattleLogElement labelBattleLogElementPrefab;
        [SerializeField] private VerticalLayoutGroup viewportContent;
        
        private readonly Subject<Unit> _onExit = new();
        public IObservable<Unit> OnExit => _onExit;

        public void Setup()
        {
            buttonExit.transform.localScale = Vector3.one;
        }

        [EventFunction]
        public void OnPushedExit()
        {
            onPushedExitInternal().Forget();
        }

        private async UniTask onPushedExitInternal()
        {
            buttonExit.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack);
            await UniTask.Delay(0.1f.ToIntMilli());
            _onExit.OnNext(Unit.Default);
        }

        public void ResetBeforeBattle(SaveData saveData)
        {
            gameObject.SetActive(false);
            
            // いったん中身をクリアして
            foreach (var child in viewportContent.transform.GetChildren())
            {
                Util.DestroyGameObject(child.gameObject);
            }

            // 要素挿入
            foreach (var logElement in saveData.BattleLogList)
            {
                var label = Instantiate(labelBattleLogElementPrefab, viewportContent.transform);
                label.SetupView(logElement);
            }
        }
    }
}