using System;
using System.Diagnostics;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CofyEngine.Editor
{
    [CustomPropertyDrawer(typeof(CofyAssetObjectAttribute))]
    public class CofyAssetObject : PropertyDrawer
    {
        private UnityEngine.Object asset;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.String)
            {
                var objAttr = (CofyAssetObjectAttribute)attribute;
                
                if(!string.IsNullOrEmpty(property.stringValue))
                {
                    //TODO: improve this to remove file extension string manipulation
                    var path = AssetDatabase.GUIDToAssetPath(
                        AssetDatabase.FindAssets(property.stringValue.Split('.').First())
                        .First());
                    asset = AssetDatabase.LoadAssetAtPath(path, objAttr.type);
                }
                
                asset = EditorGUI.ObjectField(position, label, asset, objAttr.type, false);
                property.stringValue = asset == null ? string.Empty : asset.name;
            }
            else
                EditorGUI.LabelField(position, label.text, "Use [Scene] with strings.");
        }
    }
    
    [Conditional("UNITY_EDITOR"), AttributeUsage(AttributeTargets.Field)]
    public class CofyAssetObjectAttribute: PropertyAttribute
    {
        internal Type type;
        public CofyAssetObjectAttribute(Type type)
        {
            this.type = type;
        }
    }
}