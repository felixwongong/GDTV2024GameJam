namespace CofyEngine
{
    public abstract class BaseState<TStateId>
    {
        public abstract TStateId id { get; }
        protected internal abstract void StartContext(IStateMachine<TStateId> sm, object param);
        protected internal virtual void OnEndContext() {}
   }
}