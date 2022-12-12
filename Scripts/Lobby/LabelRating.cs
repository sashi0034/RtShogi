#nullable enable

using Cysharp.Threading.Tasks;
using RtShogi.Scripts.Battle.UI;
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
    }
    
    public class LabelRating : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textMesh;

        private PlayerRating _playerRating = new PlayerRating(0);
        public PlayerRating PlayerRating => _playerRating;

        public void ResetBeforeBattle(SaveData saveData)
        {
            _playerRating = new PlayerRating(saveData.PlayerRating).WithApplyText(textMesh);
        }

        public async UniTask PerformAfterBattle(TextAfterBattle textAfterBattle, BattleResultForRating battleResult)
        {
            textAfterBattle.ChangeStyle(battleResult);
            await UniTask.Delay(1f.ToIntMilli());
            
            // TODO: 数値が減っていく演出
        }

    }
}