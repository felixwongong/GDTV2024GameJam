using System;
using System.Collections.Generic;
using CofyEngine.Util;

namespace CofyEngine
{
    public struct StateChangeRecord<TStateId>
    {
        public BaseState<TStateId> oldState;
        public BaseState<TStateId> newState;
    }
    
    public class StateMachine<TStateId>: IStateMachine<TStateId> where TStateId : Enum
    {
        private BaseState<TStateId> _prevoutState;
        private BaseState<TStateId> _curState;
        public BaseState<TStateId> previousState => _prevoutState;
        public BaseState<TStateId> currentState => _curState;

        private Dictionary<TStateId, BaseState<TStateId>> _stateDictionary = new();

        
        public CofyEvent<StateChangeRecord<TStateId>> onBeforeStateChange = new();
        public CofyEvent<StateChangeRecord<TStateId>> onAfterStateChange = new();

        private bool logging;
        
        #pragma warning disable 0414
        IRegistration loggingReg;
        #pragma warning restore 0414
        
        public StateMachine(bool logging = false)
        {
            this.logging = logging;
            if (logging)
            {
                loggingReg = onBeforeStateChange.Register(

                    rec =>
                    {
                        FLog.Log(rec.oldState != null
                            ? string.Format("Transit from state [{0}] to [{1}]", rec.oldState.GetTName(),
                                rec.newState.GetTName())
                            : string.Format("Start initial state [{0}]", rec.newState.GetTName()));
                    }
                );
            }
        }
        
        public void RegisterState(BaseState<TStateId> state)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));
            if (_stateDictionary.ContainsKey(state.id))
            {
                throw new Exception($"State {state.GetType()} already registered");
            }
            FLog.Log($"Register state {state.id}");
            _stateDictionary[state.id] = state;
        }

        public void GoToState(TStateId id, in object param = null)
        {
            if (!_curState.isRefNull())
            {
                _curState.OnEndContext();
                _prevoutState = _curState;
            }
            
            if (!_stateDictionary.TryGetValue(id, out _curState))
                throw new Exception(string.Format("State {0} not registered", id));
            
            onBeforeStateChange?.Invoke(new StateChangeRecord<TStateId>() {oldState = _prevoutState, newState = _curState});
            _curState.StartContext(this, param);
            onAfterStateChange?.Invoke(new StateChangeRecord<TStateId>() {oldState = _prevoutState, newState = _curState});
        }

        public void GoToStateNoRepeat(TStateId id, in object param = null)
        {
            if(currentState.id.Equals(id))
                GoToState(id, param);
            else if (logging)
                FLog.LogWarning(string.Format("Trying to go to the same state, {0}", id));
        }

        T IStateMachine<TStateId>.GetState<T>(TStateId id)
        {
            throw new NotImplementedException();
        }

        public T GetState<T>(TStateId id) where T : BaseState<TStateId>
        {
            if (!_stateDictionary.ContainsKey(id))
            {
                throw new Exception($"State {typeof(T)} not registered");
            }
            return (T) _stateDictionary[id];
        }
    }
}