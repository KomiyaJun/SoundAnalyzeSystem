using UnityEngine;

public class DissolveController : MonoBehaviour
{
    private Material targetMaterial;
    private bool isAnimating = false;
    private float currentProgress = 0f; // 0 = 出現完了, 1 = 消失完了

    [Header("Settings")]
    public float speed = 1.0f;

    void Awake()
    {
        targetMaterial = GetComponent<Renderer>().material;
        SetProgress(0f);
    }

    void Update()
    {
        if (!isAnimating) return;

    }


    [ContextMenu("Start Dissolve")]

    [ContextMenu("Appear (現れる)")]
    public void Appear()
    {
        StopAllCoroutines();
        StartCoroutine(AnimateDissolve(false));
    }

    [ContextMenu("Dissolve (消える)")]
    public void Dissolve()
    {
        StopAllCoroutines();
        StartCoroutine(AnimateDissolve(true));
    }

    [ContextMenu("Stop (止める)")]
    public void StopEffect()
    {
        StopAllCoroutines();
    }

    // --- 内部処理 ---

    private System.Collections.IEnumerator AnimateDissolve(bool dissolving)
    {
        float startValue = dissolving ? 0f : 1f;
        float endValue = dissolving ? 1f : 0f;
        float elapsed = 0f;

        // 現在の地点から再開（スムーズに反転させるため）
        float duration = 1.0f / speed;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float progress = Mathf.Lerp(startValue, endValue, t);


            ApplyToShader(progress);
            yield return null;
        }
    }

    private void ApplyToShader(float progress)
    {
        float fakeTime = progress * 80f;
        targetMaterial.SetFloat("_StartTime", Time.time - fakeTime);
        targetMaterial.SetFloat("_Speed", 1.0f); // 速度はスクリプト側で計算済み
    }

    private void SetProgress(float p) => ApplyToShader(p);
}
