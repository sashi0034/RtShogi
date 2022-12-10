using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RtShogi.Scripts
{
    public class LogCanvas : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textLog;
        [SerializeField] private TextMeshProUGUI textWhenSleep;
        
        public static LogCanvas Instance;
        public LogCanvas()
        {
            Instance = this;
        }
        
        [SerializeField] private int maxLine = 16;
        private List<String> _currLog = new List<string>();

        [EventFunction]
        private void Awake()
        {
            enableSleep(true);
        }

        [EventFunction]
        private void Update()
        {
            if (isTriggerKeySleep())
                flipSleep();
        }

        private static bool isTriggerKeySleep()
        {
            return (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) &&
                Input.GetKeyDown(KeyCode.L);
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

            textLog.text = log;
        }

        private void flipSleep()
        {
            bool isSleep = textWhenSleep.gameObject.activeSelf;
            enableSleep(!isSleep);
        }
        private void enableSleep(bool isSleep)
        {
            textLog.gameObject.SetActive(!isSleep);
            textWhenSleep.gameObject.SetActive(isSleep);
        }
    }
}