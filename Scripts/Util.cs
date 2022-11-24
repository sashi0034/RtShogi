
using UnityEngine;

namespace System.Runtime.CompilerServices
{
    internal sealed class IsExternalInit
    { }
}


namespace RtShogi.Scripts
{
    public static class Util
    {
        public static void DestroyGameObject(GameObject gameObject)
        {
            Object.Destroy(gameObject);
        }
        
        // https://nekojara.city/unity-enum-children
        public static Transform[] GetChildren(this Transform parent)
        {
            // 子オブジェクトを格納する配列作成
            var children = new Transform[parent.childCount];
            var childIndex = 0;

            // 子オブジェクトを順番に配列に格納
            foreach (Transform child in parent)
            {
                children[childIndex++] = child;
            }

            // 子オブジェクトが格納された配列
            return children;
        }
#if UNITY_EDITOR
        public static void DestroyGameObjectInEditor(GameObject gameObject)
        {
            Object.DestroyImmediate(gameObject);
        }
#endif
        public static void DestroyComponent(MonoBehaviour component)
        {
            Object.Destroy(component);
        }
    }
}