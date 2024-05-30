using System;
using System.Collections;
using System.Collections.Generic;

namespace CofyEngine
{
    public class CoroutineExecutor: MonoInstance<CoroutineExecutor>
    {
        public void execute(IEnumerator coroutine, Action onCompleted = null)
        {
            StartCoroutine(_execute(coroutine, onCompleted));
        }

        private IEnumerator _execute(IEnumerator coroutine, Action onCompleted = null)
        {
            yield return StartCoroutine(coroutine);
            onCompleted?.Invoke();
        }
    }
}