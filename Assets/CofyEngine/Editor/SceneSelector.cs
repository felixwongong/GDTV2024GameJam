using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace CofyEngine.Editor
{
    public class SceneSelectorWindow : EditorWindow
    {
        private static readonly string[] searchFolder = new[] { "Assets/Scene_Local", "Assets/Prefab/Scene" };
        private static Dictionary<string, List<string>> scenePathToName = new();
        private string _activeTab;

        private Vector2 scrollPosition;
        private static SceneSelectorWindow _window;
        private const string EDITOR_SCENE_ACTIVE_TAB = "EDITOR_SCENE_ACTIVE_TAB";

        [MenuItem("Window/Scene Selector #s")]
        internal static void ShowWindow()
        {
            if (_window != null) _window.Close();
            else
            {
                _window = GetWindow<SceneSelectorWindow>(true, "Scene Selector", true);
                _window.minSize = new Vector2(600, 600);
                RefreshSceneList();
            }
        }

        private void OnGUI()
        {
            if (scenePathToName == null || scenePathToName.Count == 0)
            {
                EditorGUI.HelpBox(new Rect(0, 0, position.width, position.height), "No scenes found in the project",
                    MessageType.Warning);
                return;
            }

            _activeTab = EditorPrefs.GetString(EDITOR_SCENE_ACTIVE_TAB, string.Empty);
            if (_activeTab.isNullOrEmpty()) _activeTab = scenePathToName.Keys.ToArray()[0];

            EditorGUILayout.BeginHorizontal(GUILayout.ExpandHeight(true));

            EditorGUILayout.BeginVertical(GUILayout.Width(200), GUILayout.ExpandHeight(true));

            foreach (var path in scenePathToName.Keys)
            {
                
                if (GUILayout.Button(Path.GetFileName(path), EditorStyles.toolbarButton))
                {
                    _activeTab = path;
                    EditorPrefs.SetString(EDITOR_SCENE_ACTIVE_TAB, _activeTab);
                }
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            scenePathToName[_activeTab].ForEach(scenePath =>
            {
                if (GUILayout.Button(Path.GetFileNameWithoutExtension(scenePath), EditorStyles.miniButton))
                {
                    OnSceneButtonClick(scenePath);
                }
            });

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }

        private static void OnSceneButtonClick(string scenePath)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(scenePath);
                if (_window != null) _window.Close();
            }
        }

        private static void RefreshSceneList()
        {
            scenePathToName.Clear();

            foreach (var folder in searchFolder)
            {
                scenePathToName.Add(folder, new List<string>());
                var guids = AssetDatabase.FindAssets("t:Scene", new[] { folder })
                    .Concat(EditorBuildSettings.scenes.Select(scene => scene.path));
                
                foreach (var guid in guids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    if(path.notNullOrEmpty()) scenePathToName[folder].Add(path);
                }
            }
        }
    }
}