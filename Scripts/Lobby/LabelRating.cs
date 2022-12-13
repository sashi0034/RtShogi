#nullable enable

using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using RtShogi.Scripts.Battle.UI;
using RtShogi.Scripts.Param;
using RtShogi.Scripts.Storage;
using TMPro;
using UnityEngine;

namespace RtShogi.Scripts.Lobby
{
    public record PlayerRating(
        int Value)
    {
        public PlayerRating WithApplyText(TextMeshProUGUI text)
        {
            text.text = Value.ToString();
            return this;
        }

        public PlayerRating CalcNext(EWinLoseDisconnected winLose, PlayerRating enemyRating)
        {
            float rate = (float)enemyRating.Value / this.Value;

            var baseDelta = winLose switch
            {
                EWinLoseDisconnected.Win => 50,
                EWinLoseDisconnected.Lose => -50,
                EWinLoseDisconnected.Disconnected => -50,
                _ => throw new ArgumentOutOfRangeException(nameof(winLose), winLose, null)
            };

            int appliedDelta = winLose switch
            {
                EWinLoseDisconnected.Win => (int)(baseDelta * rate),
                EWinLoseDisconnected.Lose => -(int)(baseDelta / rate),
                EWinLoseDisconnected.Disconnected => baseDelta,
                _ => throw new ArgumentOutOfRangeException(nameof(winLose), winLose, null)
            };

            int fixedAppliedDelta = appliedDelta == 0
                ? Math.Sign(baseDelta)
                : Math.Abs(appliedDelta) > ConstParameter.Instance.MaxDeltaRating
                    ? ConstParameter.Instance.MaxDeltaRating * Math.Sign(baseDelta)
                    : appliedDelta;

            return new PlayerRating(ConstParameter.Instance.PlayerRatingRange.RoundInRange(Value + fixedAppliedDelta));
        }
    }
    
    public class LabelRating : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textMesh;

        private PlayerRating _playerRating = new PlayerRating(0);
        public PlayerRating PlayerRating => _playerRating;

        public void ResetBeforeLobby(SaveData saveData)
        {
            _playerRating = new PlayerRating(saveData.PlayerRating).WithApplyText(textMesh);
        }

        public async UniTask PerformAfterBattle(TextAfterBattle textAfterBattle, BattleResultForRating battleResult)
        {
            textAfterBattle.gameObject.SetActive(true);
            textAfterBattle.ChangeStyle(battleResult);
            textAfterBattle.transform.localScale = Vector3.zero;
            
            // 変化量を先に書き換えておく
            int deltaAbs = Math.Abs(battleResult.NewPlayerRating.Value - _playerRating.Value);
            string deltaPrefix = battleResult.WinLose == EWinLoseDisconnected.Win ? "+ " : "- ";
            textAfterBattle.TextRatingDelta.text = deltaPrefix + deltaAbs;

            await textAfterBattle.transform.DOScale(1.0f, 0.3f).SetEase(Ease.OutBack);
            
            await UniTask.Delay(1f.ToIntMilli());

            // カウントダウンの演出
            float animDuration = 1.0f;
            DOTween.To(
                () => _playerRating.Value,
                (value) => { _playerRating = new PlayerRating(value).WithApplyText(textMesh); },
                battleResult.NewPlayerRating.Value, 
                animDuration)
                .SetEase(Ease.OutSine);

            DOTween.To(
                    () => deltaAbs,
                    (value) => { textAfterBattle.TextRatingDelta.text = deltaPrefix + deltaAbs; deltaAbs = value; },
                    0, 
                    animDuration)
                .SetEase(Ease.OutSine);

            SeManager.Instance.PlaySe(SeManager.Instance.SeCountRating);
            
            await UniTask.Delay(animDuration.ToIntMilli());
            await UniTask.Delay(1.0f.ToIntMilli());

            await textAfterBattle.transform.DOScale(0f, 0.5f).SetEase(Ease.InBack);
            textAfterBattle.gameObject.SetActive(false);
        }

    }
}