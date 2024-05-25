using System;
using UnityEngine;

namespace CofyEngine
{
    public class MonoInstance<T> : MonoBehaviour where T : Component
    {
        public virtual bool persistent => false;
        
        private static T _instance;
        public static T instance
        {
            get
            {
                if (_instance != null) return _instance;
                
                _instance = FindObjectOfType<T>();

                if (_instance != null) return _instance;
                
                _instance = new GameObject($"_{typeof(T).Name}").AddComponent<T>();
                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if(persistent) DontDestroyOnLoad(this);
        }
    }
}