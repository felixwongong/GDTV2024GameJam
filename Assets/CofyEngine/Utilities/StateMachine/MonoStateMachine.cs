using System;
using System.Collections.Generic;
using CofyEngine.Util;
using UnityEngine;

namespace CofyEngine
{
    public abstract class MonoStateMachine<TStateId> : MonoBehaviour where TStateId : Enum
    {
        public struct StateChangeRecord<TStateId> where TStateId : Enum
        {
            public MonoState<TStateId> oldState;
            public MonoState<TStateId> newState;
        }

        private MonoState<TStateId> _prevoutState;
        private MonoState<TStateId> _curState;
        public MonoState<TStateId> previousState => _prevoutState;
        public MonoState<TStateId> currentState => _curState;

        private Dictionary<TStateId, MonoState<TStateId>> _stateDictionary = new();


        public CofyEvent<StateChangeRecord<TStateId>> onBeforeStateChange = new();
        public CofyEvent<StateChangeRecord<TStateId>> onAfterStateChange = new();

        protected virtual void Awake()
        {
            var states = GetComponents<MonoState<TStateId>>();
            for (var i = 0; i < states.Length; i++)
            {
                RegisterState(states[i]);
                states[i]._Awake();
            }
        }

        protected virtual void Start()
        {
            foreach (var (_, state) in _stateDictionary)
            {
                state._Start();
            }
        }

        protected virtual void Update()
        {
            if (_curState != null)
            {
                _curState._Update(Time.deltaTime);
            }
        }

        private void FixedUpdate()
        {
            if (_curState != null)
            {
                _curState._FixedUpdate(Time.fixedDeltaTime);
            }
        }

        public void RegisterState(MonoState<TStateId> state)
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

            onBeforeStateChange?.Invoke(new StateChangeRecord<TStateId>()
                { oldState = _prevoutState, newState = _curState });
            _curState.StartContext(this, param);
            onAfterStateChange?.Invoke(new StateChangeRecord<TStateId>()
                { oldState = _prevoutState, newState = _curState });
        }

        public void GoToStateNoRepeat(TStateId id, in object param = null)
        {
            if (!currentState.id.Equals(id))
                GoToState(id, param);
        }

        public T GetState<T>(TStateId id) where T : MonoState<TStateId>
        {
            if (!_stateDictionary.ContainsKey(id))
            {
                throw new Exception($"State {typeof(T)} not registered");
            }

            return (T)_stateDictionary[id];
        }
    }
}