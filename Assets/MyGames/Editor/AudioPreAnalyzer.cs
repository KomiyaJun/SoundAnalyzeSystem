using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AudioPreAnalyzer : EditorWindow
{
    private AudioClip targetClip;
    private BpmAnalyzerSettings settings;

    [MenuItem("Tools/Audio Beat Pre-Analyzer")]
    public static void ShowWindow() => GetWindow<AudioPreAnalyzer>("Beat Analyzer");

    private void OnGUI()
    {
        targetClip = (AudioClip)EditorGUILayout.ObjectField("Target Clip", targetClip, typeof(AudioClip), false);
        settings = (BpmAnalyzerSettings)EditorGUILayout.ObjectField("Settings", settings, typeof(BpmAnalyzerSettings), false);

        if (GUILayout.Button("Analyze Beat") && targetClip != null && settings != null)
        {
            RunAnalysis();
        }
    }

    private void RunAnalysis()
    {
        GameObject go = new GameObject("TempAnalyzer");
        var analyzer = go.AddComponent<BpmAnalyzer>();

        var field = typeof(BpmAnalyzer).GetField("_settings", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field.SetValue(analyzer, settings);

        // 1. 全サンプル取得
        int channels = targetClip.channels;
        float[] rawSamples = new float[targetClip.samples * channels];
        targetClip.GetData(rawSamples, 0);

        // 2. モノラル化（ステレオなら平均化）
        float[] monoSamples = new float[targetClip.samples];
        if (channels > 1)
        {
            for (int i = 0; i < targetClip.samples; i++)
            {
                float sum = 0;
                for (int c = 0; c < channels; c++) sum += rawSamples[i * channels + c];
                monoSamples[i] = sum / channels;
            }
        }
        else
        {
            monoSamples = rawSamples;
        }

        BeatData resultData = ScriptableObject.CreateInstance<BeatData>();
        resultData.audioClipName = targetClip.name;

        int fftSize = 1024;
        float stepSeconds = 0.01f;
        int stepSamples = (int)(targetClip.frequency * stepSeconds);

        // 3. スキャン実行
        for (int i = 0; i < monoSamples.Length - fftSize; i += stepSamples)
        {
            float[] window = new float[fftSize];
            Array.Copy(monoSamples, i, window, 0, fftSize);

            float[] spectrum = FFTHelper.GetSpectrum(window);

            float currentTime = (float)i / targetClip.frequency;

            Debug.Log($"Time: {currentTime:F2} / KickVol: {analyzer.KickVol}");

            // CheckBeat でビートを判定
            if (analyzer.CheckBeat(spectrum, currentTime, 1.0f))
            {
                resultData.beatTimestamps.Add(currentTime);

                // 【重要】検出したことをアナライザーに記録させる（_lastHitTimeを更新）
                // エディタ解析中はイベントを飛ばさないように false を渡す
                analyzer.ProcessBeat(currentTime, false);
            }
        }
        string path = $"Assets/{targetClip.name}_BeatData.asset";
        AssetDatabase.CreateAsset(resultData, path);
        AssetDatabase.SaveAssets();

        DestroyImmediate(go);
        EditorUtility.DisplayDialog("Analysis Complete", $"Found {resultData.beatTimestamps.Count} beats.\nSaved to {path}", "OK");
    }
}