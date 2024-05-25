using System.IO;
using UnityEditor;
using UnityEngine;

namespace CofyEngine.Util
{
    [ExecuteInEditMode]
    public class IconMaker : MonoBehaviour
    {
        public Camera textureCam;
        public string path;
        public RenderTexture texture;
        public string spriteName;

        public void CreateIcon()
        {
            if (string.IsNullOrEmpty(spriteName))
            {
                spriteName = "icon";
            }

            string url = GetSavingDirPath(path);

            ReleaseRT();

            if (texture == null)
            {
                FLog.Log("Render texture could not be null for generating icon");
                return;
            }

            textureCam.targetTexture = texture;
            RenderTexture.active = texture;
            textureCam.Render();

            Texture2D png = new Texture2D(texture.width, texture.height, TextureFormat.ARGB32, false);
            png.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
            byte[] pngBytes = png.EncodeToPNG();
            File.WriteAllBytes($"{url}/{spriteName}.png", pngBytes);
            FLog.Log($"{spriteName}.png has created");

            textureCam.targetTexture = null;
            
            ReleaseRT();
            
            ProjectWindowRepaint();
        }

        private static void ProjectWindowRepaint()
        {
            #if UNITY_EDITOR
            var projectBrowserType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.ProjectBrowser");
            var projectBrowser = EditorWindow.GetWindow(projectBrowserType);
            projectBrowser.Repaint();
            #endif
        }

        private static void ReleaseRT()
        {
            if (RenderTexture.active)
            {
                var curRT = RenderTexture.active;
                RenderTexture.active = null;
                if (curRT != null) curRT.Release();
            }
        }

        private string GetSavingDirPath(string inPath)
        {
            string path = Application.dataPath.Replace("Assets", "") + this.path;

            if (!Directory.Exists(path))
            {
                FLog.Log($"Creating Directory {path}");
                Directory.CreateDirectory(path);
            }

            return path;
        }
    }

    #if UNITY_EDITOR
    [CanEditMultipleObjects]
    [CustomEditor(typeof(IconMaker))]
    public class IconMakerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Create png"))
            {
                FLog.Log("Creating Icon");
                var maker = target as IconMaker;
                if (maker != null) maker.CreateIcon();
            }
        }
    }
    #endif
}