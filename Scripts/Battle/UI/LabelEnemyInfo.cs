using TMPro;
using UnityEngine;

namespace RtShogi.Scripts.Battle.UI
{
    public class LabelEnemyInfo : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textEnemy;
        public TextMeshProUGUI TextEnemy => textEnemy;

        public void ResetBeforeBattle()
        {
            textEnemy.text = "???";
        }
    }
}