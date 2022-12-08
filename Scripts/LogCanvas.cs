using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RtShogi.Scripts
{
    public class LogCanvas : MonoBehaviour
    {
        public static LogCanvas Instance;
        public LogCanvas()
        {
            Instance = this;
        }
        
        [SerializeField] private int maxLine = 16;
        private TextMeshProUGUI _text;
        private List<String> _currLog = new List<string>();

        private void Awake()
        {
            _text = GetComponent<TextMeshProUGUI>();
            gameObject.SetActive(false);
        }

        public void Print(String logText)
        {
            // Debug.Log(logText);
            
            _currLog.Add(logText);
            
            while (_currLog.Count>maxLine) _currLog.RemoveAt(0);
            
            var log = "";
            foreach (var line in _currLog)
            {
                log += line + "\n";
            }

            _text.text = log;
        }
    }
}