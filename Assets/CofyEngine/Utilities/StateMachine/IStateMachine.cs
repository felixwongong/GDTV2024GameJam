namespace CofyEngine
{
    public interface IStateMachine<TStateId>
    {
        public void RegisterState(BaseState<TStateId> state);
        public void GoToState(TStateId id, in object param = null);
        public void GoToStateNoRepeat(TStateId id, in object param = null);
        public T GetState<T>(TStateId id) where T : BaseState<TStateId>;
    }
}