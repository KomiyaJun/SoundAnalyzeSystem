using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

public class UnitySoundManager : MonoBehaviour, ISoundManager
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private int sePoolSize = 20;
    [SerializeField] private AudioSource sePrefab;
    
    private List<AudioSource> _sePool = new List<AudioSource>();

    [Header("BGMSettings")]
    private AudioSource _activeBgmSource;
    private AudioSource _inactiveBgmSource;
    private Coroutine _fadeCoroutine;

    private void Awake()
    {
        //SoundServiceに既に登録されていた場合削除
        if(SoundService.Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        //サウンドサービスに自身を登録
        SoundService.Provide(this);

        //自身をDontDestroyOnLoadに変更
        DontDestroyOnLoad(gameObject);


        //SEの初期化
        for (int i = 0; i < sePoolSize; i++)
        {
            var sorce = Instantiate(sePrefab, transform);
            sorce.gameObject.SetActive(false);
            _sePool.Add(sorce);
        }

        //BGM用に２つのAudioSourceを生成
        _activeBgmSource = gameObject.AddComponent<AudioSource>();
        _inactiveBgmSource = gameObject.AddComponent<AudioSource>();

        //BGMのループを設定
        _activeBgmSource.loop = true;
        _inactiveBgmSource.loop = true;
    }

    //SEを再生
    public void PlaySE(SoundData data)
    {
        var source = GetAvailableSources();
        if (source == null) return;

        //SoundDataの中身を反映
        source.clip = data.clip;
        source.outputAudioMixerGroup = data.mixerGroup;

        source.volume = data.valume;
        source.pitch = data.useRandomPitch
            ? data.pitch + Random.Range(-data.pitchRandomRange, data.pitchRandomRange)
            : data.pitch;

        source.gameObject.SetActive(true);
        source.Play();
        StartCoroutine(ReturnToPool(source));
    }

    private AudioSource GetAvailableSources() => _sePool.Find(s => !s.gameObject.activeSelf);

    private System.Collections.IEnumerator ReturnToPool(AudioSource source)
    {
        yield return new WaitWhile(() => source.isPlaying);
        source.gameObject.SetActive(false);
    }

    public void PlayBGM(SoundData data, float fadeDuration)
    {
        //既に曲が再生中の場合は何もしない
        if (_activeBgmSource.clip == data.clip && _activeBgmSource.isPlaying) return;

        //現在実行中のフェードがあれば止める
        if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);

        //非アクティブなSourceに次の曲をセット
        _inactiveBgmSource.clip = data.clip;
        _inactiveBgmSource.outputAudioMixerGroup = data.mixerGroup;
        _inactiveBgmSource.volume = 0;
        _inactiveBgmSource.Play();

        //フェード開始
        _fadeCoroutine = StartCoroutine(CrossFadeRoutine(data.valume, fadeDuration));
    }
    private System.Collections.IEnumerator CrossFadeRoutine(float targetVolume, float duration)
    {
        //Time使用
        float startTime = Time.time;
        float startActiveVolume = _activeBgmSource.volume;

        while (Time.time < startTime + duration)
        {
            float t = (Time.time - startTime) / duration;

            //アクティブな曲をフェードイン
            _activeBgmSource.volume = Mathf.Lerp(startActiveVolume, 0, t);
            //次の曲をフェードイン
            _inactiveBgmSource.volume = Mathf.Lerp(0, targetVolume, t);
            yield return null;
        }

        //最終的な音量を確定
        _activeBgmSource.volume = 0;
        _activeBgmSource.Stop();
        _inactiveBgmSource.volume = targetVolume;

        //アクティブと非アクティブを入れ替える
        var temp = _activeBgmSource;
        _activeBgmSource = _inactiveBgmSource;
        _inactiveBgmSource = temp;

        _fadeCoroutine = null;

    }


    public void StopBGM(float fadeDuration)
    {
        if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
        _fadeCoroutine = StartCoroutine(FadeOutRoutine(fadeDuration));
    }

    private System.Collections.IEnumerator FadeOutRoutine(float duration)
    {
        float startVolume = _activeBgmSource.volume;
        float startTime = Time.time;

        while (Time.time < startTime + duration)
        {
            float t = (Time.time - startTime) / duration;
            _activeBgmSource.volume = Mathf.Lerp(startVolume, 0, t);
            yield return null;
        }

        _activeBgmSource.volume = 0;
        _activeBgmSource.Stop();
        _fadeCoroutine = null;
    }

    public void SetEnvironment(string snapshotName, float duration)
    {
        AudioMixerSnapshot snapshot = mixer.FindSnapshot(snapshotName);

        if(snapshot != null)
        {
            //指定した秒数をかけて遷移する
            snapshot.TransitionTo(duration);
            Debug.Log($"Enviroment changed to: {snapshotName} over {duration}s");
        }
        else
        {
            Debug.LogWarning($"Snapshot {snapshotName} not found in Mixer.");
        }
    }

}
