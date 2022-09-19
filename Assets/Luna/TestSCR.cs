using System;
using System.Collections;
using Luna.Unity;
using UnityEngine;

public class TestSCR : MonoBehaviour
{
    public event Action OnSCRFirst;

    private readonly MyTextListener _myTextListener = new MyTextListener();
    private bool _crossJudge;
    private bool _first;

    private bool _isTouch;
    private bool _isWin;
    private bool _verctJudge;
    public static TestSCR Instance { get; private set; }

    public bool first;

    private void Awake()
    {
        Instance = this;

        IE_Loading();
        AudioListener.volume = 0;
        first = false;
    }

    private void Update()
    {
        if (!_crossJudge && Screen.width > Screen.height)
        {
            _crossJudge = true;
            _verctJudge = false;
            if (_first) Resize();
            _first = true;
        }
        else if (!_verctJudge && Screen.height > Screen.width)
        {
            _verctJudge = true;
            _crossJudge = false;
            if (_first) Resize();
            _first = true;
        }

        if (_isTouch) return;
        if (Input.GetMouseButtonDown(0))
        {
            Engagement();
            _isTouch = true;
        }
    }

    private void IE_Loading()
    {
        LoadingComplete();
        Tutorial();
    }

    public void GoStore()
    {
        _myTextListener.GoStore();
    }

    public void Finish()
    {
        Analytics.LogEvent("Finish", 0);
        LifeCycle.GameEnded();
        _myTextListener.Finish();
    }

    private void Resize()
    {
        Analytics.LogEvent("Resize", 0);
        _myTextListener.Resize();
    }

    public void Lose()
    {
        if (_isWin) return;
        Analytics.LogEvent(Analytics.EventType.LevelFailed);
        _myTextListener.Lose();
        Finish();
        _isWin = true;
    }

    public void Win()
    {
        if (_isWin) return;
        Analytics.LogEvent(Analytics.EventType.LevelWon);
        _myTextListener.Win();
        Finish();
        _isWin = true;
    }

    private void Tutorial()
    {
        Analytics.LogEvent(Analytics.EventType.TutorialStarted);
        _myTextListener.Tutorial();
    }

    private void Engagement()
    {
        AudioListener.volume = 1;
        first = true;
        OnSCRFirst?.Invoke();

        _myTextListener.Engagement();
    }

    private void LoadingComplete()
    {
        Analytics.LogEvent("LoadingComplete", 0);
        _myTextListener.LoadingComplete();
    }

    public void CustomFunction(string message)
    {
        Analytics.LogEvent(message, 0);
        _myTextListener.Customfunction(message);
    }

    public int ReturnNumberIndex(int index)
    {
        var i = 0;
        switch (index)
        {
            case 0:
                i = _myTextListener.Addnumber();
                break;
            case 1:
                i = _myTextListener.Addnumber1();
                break;
            case 2:
                i = _myTextListener.Addnumber2();
                break;
        }

        return i;
    }

    public bool ReturnIsLuan()
    {
        return _myTextListener.Isluna();
    }

    public void CopyText(string copyCode)
    {
        _myTextListener.CopyAA(copyCode);
    }
}