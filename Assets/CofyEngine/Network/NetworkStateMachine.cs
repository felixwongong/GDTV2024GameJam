using System;
using System.Collections.Generic;
using CofyEngine.Util;
using Unity.Netcode;
using UnityEngine;

namespace CofyEngine.Network
{
    [GenerateSerializationForGenericParameter(0)]
    [RequireComponent(typeof(NetworkObject))]
   public abstract class NetworkStateMachine<TStateId> : NetworkBehaviour where TStateId : Enum
    {
        public struct StateChangeRecord<TStateId> where TStateId : Enum
        {
            public TStateId oldState;
            public TStateId newState;
        }

        private NetworkState<TStateId> _prevoutState;
        private NetworkState<TStateId> _currentState;
        public NetworkState<TStateId> previousState => _prevoutState;
        public NetworkState<TStateId> currentState => _currentState;

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
            if (_currentState != null)
            {
                _currentState._Update(Time.deltaTime);
            }
        }

        private void FixedUpdate()
        {
            if (_currentState != null)
            {
                _currentState._FixedUpdate(Time.fixedDeltaTime);
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

        [Rpc(SendTo.Server)]
        public void GoToStateServerRpc(TStateId id)
        {
            if (!_currentState.isRefNull())
            {
                _currentState.OnEndContextClientRpc();
                _prevoutState = _currentState;
            }

            if (!_stateDictionary.TryGetValue(id, out _currentState))
                throw new Exception(string.Format("State {0} not registered", id));

            if(_prevoutState != null)
                invokeBeforeStateChangeEventRpc(_prevoutState.id, currentState.id);
            _currentState.StartContextClientRpc();
            if(_prevoutState != null)
                invokeAfterStateChangeEventRpc(_prevoutState.id, currentState.id);
        }

        [Rpc(SendTo.Server)]
        public void GoToStateNoRepeatServerRpc(TStateId id)
        {
            if (!currentState.id.Equals(id))
                GoToStateServerRpc(id);
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void invokeBeforeStateChangeEventRpc(TStateId prevousSteteId, TStateId nextStateId)
        {
            onBeforeStateChange?.Invoke(new StateChangeRecord<TStateId>()
            {
                oldState = prevousSteteId, newState = nextStateId
            });
        }
        
        [Rpc(SendTo.ClientsAndHost)]
        private void invokeAfterStateChangeEventRpc(TStateId prevousSteteId, TStateId nextStateId)
        {
            onAfterStateChange?.Invoke(new StateChangeRecord<TStateId>()
            {
                oldState = prevousSteteId, newState = nextStateId
            });
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