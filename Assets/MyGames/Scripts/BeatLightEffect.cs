using UnityEngine;
using System.Collections.Generic;
using MyGame.AudioSetting;

[RequireComponent(typeof(Renderer))]
public class BeatLightEffect : MonoBehaviour
{
    [Header("ターゲット")]
    [SerializeField] private BgmPartType part;

    [Header("参照")]
    [SerializeField] private BeatData _beatData;

    [Header("エフェクト設定")]
    [SerializeField] private Color _flashColor = Color.cyan; // 光る時の色
    [SerializeField] private float _fadeSpeed = 5f;          // 元の色に戻るスピード

    private AudioSource _audioSource;
    private Material _material;
    private Color _baseColor;
    private int _nextBeatIndex = 0;

    private void Start()
    {
        // オブジェクトのマテリアルと元の色を取得
        _material = GetComponent<Renderer>().material;
        _baseColor = _material.color;
    }

    private void Update()
    {
        // 1. 常に元の色に向かって滑らかに戻していく（フェードアウト）
        _material.color = Color.Lerp(_material.color, _baseColor, Time.deltaTime * _fadeSpeed);

        _audioSource = SoundService.Instance.GetLayerSource(part);

        // 再生されていない、データがない、または最後まで読み終わった場合は何もしない
        if (_beatData == null || _audioSource == null || !_audioSource.isPlaying) return;
        if (_nextBeatIndex >= _beatData.beatTimestamps.Count) return;

        // 2. 現在のBGMの再生位置（秒）を取得
        float currentTime = _audioSource.time;

        // 3. 次のビートの時間が来たら光らせて、インデックスを進める
        if (currentTime >= _beatData.beatTimestamps[_nextBeatIndex])
        {
            Flash();
            _nextBeatIndex++;
        }
    }

    private void Flash()
    {
        // 色を一瞬で flashColor に変える
        _material.color = _flashColor;

        // （おまけ）少し上に跳ねるような動きを入れる場合
        transform.localScale = Vector3.one * 1.5f;
    }
}