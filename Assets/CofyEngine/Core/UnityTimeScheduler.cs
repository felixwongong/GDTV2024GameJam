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

        private void Update()
        {
            for (var i = 0; i < _tasks.Count; i++)
            {
                var task = _tasks[i];
                if (task.perFrameAction != null)
                {
                    var startTime = task.endTime - task.duration;
                    var percent = (float)((Time.timeAsDouble * 1000 - startTime)/ (task.endTime - startTime));
                    task.perFrameAction((Mathf.Clamp01(percent)));
                }
                
                if (Time.timeAsDouble * 1000 > task.endTime)
                {
                    task.taskAction();
                    _pool.Release(task);
                    _tasks.RemoveAt(i);
                }
            }
        }

        public void AddDelay(double ms, Action task, Action<float> perFrameAction = null)
        {
            var invokeMS = Time.timeAsDouble * 1000 + ms;
            _tasks.Add(_pool.Get().Set(ms, invokeMS, task, perFrameAction));
        }
    }

    public class ScheduledTask
    {
        public double duration;
        public double endTime;
        public Action taskAction;
        public Action<float> perFrameAction;

        public ScheduledTask() { }

        public ScheduledTask Set(double duration, double endTime, Action taskAction, Action<float> perFrameAction = null)
        {
            this.duration = duration;
            this.endTime = endTime;
            this.taskAction = taskAction;
            this.perFrameAction = perFrameAction;
            return this;
        }

        public void Clear()
        {
            endTime = double.MinValue;
            taskAction = null;
        }
    }
}