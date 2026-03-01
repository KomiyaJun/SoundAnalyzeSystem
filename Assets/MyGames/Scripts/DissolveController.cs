using System.Collections;
using UnityEngine;

public class DissolveController : MonoBehaviour
{
    private Material targetMaterial;

    private float currentAmount = 0f;

    [Header("Settings")]
    public float duration = 0.3f; // 変化にかかる時間（秒）

    void Awake()
    {
        targetMaterial = GetComponent<Renderer>().material;

        currentAmount = 0f;
        ApplyToShader(currentAmount);
    }

    [ContextMenu("Appear (現れる)")]
    public void Appear()
    {
        StopAllCoroutines();
        // 現在の値(currentAmount)から 0(表示) に向かってアニメーション
        StartCoroutine(AnimateDissolve(0f));
    }

    [ContextMenu("Dissolve (消える)")]
    public void Dissolve()
    {
        StopAllCoroutines();
        // 現在の値(currentAmount)から 1(消失) に向かってアニメーション
        StartCoroutine(AnimateDissolve(1f));
    }

    [ContextMenu("Stop (止める)")]
    public void StopEffect()
    {
        StopAllCoroutines();
    }

    // --- 内部処理 ---

    private IEnumerator AnimateDissolve(float targetValue)
    {
        float startValue = currentAmount;
        float time = 0f;

        // すでに目標値なら何もしない
        if (Mathf.Abs(startValue - targetValue) < 0.01f) yield break;

        while (time < duration)
        {
            //Time使用
            time += Time.deltaTime;
            currentAmount = Mathf.Lerp(startValue, targetValue, time / duration);

            ApplyToShader(currentAmount);
            yield return null;
        }

        currentAmount = targetValue;
        ApplyToShader(currentAmount);
    }

    private void ApplyToShader(float val)
    {
        targetMaterial.SetFloat("_DissolveAmount", val);
    }
}