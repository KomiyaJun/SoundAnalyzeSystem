using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BeatGlow2D : MonoBehaviour
{
    [Header("Color Settings")]
    [SerializeField] private Color flashColor = Color.white; // ビート時の色
    [SerializeField] private float fadeSpeed = 5f;          // 元に戻る速さ

    private SpriteRenderer _spriteRenderer;
    private Color _baseColor;
    private Color _targetColor;

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _baseColor = _spriteRenderer.color; // 初期の色を保存
        _targetColor = _baseColor;
    }

    // ★BpmAnalyzerのEventからこれを呼ぶ
    public void OnBeat()
    {
        // 瞬間的に色をフラッシュカラーにする
        _spriteRenderer.color = flashColor;
    }

    void Update()
    {
        // フレームごとに現在の色をベースカラーへ近づける（フェードアウト）
        _spriteRenderer.color = Color.Lerp(_spriteRenderer.color, _baseColor, Time.deltaTime * fadeSpeed);
    }
}