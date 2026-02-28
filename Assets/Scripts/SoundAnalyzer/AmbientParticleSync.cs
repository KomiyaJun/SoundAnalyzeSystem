using UnityEngine;

public class AmbientParticleSync : MonoBehaviour
{
    [Header("’á‰¹")]
    [SerializeField] private ParticleSystem ambientParticle;
    [SerializeField] private float minStartSize = 0.05f;
    [SerializeField] private float maxStartSize = 0.5f;

    [Header("‚‰¹")]
    [SerializeField] private ParticleSystem sparkParticles;
    [SerializeField] private float sparkThreshold = 0.05f;
    [SerializeField] private int sparkBurstCount = 50;

    [Header("‰ðÍÝ’è")]
    [SerializeField] private int lowFreqMax = 10;
    [SerializeField] private int highFreqMin = 200;
    [SerializeField] private float smoothSpeed = 5f;

    private float _smoothedLow;
    private IAudioAnalyzer _analyzer;
    


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _analyzer = AudioAnalyzeService.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (_analyzer == null) return;

        //’á‰¹‚Ìˆ—
        float lowVal = _analyzer.GetBandAverage(0, lowFreqMax);
        _smoothedLow = Mathf.Lerp(_smoothedLow, lowVal, Time.deltaTime * smoothSpeed);

        if(ambientParticle != null)
        {
            var main = ambientParticle.main;

            main.startSize = Mathf.Lerp(minStartSize, maxStartSize, _smoothedLow * 10f);

            var emission = ambientParticle.emission;
            emission.rateOverTime = 10 + (_smoothedLow * 100);
        }

        //‚‰¹‚Ìˆ—
        float[] data = _analyzer.GetRawSpectrumData();
        if (data == null || data.Length == 0) return;

        float highVol = _analyzer.GetBandAverage(highFreqMin, data.Length - 1);

        if(highVol > sparkThreshold && sparkParticles!= null)
        {
            int emitCount = Mathf.CeilToInt(highVol * sparkBurstCount * 1000f);
            sparkParticles.Emit(Mathf.FloorToInt(emitCount));
        }
    }
}
