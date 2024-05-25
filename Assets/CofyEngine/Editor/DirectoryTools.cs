using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CofyEngine.Editor
{
    public class DirectoryTools
    {
        [MenuItem("Assets/Move To/UI", false, 0)]
        static void MoveToUI()
        {
            MoveAllSelectToDir("Assets/Prefab/UI");
        }

        [MenuItem("Assets/Open To/UI", false, 1)]
        static void OpenToUI()
        {
            FocusDirectory("Assets/Prefab/UI");
        }

        public static void FocusDirectory(string path)
        {
            Object folderMeta = AssetDatabase.LoadAssetAtPath<Object>(path);
            if (folderMeta != null)
            {
                Selection.activeObject = folderMeta;
            }
            else
            {
                FLog.Log($"folder ({path}) does not exist");
            }
        }

        private static void MoveAllSelectToDir(string targetDirPath)
        {
            for (var i = 0; i < Selection.objects.Length; i++)
            {
                MoveToDirectory(targetDirPath, Selection.objects[i]);
            }
        }

        private static void MoveToDirectory(string targetDirPath, Object obj)
        {
            string sourcePath = AssetDatabase.GetAssetPath(obj);
            string filename = Path.GetFileName(sourcePath);

            string targetPath = Path.Combine($"{targetDirPath}/{filename}");

            if (!Directory.Exists(targetDirPath))
            {
                FLog.Log(
                    $"Target directory ({targetDirPath}) does not exist. Please create skeleton using CofyTool first");
            }
            else
            {
                AssetDatabase.MoveAsset(sourcePath, targetPath);
            }
        }

        [MenuItem("Assets/Create/Scene To Addressable Folder")]
        public static void CreateSceneAddressable()
        {
            string path = "Assets/Prefab/Scene/New Scene.unity";

            Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            EditorSceneManager.SaveScene(newScene, path);
            
            FocusDirectory(path);
        }
    }
}