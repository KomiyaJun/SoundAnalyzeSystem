using UnityEngine;

public class Visualizer : MonoBehaviour
{
    [SerializeField] int minIndex = 0;
    [SerializeField] int maxIndex = 512;

    private float multiple = 100;
    void Update()
    {
        var analyzer = AudioAnalyzeService.Instance;

        if(analyzer != null)
        {
            float vol = analyzer.GetBandAverage(minIndex, maxIndex);
            
            Vector3 originScale = gameObject.transform.localScale;

            gameObject.transform.localScale = new Vector3(originScale.x, vol * 10000, originScale.z);
        }
    }
}
