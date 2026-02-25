using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using MyGame.AudioSetting;

public class BpmAnalyzer : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private float threshold = 0.5f;
    [SerializeField] private float baseCoolDownTime = 0.25f;

    private float _lastHitTime;
    private bool _isBeatSustained;

    private List<float> _intervals = new List<float>();
    private const int MaxIntervals = 8;

    public UnityEvent OnBeatDetectedEvent;

    public float EstimatedBpm { get; private set; }

    private void Update()
    {
        var analyzer = AudioAnalyzeService.Instance;
        var soundManager = SoundService.Instance as UnitySoundManager;

        if(analyzer == null || soundManager == null) return;

        float pitch = soundManager.CurrentBgmPitch;

        int dynamicMin = Mathf.RoundToInt(0 * pitch);
        int dynamicMax = Mathf.RoundToInt(2 * pitch);

        float kickVol = analyzer.GetBandAverage(dynamicMin, dynamicMax);

        float dynamicCoolDown = baseCoolDownTime / pitch;

        if (kickVol > threshold)
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

            //•½‹ÏŠ´Šo‚©‚çBPM‚ðo‚·
            float avgInterval = 0;
            _intervals.ForEach(i => avgInterval += i);
            avgInterval /= _intervals.Count;

            EstimatedBpm = 60f / avgInterval;

            OnBeatDetectedEvent?.Invoke();
        }

        _lastHitTime = currentTime;
    }
}
