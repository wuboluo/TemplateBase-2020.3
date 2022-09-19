using System;
using DG.Tweening;
using UnityEngine;

public class CameraVisionChanger : MonoBehaviour
{
    private Sequence _camVisionSequence;
    private CameraAdapter _cameraAdapter;
    private Camera _mainCam;

    public bool Moving { get; private set; }

    private void Start()
    {
        _camVisionSequence = DOTween.Sequence();
        _camVisionSequence.SetAutoKill(false);

        _mainCam = Camera.main;
        _cameraAdapter = GetComponent<CameraAdapter>();
    }

    public void GoTo_Position(Vector3 endingPos, float dur, bool keepAdapterOnComplete, Action onStart = null, Action onComplete = null)
    {
        _camVisionSequence.Kill();
        _camVisionSequence = DOTween.Sequence();

        var posMoveTween = _mainCam.transform.DOMove(endingPos, dur).SetEase(Ease.InOutSine);
        _camVisionSequence.Append(posMoveTween);

        OnConfigComplete(keepAdapterOnComplete, onStart, onComplete);
    }

    public void GoTo_Size(float endingSize, float dur, bool keepAdapterOnComplete, Action onStart = null, Action onComplete = null)
    {
        _camVisionSequence.Kill();
        _camVisionSequence = DOTween.Sequence();

        var sizeMoveTween = _mainCam.DOOrthoSize(endingSize, dur).SetEase(Ease.InOutSine);
        _camVisionSequence.Append(sizeMoveTween);

        OnConfigComplete(keepAdapterOnComplete, onStart, onComplete);
    }

    public void GoTo_PosAndSize(Vector3 endingPos, float endingSize, float dur, bool keepAdapterOnComplete, Action onStart = null, Action onComplete = null)
    {
        _camVisionSequence.Kill();
        _camVisionSequence = DOTween.Sequence();

        var posMoveTween = _mainCam.transform.DOMove(endingPos, dur).SetEase(Ease.InOutSine);
        var sizeMoveTween = _mainCam.DOOrthoSize(endingSize, dur).SetEase(Ease.InOutSine);

        _camVisionSequence.Append(posMoveTween);
        _camVisionSequence.Join(sizeMoveTween);

        OnConfigComplete(keepAdapterOnComplete, onStart, onComplete);
    }

    private void OnConfigComplete(bool keepAdapterOnComplete, Action onStart = null, Action onComplete = null)
    {
        Action onStartAction = () =>
        {
            _cameraAdapter.useAdapter = false;
            Moving = true;
        };

        Action onCompleteAction = () =>
        {
            _cameraAdapter.useAdapter = keepAdapterOnComplete;
            Moving = false;
        };

        if (onStart != null) onStartAction += onStart;
        if (onComplete != null) onCompleteAction += onComplete;

        _camVisionSequence.OnStart(() => onStartAction?.Invoke()).OnComplete(() => onCompleteAction?.Invoke());
        _camVisionSequence.Play();
    }
}