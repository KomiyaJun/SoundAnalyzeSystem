using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class BpmAnalyzer : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private float thereshold = 0.5f;
    [SerializeField] private float coolDownTime = 0.25f;

    private float _lastHitTime;
    private List<float> _intervals = new List<float>();
    private const int MaxIntervals = 8;

    public UnityEvent OnBeatDetectedEvent;

    public float EstimatedBpm { get; private set; }

    private void Update()
    {
        var analyzer = AudioAnalyzeService.Instance;
        if(analyzer == null ) return;

        float kickVol = analyzer.GetBandAverage(0, 2);

        if (kickVol > thereshold && Time.time > _lastHitTime + coolDownTime)
        {
            OnBeatDetected();
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
            Debug.Log($"Beat! Estimated BPM : {EstimatedBpm:F1}");
        }

        _lastHitTime = currentTime;
    }
}
