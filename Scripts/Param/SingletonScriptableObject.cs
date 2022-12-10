using UnityEngine;

namespace RtShogi.Scripts.Param
{
    public abstract class SingletonScriptableObject<T> : ScriptableObject where T : SingletonScriptableObject<T>
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load(typeof(T).Name) as T;
                }

                return _instance;
            }
        }
    }
}