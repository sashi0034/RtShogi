using System;
using Michsky.MUIP;
using RtShogi.Scripts.Param;
using Sirenix.Utilities;
using UnityEngine;

namespace RtShogi.Scripts.Lobby
{
    public class InputPlayerName : MonoBehaviour
    {
        [SerializeField] private CustomInputField customInputField;

        private string _oldPlayerName = "通りすがりの人";
        public string PlayerName => customInputField.inputText.text;

        public void OnChangedPlayerName()
        {
            checkNameLength();
        }

        private void checkNameLength()
        {
            if (customInputField.inputText.text.Length <= ConstParameter.Instance.MaxPlayerNameLength) return;

            customInputField.inputText.text = customInputField.inputText.text
                .Substring(0, ConstParameter.Instance.MaxPlayerNameLength);
        }

        public void OnSubmitPlayerName()
        {
            if (PlayerName.IsNullOrWhitespace())
            {
                customInputField.inputText.text = _oldPlayerName;
            }
            else
            {
                _oldPlayerName = customInputField.inputText.text;
            }
        }
    }
}