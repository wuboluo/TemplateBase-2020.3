using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ClickEffect : PooledObject
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    protected override void OnWakeUp()
    {
    }

    protected override void OnFinish()
    {
    }
    
    public void Play()
    {
        DelayedFinish(_animator.GetCurrentClipLength());
    }
}