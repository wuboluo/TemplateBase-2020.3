using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace AN_Match3
{
    public class Clock : MonoBehaviour
    {
        [SerializeField] private SourceConfigEventChannelSO playSourceEvent;
        
        private const float MAXTimerValue = 360f;
        private const float ScaleSize = 1.3f;
        private static readonly int ArcValue = Shader.PropertyToID("_Arc1");

        // [SerializeField] private SingleSource ;
        [SerializeField] private int maxTime;
        [SerializeField] private SpriteRenderer countDown;
        [SerializeField] private Text timeLabel;
        private int _second;
        private float _value;

        private bool _inScaling;
        private bool _inTimeCountdown;

        private EventListener<int> _listener;

        public Action OnTimeUpAction;

        private void Start()
        {
            _listener = new EventListener<int>();
            _listener.OnVariableChange += AtTimeChanged;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.C)) StartTheTimer();
            if (Input.GetKeyDown(KeyCode.V)) FinishTheTimer();

            if (!_inTimeCountdown) return;

            _value += MAXTimerValue / maxTime * Time.deltaTime;
            countDown.material.SetFloat(ArcValue, _value);

            _second = maxTime + 1 - (int) Math.Ceiling(countDown.material.GetFloat(ArcValue) / MAXTimerValue * maxTime);
            _listener.Value = _second;

            if (_value >= MAXTimerValue)
            {
                FinishTheTimer();
                OnTimeUpAction?.Invoke();
            }
        }

        private void OnDisable()
        {
            _listener.OnVariableChange -= AtTimeChanged;
        }

        private void AtTimeChanged(int time)
        {
            OnSpecialMomentExample(_second, maxTime);

            timeLabel.text = _second < 10 ? $"00:0{_second}" : $"00:{_second}";

            StartCoroutine(nameof(PlayScaleAnimationAtTimeChanged));
            if (null != countDownClipConfig) playSourceEvent.RaiseEvent(countDownClipConfig);
        }

        public void StartTheTimer()
        {
            _inTimeCountdown = true;
        }

        public void FinishTheTimer()
        {
            _inTimeCountdown = false;
        }

        private IEnumerator PlayScaleAnimationAtTimeChanged()
        {
            _inScaling = true;
            var scale = ScaleSize;

            while (_inScaling)
            {
                scale -= Time.deltaTime * 0.5f;

                SetScale(scale, countDown.transform.parent, timeLabel.transform);
                yield return 0;

                if (scale > 1) continue;
                _inScaling = false;
            }
        }

        private void OnSpecialMomentExample(int currentSecond, int maxSecond)
        {
            if (maxSecond - currentSecond == maxSecond / 3)
            {
                print($"当时间过去三分之一时，当前是第{maxSecond - currentSecond}s，一共{maxSecond}s");
            }

            if (maxSecond - currentSecond == maxSecond / 4 * 3)
            {
                print($"当时间过去四分之三时，当前是第{maxSecond - currentSecond}s，一共{maxSecond}s");
            }
        }

        private static void SetScale(float scale, params Transform[] elements)
        {
            elements.ToList().ForEach(e => e.SetScale(scale));
        }
    }
}