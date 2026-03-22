using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class BpmSceneDebugger : MonoBehaviour
{
    [SerializeField] private BpmAnalyzer targetAnalyzer;

    [Header("グラフ設定")]
    [SerializeField] private float width = 10f;       // グラフの横幅
    [SerializeField] private float height = 5f;      // グラフの最大高さ
    [SerializeField] private int historySize = 100;  // 履歴の数
    [SerializeField] private float displayGain = 500f; // 可視化倍率

    private Queue<float> _history = new Queue<float>();

    private void Update()
    {
        if (targetAnalyzer == null) return;

        // 【修正】KickVolはpublicプロパティになったので直接取得可能
        float rawVol = targetAnalyzer.KickVol;

        _history.Enqueue(rawVol);
        if (_history.Count > historySize) _history.Dequeue();
    }

    private void OnDrawGizmos()
    {
        if (targetAnalyzer == null || _history.Count < 2) return;

        // 【修正】threshold は _settings（ScriptableObject）の中にある
        // まずプライベートな _settings を取得し、その中の値を参照する
        float threshold = 0.5f; // デフォルト値
        var settings = GetPrivateField<BpmAnalyzerSettings>(targetAnalyzer, "_settings");

        if (settings != null)
        {
            threshold = settings.threshold;
        }

        float releaseThreshold = threshold * 0.8f;
        Vector3 pos = transform.position;

        // 1. 枠線の描画
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(pos + new Vector3(width / 2, height / 2, 0), new Vector3(width, height, 0));

        // 2. 閾値（ON/OFF）のライン描画
        float thresholdY = Mathf.Clamp(threshold * displayGain, 0, height);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(pos + new Vector3(0, thresholdY, 0), pos + new Vector3(width, thresholdY, 0));

        float releaseY = Mathf.Clamp(releaseThreshold * displayGain, 0, height);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(pos + new Vector3(0, releaseY, 0), pos + new Vector3(width, releaseY, 0));

        // 3. 音量グラフの描画
        Gizmos.color = Color.green;
        float[] historyArray = _history.ToArray();
        float stepX = width / historySize;

        for (int i = 0; i < historyArray.Length - 1; i++)
        {
            Vector3 p1 = pos + new Vector3(i * stepX, Mathf.Clamp(historyArray[i] * displayGain, 0, height), 0);
            Vector3 p2 = pos + new Vector3((i + 1) * stepX, Mathf.Clamp(historyArray[i + 1] * displayGain, 0, height), 0);
            Gizmos.DrawLine(p1, p2);
        }

#if UNITY_EDITOR
        Handles.Label(pos + new Vector3(0, height + 0.5f, 0), "BPM Analysis Debugger");
        Handles.Label(pos + new Vector3(width + 0.2f, thresholdY, 0), $"ON: {threshold}");
        Handles.Label(pos + new Vector3(width + 0.2f, releaseY, 0), $"OFF: {releaseThreshold:F4}");
#endif
    }

    // 変数（Field）を取得するためのヘルパー
    private T GetPrivateField<T>(object obj, string fieldName)
    {
        // クラスだけでなく、親クラスまで遡ってフィールドを探す設定
        var field = obj.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field == null) return default;
        return (T)field.GetValue(obj);
    }
}