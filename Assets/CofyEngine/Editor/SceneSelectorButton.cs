using UnityEditor;
using UnityEngine;

namespace CofyEngine.Editor
{
    [InitializeOnLoad]
    public class SceneSelectorButton
    {
        private static GUIContent sceneSelectorContent;
        private static Rect buttonRect;

        static SceneSelectorButton()
        {
            sceneSelectorContent = EditorGUIUtility.IconContent("SceneAsset Icon");
            buttonRect = new Rect(0, 0, 30, 18);
            SceneView.duringSceneGui += DuringSceneGui;
        }

        private static void DuringSceneGui(SceneView sceneView)
        {
            buttonRect.x = sceneView.position.width - buttonRect.width - 20;
            buttonRect.y = 150;

            Handles.BeginGUI();
            if (GUI.Button(buttonRect, sceneSelectorContent, EditorStyles.toolbarButton))
            {
                SceneSelectorWindow.ShowWindow();
            }

            Handles.EndGUI();
        }
    }
}