using UnityEngine;

public interface IAudioAnalyzer
{
    //指定帯域の音量を取得
    float GetBandAverage(int minIndex, int maxIndex);

    //生データを取得
    float[] GetRawSpectrumData();
    float[] SpectrumData { get; }
    void SetPreset(AudioAnalyzerPreset preset);
}
