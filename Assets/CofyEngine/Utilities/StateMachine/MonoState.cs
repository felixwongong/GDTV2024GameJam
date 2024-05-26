using System;
using UnityEngine;

namespace CofyEngine
{
    public abstract class MonoState<TStateId>: MonoBehaviour where TStateId: Enum
    {
        public abstract TStateId id { get; }

        public virtual void _Awake() {}
        public virtual void _Start() { }
        
        public virtual void _Update(double delta) {}

        public virtual void _FixedUpdate(double fixedDelta) { }

        protected internal abstract void StartContext(MonoStateMachine<TStateId> sm, object param);

        protected internal virtual void OnEndContext()
        {
        }
    }
}