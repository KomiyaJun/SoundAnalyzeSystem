using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using MyGame.AudioSetting;
using TMPro;

public class BpmAnalyzer : MonoBehaviour
{
    [Header("イベント通知先")]
    [SerializeField] private GameEvent BeadEvent;

    [Header("設定")]
    [SerializeField] private float threshold = 0.5f;
    [SerializeField] private float baseCoolDownTime = 0.25f;
    [SerializeField] private int fftParamMin = 0;
    [SerializeField] private int fftParamMax = 2;

    [Header("スネアの回避設定")]
    [SerializeField] private int highFreqMin = 10;
    [SerializeField] private int highFreqMax = 20;
    [SerializeField] private float kickBias = 1.5f;

    private float _lastHitTime;
    private bool _isBeatSustained;

    private List<float> _intervals = new List<float>();
    private const int MaxIntervals = 8;

    public float EstimatedBpm { get; private set; }

    private float kickVol;

    private void Update()
    {
        var analyzer = AudioAnalyzeService.Instance;
        var soundManager = SoundService.Instance as UnitySoundManager;

        if(analyzer == null || soundManager == null) return;

        float pitch = soundManager.CurrentBgmPitch;

        int dynamicMin = Mathf.RoundToInt(fftParamMin * pitch);
        int dynamicMax = Mathf.RoundToInt(fftParamMax * pitch);

        kickVol = analyzer.GetBandAverage(dynamicMin, dynamicMax);
        float highVol = analyzer.GetBandAverage(highFreqMin, highFreqMax);

        float dynamicCoolDown = baseCoolDownTime / pitch;

        if (kickVol > threshold && kickVol > highVol * kickBias)
        {
            if(!_isBeatSustained && Time.time > _lastHitTime + dynamicCoolDown)
            {
                OnBeatDetected();
                _isBeatSustained = true;
            }
        }
        else
        {
            if (kickVol < threshold * 0.8f)
            {
                _isBeatSustained = false;
            }
        }
    }

    private void OnBeatDetected()
    {
        float currentTime = Time.time;
        float interval = currentTime - _lastHitTime;

        if(_lastHitTime > 0 && interval < 2.0f)
        {
            _intervals.Add(interval);
            if (_intervals.Count > MaxIntervals) _intervals.RemoveAt(0);

            //平均感覚からBPMを出す
            float avgInterval = 0;
            _intervals.ForEach(i => avgInterval += i);
            avgInterval /= _intervals.Count;

            EstimatedBpm = 60f / avgInterval;

            BeadEvent?.Raise();
        }

        _lastHitTime = currentTime;
    }
}
