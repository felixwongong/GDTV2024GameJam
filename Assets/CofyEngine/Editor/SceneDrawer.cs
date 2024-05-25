using System;
using System.Diagnostics;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CofyEngine.Editor
{
    [CustomPropertyDrawer(typeof(CofySceneAttribute))]
    [Obsolete]
    public class SceneDrawer : PropertyDrawer
    {
        private SceneAsset sceneAsset;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.String)
            {
                if(!string.IsNullOrEmpty(property.stringValue))
                {
                    var path = AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets(property.stringValue).First());
                    sceneAsset = (SceneAsset)AssetDatabase.LoadAssetAtPath(path, typeof(SceneAsset));
                }
                
                sceneAsset = (SceneAsset)EditorGUI.ObjectField(position, label, sceneAsset, typeof(SceneAsset), true);
                property.stringValue = sceneAsset == null ? string.Empty : sceneAsset.name;
            }
            else
                EditorGUI.LabelField(position, label.text, "Use [Scene] with strings.");
        }
    }
    
    [Obsolete("Use CofyAssetObject instead for more generic usage")]
    [Conditional("UNITY_EDITOR"), AttributeUsage(AttributeTargets.Field)]
    public class CofySceneAttribute : PropertyAttribute
    {
    }
}
