using System;
using CofyEngine;
using UnityEngine;
using UnityEngine.Pool;

namespace CofyEngine
{
    public class UnityTimeScheduler: MonoInstance<UnityTimeScheduler>
    {
        private SortedSeq<ScheduledTask> _tasks;
        private ObjectPool<ScheduledTask> _pool;

        protected override void Awake()
        {
            base.Awake();
            _tasks = new SortedSeq<ScheduledTask>(task => task.endTime);
            _pool = new ObjectPool<ScheduledTask>(() => new ScheduledTask());
        }

        private Action _taskActionCache;
        private Action<float> _perFrameActionCache;
        private void Update()
        {
            for (var i = 0; i < _tasks.Count; i++)
            {
                var task = _tasks[i];
                if (task.perFrameAction != null && task.perFrameAction.TryGetTarget(out _perFrameActionCache))
                {
                    var startTime = task.endTime - task.duration;
                    var percent = (float)((Time.timeAsDouble * 1000 - startTime)/ (task.endTime - startTime));
                    _perFrameActionCache((Mathf.Clamp01(percent)));
                }
                
                if (Time.timeAsDouble * 1000 > task.endTime && task.taskAction != null && task.taskAction.TryGetTarget(out _taskActionCache))
                {
                    _taskActionCache();
                    _pool.Release(task);
                    _tasks.RemoveAt(i);
                }
            }
        }

        public void AddDelay(double ms, Action task, Action<float> perFrameAction = null)
        {
            var invokeMS = Time.timeAsDouble * 1000 + ms;
            _tasks.Add(_pool.Get().Set(new ScheduledTask.Param()
            {
                duration = ms, endTime = invokeMS, taskAction = task, perFrameAction = perFrameAction
            }));
        }
    }

    public class ScheduledTask
    {
        public struct Param
        {
            public double duration;
            public double endTime;
            public Action taskAction;
            public Action<float> perFrameAction;
        }
        
        public double duration;
        public double endTime;
        public WeakReference<Action> taskAction;
        public WeakReference<Action<float>> perFrameAction;

        public ScheduledTask() { }

        public ScheduledTask Set(Param param)
        {
            this.duration = param.duration;
            this.endTime = param.endTime;
            this.taskAction = new WeakReference<Action>(param.taskAction);
            this.perFrameAction = new WeakReference<Action<float>>(param.perFrameAction);
            return this;
        }

        public void Clear()
        {
            endTime = double.MinValue;
            taskAction = null;
        }
    }
}