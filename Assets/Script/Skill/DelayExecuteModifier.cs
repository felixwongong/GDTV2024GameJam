using System;
using System.Collections;
using System.Runtime.CompilerServices;
using CofyEngine;
using UnityEngine;
using Event = CofyEngine.Event;

public interface IPercentPrgress
{
    public float getProgress();
}

public class DelayExecuteModifier : MonoBehaviour, IPercentPrgress
{
    private float _second = 1;
    public float second => _second;

    private float _progress;

    public IEnumerator wait()
    {
        var completed = false;
        bool isCompleted () => completed;
        
        UnityTimeScheduler.instance.AddDelay(
            getDelaySecond() * 1000,
            () =>
            {
                completed = true;
            }, 
            percent => _progress = percent);

        yield return new WaitUntil(isCompleted);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual float getDelaySecond()
    {
        return second;
    }

    public float getProgress()
    {
        return _progress;
    }
}
