using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using MyGame.AudioSetting;
using TMPro;

public class BpmAnalyzer : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private BpmAnalyzerSettings _settings;

    [Header("イベント通知先")]
    [SerializeField] private GameEvent BeatEvent;


    private float _lastHitTime;
    private bool _isBeatSustained;

    private List<float> _intervals = new List<float>();
    private const int MaxIntervals = 8;

    public float EstimatedBpm { get; private set; }
    public float KickVol { get; private set; }

    private float kickVol;

    private void Update()
    {
        var analyzer = AudioAnalyzeService.Instance;
        var soundManager = SoundService.Instance as UnitySoundManager;

        if(analyzer == null || soundManager == null) return;


        bool isBeat = CheckBeat(analyzer.SpectrumData, Time.time, soundManager.CurrentBgmPitch);

        if (isBeat)
        {
            ProcessBeat(Time.time);
        }
    }

    public bool CheckBeat(float[] spectrum, float currentTime, float pitch)
    {
        if (_settings == null || spectrum == null || spectrum.Length == 0) return false;

        int dynamicMin = Mathf.RoundToInt(_settings.fftParamMin * pitch);
        int dynamicMax = Mathf.RoundToInt(_settings.fftParamMax * pitch);
        KickVol = GetBandAverage(spectrum, dynamicMin, dynamicMax);

        float highVol = GetBandAverage(spectrum, _settings.highFreqMin, _settings.highFreqMax);
        float dynamicCoolDown = _settings.baseCoolDownTime / pitch;

        bool detected = false;
        if (KickVol > _settings.threshold && KickVol > highVol * _settings.kickBias)
        {
            if (!_isBeatSustained && currentTime > _lastHitTime + dynamicCoolDown)
            {
                detected = true;
                _isBeatSustained = true;
            }
        }
        else if (KickVol < _settings.threshold * 0.8f)
        {
            _isBeatSustained = false;
        }
        return detected;
    }

    private float GetBandAverage(float[] spectrum, int min, int max)
    {
        if (min < 0 || max >= spectrum.Length || min > max) return 0;

        float sum = 0;
        for(int i = min; i <= max; i++)
        {
            sum += spectrum[i];
        }
        return sum / (max - min + 1);

    }

    public void ProcessBeat(float currentTime, bool raiseEvent = true)
    {
        float interval = currentTime - _lastHitTime;
        if (_lastHitTime > 0 && interval < 2.0f)
        {
            _intervals.Add(interval);
            if (_intervals.Count > MaxIntervals) _intervals.RemoveAt(0);

            float avgInterval = 0;
            _intervals.ForEach(i => avgInterval += i);
            avgInterval /= _intervals.Count;

            EstimatedBpm = 60 / avgInterval;

            if (raiseEvent)
            {
                BeatEvent?.Raise();
            }
        }
        _lastHitTime = currentTime;
    }

    public void ResetState()
    {
        _lastHitTime = 0;
        _isBeatSustained = false;
        _intervals.Clear();
    }
}
