using System;
using System.Collections.Generic;
using CofyEngine.Util;
using Unity.Netcode;
using UnityEngine;

namespace CofyEngine.Network
{
   public abstract class NetworkStateMachine<TStateId> : NetworkBehaviour where TStateId : Enum
    {
        public struct StateChangeRecord<TStateId> where TStateId : Enum
        {
            public NetworkState<TStateId> oldState;
            public NetworkState<TStateId> newState;
        }

        private NetworkState<TStateId> _prevoutState;
        private NetworkState<TStateId> _curState;
        public NetworkState<TStateId> previousState => _prevoutState;
        public NetworkState<TStateId> currentState => _curState;

        private Dictionary<TStateId, NetworkState<TStateId>> _stateDictionary = new();


        public CofyEvent<StateChangeRecord<TStateId>> onBeforeStateChange = new();
        public CofyEvent<StateChangeRecord<TStateId>> onAfterStateChange = new();

        protected virtual void Awake()
        {
            var states = GetComponents<NetworkState<TStateId>>();
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

        public void RegisterState(NetworkState<TStateId> state)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));
            if (_stateDictionary.ContainsKey(state.id))
            {
                throw new Exception($"State {state.GetType()} already registered");
            }

            FLog.Log($"Register state {state.id}");
            _stateDictionary[state.id] = state;

            state.stateMachine = this;
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
            _curState.StartContext(param);
            onAfterStateChange?.Invoke(new StateChangeRecord<TStateId>()
                { oldState = _prevoutState, newState = _curState });
        }

        public void GoToStateNoRepeat(TStateId id, in object param = null)
        {
            if (!currentState.id.Equals(id))
                GoToState(id, param);
        }

        public T GetState<T>(TStateId id) where T : NetworkState<TStateId>
        {
            if (!_stateDictionary.ContainsKey(id))
            {
                throw new Exception($"State {typeof(T)} not registered");
            }

            return (T)_stateDictionary[id];
        }
    }

}