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

    private enum VisualizeType
    {
        Scale,
        Rotate,
    }
    [SerializeField] private VisualizeType visualizeType = VisualizeType.Scale;


    private float currentYValue = 0f;
    void Update()
    {
        var analyzer = AudioAnalyzeService.Instance;

        float rawVol = analyzer.GetBandAverage(minIndex, maxIndex);

        float freqPosition = (float)minIndex / 512f;
        float boost = 1f + (freqPosition * highFreqBoost);
        float targetVol = rawVol * baseMultiple * boost;

        if(targetVol > currentYValue)
        {
            currentYValue = targetVol;
        }
        else
        {
            currentYValue = Mathf.Lerp(currentYValue, targetVol, Time.deltaTime * lerpSpeed);   //Timeégóp
        }

        float finalY = Mathf.Max(minHeight, currentYValue);

        switch (visualizeType)
        {
            case VisualizeType.Scale:
                Vector3 originalScale = transform.localScale;
                transform.localScale = new Vector3(originalScale.x, finalY, originalScale.z);
                break;
            case VisualizeType.Rotate:
                Vector3 originalRotate = transform.localEulerAngles;

                float targetAngles = finalY * 180f;

                transform.localEulerAngles = new Vector3(originalRotate.x, originalRotate.y, targetAngles);
                break;

            default:

                break;
        }

    }
}
