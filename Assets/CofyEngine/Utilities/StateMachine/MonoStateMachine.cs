using System;
using UnityEngine;

namespace CofyEngine
{
    public abstract class MonoStateMachine<TStateId> : MonoBehaviour, IStateMachine<TStateId> where TStateId: Enum
    {
        [SerializeField] private bool logging;
        
        private StateMachine<TStateId> _sm;

        protected virtual void Awake()
        {
            _sm = new StateMachine<TStateId>(logging);
        }

        public void RegisterState(BaseState<TStateId> state)
        {
            _sm.RegisterState(state);
        }

        public void GoToState(TStateId id, in object param = null)
        {
            _sm.GoToState(id, param);
        }

        public void GoToStateNoRepeat(TStateId id, in object param = null)
        {
            _sm.GoToStateNoRepeat(id, param);
        }

        public T GetState<T>(TStateId id) where T : BaseState<TStateId>
        {
            return _sm.GetState<T>(id);
        }
    }
}