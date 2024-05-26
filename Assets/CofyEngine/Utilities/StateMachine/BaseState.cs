using System;

namespace CofyEngine
{
    public abstract class BaseState<TStateId> where TStateId: Enum
    {
        public abstract TStateId id { get; }
        protected internal abstract void StartContext(StateMachine<TStateId> sm, object param);
        protected internal virtual void OnEndContext() {}
   }
}