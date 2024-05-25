using System;
using System.Diagnostics;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace CofyEngine.Editor
{
    [Conditional("UNITY_EDITOR"), AttributeUsage(AttributeTargets.Method)]
    public class MethodButtonAttribute : Attribute
    {
        public string buttonName;
        public bool titled;

        public MethodButtonAttribute(string buttonName = "", bool titled = true)
        {
            this.buttonName = buttonName;
            this.titled = titled;
        }
    }


    [CustomEditor(typeof(MonoBehaviour), true)]
    public class MethodButtonEditor : UnityEditor.Editor
    {
        private GUIStyle _headerStyle;
        private object[] parameterValues;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (_headerStyle == null)
            {
                _headerStyle = new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold };
            }

            MonoBehaviour mono = target as MonoBehaviour;
            if (mono == null)
            {
                Debug.Log($"target {target} is not a MonoBehaviour!");
                return;
            }

            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            MethodInfo[] methods = mono.GetType().GetMethods(flags);

            for (var i = 0; i < methods.Length; i++)
            {
                var method = methods[i];
                
                var attr = (MethodButtonAttribute) Attribute.GetCustomAttribute(methods[i], typeof(MethodButtonAttribute));
                if (attr == null) continue;

                if (attr.buttonName.isNullOrEmpty()) attr.buttonName = method.Name;
                
                var parameters = method.GetParameters();
                parameterValues ??= new object[parameters.Length];

                #region Layout

                if (attr.titled)
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(attr.buttonName, _headerStyle);
                }

                for (var j = 0; j < parameters.Length; j++)
                {
                    var parameter = parameters[j];
                    EditorGUILayout.BeginHorizontal();
                    Type type = parameter.ParameterType;
                    parameterValues[j] = CustomInspector.DrawField(type, parameter.Name, parameterValues[j]);
                    EditorGUILayout.EndHorizontal();
                }

                if (GUILayout.Button("Invoke " + attr.buttonName))
                {
                    method.Invoke(mono, parameterValues);
                }

                #endregion

            }
        }
    }
    
}