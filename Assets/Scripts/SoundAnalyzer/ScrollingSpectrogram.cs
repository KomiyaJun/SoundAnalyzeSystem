using UnityEngine;

public class ScrollingSpectrogram : MonoBehaviour
{
    [Header("ê›íË")]
    [SerializeField] private int histroyWidth = 512;
    [SerializeField] private Gradient colorGradient;
    [SerializeField] private float intensityMultiplier = 10f;

    private Texture2D _spectrogramTexture;
    private Color[] _columnColors;
    private SpriteRenderer _spriteRenderer;
    private IAudioAnalyzer _analyzer;

    void Start()
    {
        _analyzer = AudioAnalyzeService.Instance;
        _spriteRenderer = GetComponent<SpriteRenderer>();

        int fftSize = 1024;
        _spectrogramTexture = new Texture2D(histroyWidth, fftSize, TextureFormat.ARGB32, false);
        _spectrogramTexture.filterMode = FilterMode.Bilinear;
        _spectrogramTexture.wrapMode = TextureWrapMode.Clamp;

        Color[] resetColors = new Color[histroyWidth * fftSize];
        _spectrogramTexture.SetPixels(resetColors);
        _spectrogramTexture.Apply();

        _spriteRenderer.sprite = Sprite.Create(_spectrogramTexture, new Rect(0, 0, histroyWidth, fftSize), Vector2.one * 0.5f);

        _columnColors = new Color[fftSize];
    }

    void Update()
    {
        if (_analyzer == null) return;

        float[] spectrum = _analyzer.GetRawSpectrumData();
        if (spectrum == null || spectrum.Length == 0) return;

        Color[] pixels = _spectrogramTexture.GetPixels(1, 0,histroyWidth - 1, _spectrogramTexture.height);
        _spectrogramTexture.SetPixels(0, 0, histroyWidth - 1, _spectrogramTexture.height, pixels);

        for(int i = 0; i < spectrum.Length; i++)
        {
            float val = Mathf.Clamp01(spectrum[i] * intensityMultiplier);
            _columnColors[i] = colorGradient.Evaluate(val);
        }

        _spectrogramTexture.SetPixels(histroyWidth -1, 0, 1, _spectrogramTexture.height, _columnColors);

        _spectrogramTexture.Apply();

    }
}
