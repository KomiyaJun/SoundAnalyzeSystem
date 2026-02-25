using UnityEngine;

public class VisualizerInstance : MonoBehaviour
{
    public enum VisualizeType
    {
        Scale,
        Rotate
    }

    private VisualizeType visualizeType;
    private int minIndex;
    private int maxIndex;
    private float baseMultiple;
    private float highFreqBoost;
    private float lerpSpeed;
    private float minHeight;

    private float currentYValue = 0f;

    public void SetUp(VisualizeType type, int min, int max, float mult, float boost, float lerp, float minH)
    {
        visualizeType = type;
        minIndex = min; 
        maxIndex = max;
        baseMultiple = mult;
        highFreqBoost = boost;
        lerpSpeed = lerp;
        minHeight = minH;
    }

    void Update()
    {
        var analyzer = AudioAnalyzeService.Instance;
        if (analyzer == null) return;

        float rawVol = analyzer.GetBandAverage(minIndex, maxIndex);
        float freqPosition = (float)minIndex / 512f;
        float boost = 1f + (freqPosition * highFreqBoost);
        float targetVol = rawVol * baseMultiple * boost;

        if (targetVol > currentYValue)
            currentYValue = targetVol;
        else
            currentYValue = Mathf.Lerp(currentYValue, targetVol, Time.deltaTime * lerpSpeed);

        float finalY = Mathf.Max(minHeight, currentYValue);

        if (visualizeType == VisualizeType.Scale)
        {
            Vector3 s = transform.localScale;
            transform.localScale = new Vector3(s.x, finalY, s.z);
        }
        else
        {
            Vector3 r = transform.localEulerAngles;
            transform.localEulerAngles = new Vector3(r.x, r.y, finalY * 180f);
        }
    }
}
