using UnityEngine;

public class ScrollingSpectrogram : MonoBehaviour
{
    [Header("ê›íË")]
    [SerializeField] private int histroyWidth = 512;
    [SerializeField] private Gradient colorGradient;
    [SerializeField] private float intensityMultiplier = 10f;

    [SerializeField, Range(2f, 100f)] private float logScaleBase = 10f; 
    private Texture2D _spectrogramTexture;
    private Color[] _columnColors;
    private Renderer _renderer;
    private IAudioAnalyzer _analyzer;

    private int _currentColumn = 0;
    void Start()
    {
        _analyzer = AudioAnalyzeService.Instance;
        _renderer = GetComponent<Renderer>();

        int fftSize = 1024;
        _spectrogramTexture = new Texture2D(histroyWidth, fftSize, TextureFormat.ARGB32, false);
        _spectrogramTexture.filterMode = FilterMode.Bilinear;
        _spectrogramTexture.wrapMode = TextureWrapMode.Repeat;

        Color[] resetColors = new Color[histroyWidth * fftSize];
        _spectrogramTexture.SetPixels(resetColors);
        _spectrogramTexture.Apply();

        _renderer.material.mainTexture = _spectrogramTexture;

        _columnColors = new Color[fftSize];
    }

    void Update()
    {
        if (_analyzer == null) return;

        float[] spectrum = _analyzer.GetRawSpectrumData();
        if (spectrum == null || spectrum.Length == 0) return;

        int texHeight = _columnColors.Length;
        for(int y = 0; y < spectrum.Length; y++)
        {
            float t = (float)y / (texHeight - 1);

            float curve = (Mathf.Pow(logScaleBase, t) - 1f) / (logScaleBase - 1f);

            int index = Mathf.Clamp(Mathf.FloorToInt(curve * spectrum.Length), 0, spectrum.Length - 1);

            float val = Mathf.Clamp01(spectrum[index] * intensityMultiplier);
            _columnColors[y] = colorGradient.Evaluate(val);
        }

        _spectrogramTexture.SetPixels(_currentColumn, 0, 1, _spectrogramTexture.height, _columnColors);

        _spectrogramTexture.Apply();

        float offset = (float)(_currentColumn + 1) / histroyWidth;
        _renderer.material.mainTextureOffset = new Vector2(offset, 0);

        _currentColumn = (_currentColumn + 1) % histroyWidth;
    }
}
