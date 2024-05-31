using System;
using System.Collections.Generic;
using UnityEngine;

namespace CofyEngine
{
    public class MainThreadExecutor : MonoInstance<MainThreadExecutor>, IDisposable
    {
        public delegate void CancelAction();
        public override bool persistent => true;

        private Queue<WeakReference<Action>> _actionQueue = new();
        private List<WeakReference<Action>> _actionPersistent = new();

        private Action _cache;
        public void OnUpdate()
        {
            for (var i = _actionPersistent.Count - 1; i > 0; i--)
            {
                var wr = _actionPersistent[i];
                if (!wr.TryGetTarget(out _cache))
                    _actionPersistent.RemoveAt(i);
                else
                    _cache.Invoke();
            }

            while (_actionQueue.Count > 0)
            {
                var wr = _actionQueue.Dequeue();
                if (wr.TryGetTarget(out _cache))
                    _cache();
            }
        }

        public void QueueAction(in Action action)
        {
            _actionQueue.Enqueue(new WeakReference<Action>(action));
        }
        
        public CancelAction QueueUpdate(in Action action)
        {
            var wr = new WeakReference<Action>(action);
            _actionPersistent.Add(wr);

            return () => wr.SetTarget(null);
        }

        public void Dispose()
        {
            _actionQueue = null;
            _actionPersistent = null;
        }
    }
}