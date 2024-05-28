using System;
using Unity.Netcode;
using UnityEngine;

namespace CofyEngine.Network
{
    [GenerateSerializationForGenericParameter(0)]
    [RequireComponent(typeof(NetworkObject))]
    public abstract class NetworkState<TStateId>: NetworkBehaviour where TStateId: Enum
    {
        protected internal IStateMachine<TStateId> stateMachine;

        public abstract TStateId id { get; }

        public virtual void _Awake() {}
        public virtual void _Start() { }
        
        public virtual void _Update(double delta) {}

        public virtual void _FixedUpdate(double fixedDelta) { }

        [Rpc(SendTo.ClientsAndHost)]
        protected internal  void StartContextClientRpc()
        {
            StartContext();
        }

        protected abstract void StartContext();

        protected internal virtual void OnEndContext()
        {
            
        }
    }
}