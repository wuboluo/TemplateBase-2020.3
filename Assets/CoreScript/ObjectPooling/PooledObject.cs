using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public abstract class PooledObject : MonoBehaviour
{
    protected void OnEnable()
    {
        OnWakeUp();
    }

    private void OnDisable()
    {
        StopCoroutine(nameof(DelayedFinishCor));
    }

    public event UnityAction<PooledObject> OnFinished;

    protected void DelayedFinish(float aliveTime)
    {
        StartCoroutine(nameof(DelayedFinishCor), aliveTime);
    }

    private IEnumerator DelayedFinishCor(float dur)
    {
        yield return new WaitForSeconds(dur);

        OnFinish();
        OnFinished?.Invoke(this);
    }

    protected abstract void OnWakeUp();
    protected abstract void OnFinish();
}