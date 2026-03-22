using UnityEngine;

[CreateAssetMenu(fileName = "BpmAnalyzerSettings", menuName = "Sound/BpmAnalyzerSettings")]
public class BpmAnalyzerSettings : ScriptableObject
{
    public float threshold = 0.5f;
    public float baseCoolDownTime = 0f;
    public int fftParamMin = 0;
    public int fftParamMax = 2;
    public int highFreqMin = 10;
    public int highFreqMax = 20;
    public float kickBias = 1.5f;
}
