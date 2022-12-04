using UnityEngine;

namespace RtShogi.Scripts
{
    public static class Logger
    {
        public static void Print(string text)
        {
            UnityEngine.Debug.Log(fixText(text));
        }
        
        private static string fixText(string text)
        {
            return $"[{Time.frameCount}] {text}";
        }
    }
}