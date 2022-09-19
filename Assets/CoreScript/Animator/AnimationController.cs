using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class AnimationController : MonoBehaviour
{
    public UnityEvent onStartEvents;
    public UnityEvent onCompleteEvents;

    [SerializeField] private CompleteType completeType;

    private Animator _animator;

    private void OnEnable()
    {
        _animator = GetComponent<Animator>();

        StartCoroutine(nameof(DelayClipComplete));
    }

    private IEnumerator DelayClipComplete()
    {
        onStartEvents?.Invoke();

        yield return new WaitForSeconds(_animator.GetCurrentClipLength());

        switch (completeType)
        {
            case CompleteType.KeepAppear:
                break;
            
            case CompleteType.Disappear:
                gameObject.SetActive(false);
                break;

            case CompleteType.Destroy:
                Destroy(gameObject);
                break;
        }

        onCompleteEvents?.Invoke();
    }
}

public enum CompleteType
{
    KeepAppear,
    Disappear,
    Destroy,
}