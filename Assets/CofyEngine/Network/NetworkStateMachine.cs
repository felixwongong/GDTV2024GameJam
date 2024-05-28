using System;
using System.Collections.Generic;
using CofyEngine.Util;
using Unity.Netcode;
using UnityEngine;

namespace CofyEngine.Network
{
    public interface IStateMachine<TStateId> where TStateId : Enum
    {
        public void GoToState(TStateId id);
        public void GoToStateNoRepeat(TStateId id);
    }
    
    [GenerateSerializationForGenericParameter(0)]
    [RequireComponent(typeof(NetworkObject))]
   public abstract class NetworkStateMachine<TStateId, TState> : NetworkBehaviour, IStateMachine<TStateId> where TStateId : Enum where TState: NetworkState<TStateId>
    {
        public struct StateChangeRecord
        {
            public TStateId oldState;
            public TStateId newState;
        }

        public NetworkVariable<TStateId> currentStateId;
        private TState _prevoutState;
        private TState _currentState;
        public TState previousState => _prevoutState;

        public TState currentState
        {
            get
            {
                if (_currentState == null || !_currentState.id.Equals(currentStateId.Value))
                {
                    _currentState = GetState(currentStateId.Value);
                }

                return _currentState;
            }
        }

        private Dictionary<TStateId, TState> _stateDictionary = new();


        public CofyEvent<StateChangeRecord> onBeforeStateChange = new();
        public CofyEvent<StateChangeRecord> onAfterStateChange = new();

        protected virtual void Awake()
        {
            var states = GetComponents<TState>();
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

        public void RegisterState(TState state)
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

        public void GoToState(TStateId id)
        {
            GoToStateServerRpc(id);
        } 
        
        [Rpc(SendTo.Server)]
        private void GoToStateServerRpc(TStateId id)
        {
            if (!currentState.isRefNull())
            {
                changeStateClientRpc(id);
            }

            currentStateId.Value = GetState(id).id;
            if(_prevoutState != null)
                invokeBeforeStateChangeEventRpc(_prevoutState.id, currentState.id);
            currentState.StartContextClientRpc();
            if(_prevoutState != null)
                invokeAfterStateChangeEventRpc(_prevoutState.id, currentState.id);
        }


        public void GoToStateNoRepeat(TStateId id)
        {
            GoToStateNoRepeatServerRpc(id);
        }
        
        [Rpc(SendTo.Server)]
        private void GoToStateNoRepeatServerRpc(TStateId id)
        {
            if (!_currentState.id.Equals(id))
                GoToState(id);
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void changeStateClientRpc(TStateId nextStateId)
        {
            _currentState.OnEndContext();
            _prevoutState = _currentState;
        }
        
        [Rpc(SendTo.ClientsAndHost)]
        private void invokeBeforeStateChangeEventRpc(TStateId prevousSteteId, TStateId nextStateId)
        {
            onBeforeStateChange?.Invoke(new StateChangeRecord()
            {
                oldState = prevousSteteId, newState = nextStateId
            });
        }
        
        [Rpc(SendTo.ClientsAndHost)]
        private void invokeAfterStateChangeEventRpc(TStateId prevousSteteId, TStateId nextStateId)
        {
            onAfterStateChange?.Invoke(new StateChangeRecord()
            {
                oldState = prevousSteteId, newState = nextStateId
            });
        }

        public TState GetState(TStateId id)
        {
            if (!_stateDictionary.ContainsKey(id))
            {
                throw new Exception($"State {id.ToString()} not registered");
            }

            return _stateDictionary[id];
        }
    }
}