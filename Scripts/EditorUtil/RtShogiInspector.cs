#if UNITY_EDITOR

using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace RtShogi.Scripts.EditorUtil
{
    public class RtShogiInspector : EditorWindow
    {
        private readonly Vector2 buttonsize = new Vector2(200.0f, 20.0f);
        
        [MenuItem("Window/" + nameof(RtShogiInspector))]
        private static void ShowWindow()
        {
            // ウィンドウを表示
            EditorWindow.GetWindow<RtShogiInspector>();
        }

        [EventFunction]
        private void Update()
        {
            Repaint();
        }

        [EventFunction]
        private void OnGUI()
        {
            if (GUILayout.Button("Open Main Scene", GUILayout.Width(buttonsize.x), GUILayout.Height(buttonsize.y)))
            {
                EditorSceneManager.OpenScene("Assets/RtShogi/Scenes/MainScene.unity", OpenSceneMode.Single);
            }
        }
    }
}

#endif