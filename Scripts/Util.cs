
using System;
using DG.Tweening;
using UnityEngine;
using Object = UnityEngine.Object;

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
        public static void DestroyGameObjectPossibleInEditor(GameObject gameObject)
        {
#if UNITY_EDITOR
            Object.DestroyImmediate(gameObject);
#else
            Util.DestroyGameObject(gameObject);
#endif
        }
        public static void DestroyComponent(MonoBehaviour component)
        {
            Object.Destroy(component);
        }
        
        public static Tween GetCompletedTween()
        {
            return DOVirtual.DelayedCall(0, () => { }, false);
        }

        public static int ToIntMilli(this float second)
        {
            return (int)((1000) * second);
        }

        public static Vector3 FixX(this Vector3 before, float x)
        {
            return new Vector3(x, before.y, before.z);
        }
        public static Vector3 FixY(this Vector3 before, float y)
        {
            return new Vector3(before.x, y, before.z);
        }
        public static Vector3 FixZ(this Vector3 before, float z)
        {
            return new Vector3(before.x, before.y, z);
        }

    }
    
    [AttributeUsage(AttributeTargets.Method)]
    public class EventFunctionAttribute : System.Attribute { }
}