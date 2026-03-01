using System.Collections.Generic;
using UnityEngine;

public class WaterChain : MonoBehaviour
{
    [Header("波のプレハブ")]
    [SerializeField] private GameObject wavePrefab;

    [Header("波の数")]
    [SerializeField] private int waveCount = 30; // 画面内に表示する波の数

    [Header("波の出現エリア（ワールド座標Y）")]
    // ここが重要：カメラの位置に関係なく、この高さの範囲に波が出ます
    [SerializeField] private float minWorldY = -4.5f; // 海の底の高さ
    [SerializeField] private float maxWorldY = -2.0f; // 海面の高さ

    [Header("動きの設定")]
    [SerializeField] private float moveSpeed = 3.0f;    // 左へ流れる速さ
    [SerializeField] private float bobFrequency = 2.0f; // ゆらゆらする速さ
    [SerializeField] private float bobAmplitude = 0.2f; // ゆらゆらする幅

    // 内部データ
    private class WaveData
    {
        public Transform transform;
        public float baseWorldY; // その波が本来あるべき基準の高さ(Y)
        public float bobOffset;  // 個別のタイミングのズレ
    }

    private List<WaveData> waves = new List<WaveData>();
    private Transform camTransform;
    private float screenWidthWorld;

    void Start()
    {
        camTransform = Camera.main.transform;

        // 画面の横幅を計算（X軸のループ判定に使用）
        float screenHeight = Camera.main.orthographicSize * 2;
        screenWidthWorld = screenHeight * Camera.main.aspect;

        // 指定数の波を生成
        for (int i = 0; i < waveCount; i++)
        {
            GameObject obj = Instantiate(wavePrefab, transform);

            WaveData data = new WaveData();
            data.transform = obj.transform;

            // 最初はカメラ周辺にランダムに配置
            // Y座標は「ワールド座標設定」の中からランダムに決める
            float randomX = Random.Range(-screenWidthWorld, screenWidthWorld);
            InitializeWave(data, camTransform.position.x + randomX);

            waves.Add(data);
        }
    }

    void Update()
    {
        float camX = camTransform.position.x;

        // リサイクル判定ライン（画面外に出たかどうか）
        float deleteLine = camX - (screenWidthWorld / 2) - 3.0f; // 左端
        float spawnLine = camX + (screenWidthWorld / 2) + 3.0f;  // 右端

        foreach (var wave in waves)
        {
            Vector3 pos = wave.transform.position;

            // --- 1. 左へ移動 ---
            pos.x -= moveSpeed * Time.deltaTime;

            // --- 2. ノイズ揺れ（上下） ---
            // 基準の高さ(baseWorldY) を中心に揺らす
            float bob = Mathf.Sin(Time.time * bobFrequency + wave.bobOffset) * bobAmplitude;
            pos.y = wave.baseWorldY + bob;

            // --- 3. ループ処理 ---
            // 画面の左に消えたら、画面の右に再配置
            if (pos.x < deleteLine)
            {
                // Xは右端へ、Yは再びワールド座標の範囲内でランダムに再抽選
                InitializeWave(wave, spawnLine);
                pos.x = wave.transform.position.x;
                pos.y = wave.baseWorldY; // リセット直後の高さ
            }

            wave.transform.position = pos;
        }
    }

    // 波を初期化・再配置する関数
    void InitializeWave(WaveData wave, float startX)
    {
        // X座標：指定位置 + 少しランダム（ばらつき用）
        float randomX = startX + Random.Range(0f, 4.0f);

        // Y座標：【ここがポイント】指定されたワールド座標の範囲内でランダムに決定
        float randomY = Random.Range(minWorldY, maxWorldY);

        // 揺れのタイミング：ランダム
        float randomOffset = Random.Range(0f, 100f);

        // データを更新
        wave.baseWorldY = randomY;
        wave.bobOffset = randomOffset;

        // 位置を適用（Zは0固定、または必要なら変えてください）
        wave.transform.position = new Vector3(randomX, randomY, 0);
    }
}