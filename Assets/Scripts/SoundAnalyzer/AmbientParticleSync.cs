using UnityEngine;

public class AmbientParticleSync : MonoBehaviour
{
    [Header("低音")]
    [SerializeField] private ParticleSystem ambientParticle;
    [SerializeField] private float minStartSize = 0.05f;
    [SerializeField] private float maxStartSize = 0.5f;

    [Header("高音")]
    [SerializeField] private ParticleSystem sparkParticles;
    [SerializeField] private float sparkThreshold = 0.05f;
    [SerializeField] private int sparkBurstCount = 50;

    [Header("解析設定")]
    [SerializeField] private int lowFreqMax = 10;
    [SerializeField] private int highFreqMin = 200;
    [SerializeField] private float smoothSpeed = 5f;

    private float _smoothedLow;
    private float _currentHighVol;
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

        //低音の処理
        float lowVal = _analyzer.GetBandAverage(0, lowFreqMax);
        _smoothedLow = Mathf.Lerp(_smoothedLow, lowVal, Time.deltaTime * smoothSpeed);

        if(ambientParticle != null)
        {
            var main = ambientParticle.main;

            main.startSize = Mathf.Lerp(minStartSize, maxStartSize, _smoothedLow * 10f);

            var emission = ambientParticle.emission;
            emission.rateOverTime = 10 + (_smoothedLow * 100);
        }

        //高音の処理
        float[] data = _analyzer.GetRawSpectrumData();
        if (data == null || data.Length == 0) return;

        float highVol = _analyzer.GetBandAverage(highFreqMin, data.Length - 1);
        _currentHighVol = highVol;
        if (highVol > sparkThreshold && sparkParticles!= null)
        {
            int emitCount = Mathf.CeilToInt(highVol * sparkBurstCount * 1000f);
            sparkParticles.Emit(Mathf.FloorToInt(emitCount));
        }
    }

    private void OnDrawGizmosSelected()
    {

        Gizmos.color = new Color(0, 1, 1, 0.5f); // 半透明の水色
        float sphereSize = Mathf.Lerp(minStartSize, maxStartSize, _smoothedLow * 10f);
        Gizmos.DrawWireSphere(transform.position, sphereSize);


        Vector3 barBasePos = transform.position + Vector3.right * 2.0f;
        float visualScale = 2000f; // 値が小さいので大きく拡大して表示

        Gizmos.color = Color.red;
        float thresholdHeight = sparkThreshold * visualScale;
        Gizmos.DrawLine(barBasePos - Vector3.right * 0.5f + Vector3.up * thresholdHeight,
                        barBasePos + Vector3.right * 0.5f + Vector3.up * thresholdHeight);

        // 現在の音量バー
        Gizmos.color = _currentHighVol > sparkThreshold ? Color.yellow : Color.gray;
        float currentHeight = _currentHighVol * visualScale;

        Gizmos.DrawCube(barBasePos + Vector3.up * (currentHeight / 2), new Vector3(0.5f, currentHeight, 0.5f));
    }
}
