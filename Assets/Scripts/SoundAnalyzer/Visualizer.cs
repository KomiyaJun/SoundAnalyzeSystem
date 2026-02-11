using UnityEngine;

public class Visualizer : MonoBehaviour
{
    [SerializeField] int minIndex = 0;
    [SerializeField] int maxIndex = 64;

    [Header("Sensitivity")]
    [SerializeField] private float baseMultiple = 1000;
    [SerializeField] private float highFreqBoost = 10f;

    [Header("VisualSmooth")]
    [SerializeField] private float lerpSpeed = 15f;
    [SerializeField] private float minHeight = 0.2f;

    private float currentYScale = 0f;
    void Update()
    {
        var analyzer = AudioAnalyzeService.Instance;

        float rawVol = analyzer.GetBandAverage(minIndex, maxIndex);

        float freqPosition = (float)minIndex / 512f;
        float boost = 1f + (freqPosition * highFreqBoost);
        float targetVol = rawVol * baseMultiple * boost;

        if(targetVol > currentYScale)
        {
            currentYScale = targetVol;
        }
        else
        {
            currentYScale = Mathf.Lerp(currentYScale, targetVol, Time.deltaTime * lerpSpeed);   //Timeégóp
        }

        float finalY = Mathf.Max(minHeight, currentYScale);
        Vector3 originalScale = transform.localScale;
        transform.localScale = new Vector3(originalScale.x, finalY, originalScale.z);

    }
}
