using System;
using Michsky.MUIP;
using RtShogi.Scripts.Param;
using RtShogi.Scripts.Storage;
using Sirenix.Utilities;
using UnityEngine;

namespace RtShogi.Scripts.Lobby
{
    public class InputPlayerName : MonoBehaviour
    {
        [SerializeField] private CustomInputField customInputField;

        private string _oldPlayerName = ConstParameter.DefaultPlayerName;
        public string PlayerName => customInputField.inputText.text;

        public void ResetBeforeLobby(SaveData saveData)
        {
            customInputField.inputText.text = _oldPlayerName;
            
            var saveDataPlayerName = saveData.PlayerName;
            if (saveDataPlayerName.IsNullOrWhitespace()) return;
            _oldPlayerName = saveDataPlayerName;
            customInputField.inputText.text = saveDataPlayerName;
        }

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