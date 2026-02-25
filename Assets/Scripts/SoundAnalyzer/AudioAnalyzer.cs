using MyGame.AudioSetting;
using UnityEngine;
using System.Collections.Generic;
using System;

public class AudioAnalyzer : MonoBehaviour , IAudioAnalyzer
{
    [Header("設定")]
    [SerializeField] private List<BgmPartType> targetPart = new List<BgmPartType>();    //対象の楽器
    [SerializeField] private int sampleCount = 512;     //FFTの解像度
    [SerializeField] private FFTWindow windowType = FFTWindow.BlackmanHarris; //窓関数
    
    private AudioSource _targetSource;
    private float[] _spectrumData;
    private float[] _tempBuffer;

    /// <summary>
    /// 外部から生のデータを参照する用
    /// </summary>
    public float[] SpectrumData => _spectrumData;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        _spectrumData = new float[sampleCount];
        _tempBuffer = new float[sampleCount];

        AudioAnalyzeService.Provide(this);
    }

    private void OnDestroy()
    {
        if ((object)AudioAnalyzeService.Instance == this)
        {
            AudioAnalyzeService.Clear();
        }
    }

    // Update is called once per frame
    void Update()
    {
        Array.Clear(_spectrumData, 0, _spectrumData.Length);
        if (SoundService.Instance == null) return;

        foreach(var part in targetPart)
        {
            AudioSource source = SoundService.Instance.GetLayerSource(part);

            if(source != null && source.isPlaying)
            {
                source.GetSpectrumData(_spectrumData, 0, windowType);

                for(int i = 0; i < sampleCount; i++)
                {
                    _spectrumData[i] += _tempBuffer[i];
                }
            }
        }

    }

    public float GetBandAverage(int minIndex, int maxIndex)
    {
        if (_spectrumData == null || _spectrumData.Length == 0) return 0f;

        minIndex = Mathf.Clamp(minIndex, 0, _spectrumData.Length -1);
        maxIndex = Mathf.Clamp(maxIndex, 0, _spectrumData.Length -1);

        if (minIndex > maxIndex) return 0f;

        float sum = 0f;
        int count = 0;

        for(int i = minIndex; i <= maxIndex; i++)
        {
            sum += _spectrumData[i];
            count++;
        }

        return (count > 0) ? sum / count : 0f;
    }

    public float[] GetRawSpectrumData()
    {
        return _spectrumData;
    }
}
