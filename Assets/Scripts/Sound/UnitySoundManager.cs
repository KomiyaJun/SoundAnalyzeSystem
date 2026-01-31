using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

public class UnitySoundManager : MonoBehaviour, ISoundManager
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private int sePoolSize = 20;
    [SerializeField] private AudioSource sePrefab;
    
    private List<AudioSource> _layerSources = new List<AudioSource>();
    private List<AudioSource> _sePool = new List<AudioSource>();

    [Header("BGMSettings")]
    private AudioSource _activeBgmSource;
    private AudioSource _inactiveBgmSource;
    private Coroutine _fadeCoroutine;


    private const string KeyMasterVolume = "MasterVolume";
    private const string KeyBGMVolume = "BGMVolume";
    private const string KeySEVolume = "SEVolume";
    private const string KeyUIVolume = "UIVolume";

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

    private void Start()
    {
        LoadVolumes();
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

    public void SetVolume(VolumeType type, float volume)
    {
        //0を入れると計算不可能になってエラーが出るので、極小値を入れる
        float clampedValume = Mathf.Clamp(volume, 0.0001f, 1f);

        //デシベル変換
        float decibal = 20f * Mathf.Log10(clampedValume);

        string paramName = "";
        string saveKey = "";

        //enumに応じたパラメータ名を指定
        switch(type)
        {
            case VolumeType.Master: paramName = KeyMasterVolume; saveKey = KeyMasterVolume; break;
            case VolumeType.BGM: paramName = KeyBGMVolume; saveKey = KeyBGMVolume; break;
            case VolumeType.SE: paramName = KeySEVolume; saveKey = KeySEVolume; break;
            case VolumeType.UI: paramName = KeyUIVolume; saveKey = KeyUIVolume; break;


        };

        mixer.SetFloat(paramName, decibal);

        //値を保存
        PlayerPrefs.SetFloat(saveKey, volume);
        PlayerPrefs.Save();
    }

    //保存された音量設定をロードする
    private void LoadVolumes()
    {
        SetVolume(VolumeType.Master, PlayerPrefs.GetFloat(KeyMasterVolume, 1.0f));
        SetVolume(VolumeType.BGM, PlayerPrefs.GetFloat(KeyBGMVolume, 1.0f));
        SetVolume(VolumeType.SE, PlayerPrefs.GetFloat(KeySEVolume, 1.0f));
        SetVolume(VolumeType.UI, PlayerPrefs.GetFloat(KeyUIVolume, 1.0f));
    }

    
    public void PlayLayeredBGM(LayeredSoundData data)
    {
        //既存のBGMを停止
        StopBGM(data.fadeDuration);

        //必要な数だけAudioSourceを用意し、足りなければ追加する
        PrepareLayerSources(data.layers.Length);

        //オーディオエンジンの現在の精密の時間を取得する
        double startTime = AudioSettings.dspTime + 0.1; //0.1秒後に一斉に予約をする

        for(int i = 0; i < data.layers.Length; i++)
        {
            _layerSources[i].clip = data.layers[i];
            _layerSources[i].loop = true;
            _layerSources[i].volume = (i == 0) ? 1f : 0f; //最初のパート(ドラム等)以外は0で開始

            //精密予約再生
            _layerSources[i].PlayScheduled(startTime);
        }
    }

    private void PrepareLayerSources(int count)
    {
        while(_layerSources.Count < count)
        {
            var source = gameObject.AddComponent<AudioSource>();
            source.outputAudioMixerGroup = mixer.FindMatchingGroups("BGM")[0];
            _layerSources.Add(source);
        }
    }

    public void SetLayerVolume(int index, float volume, float duration)
    {
        if (index >= _layerSources.Count) return;

        //音量を滑らかに変更
        StartCoroutine(FadeLayerVolume(_layerSources[index], volume, duration));
    }

    public System.Collections.IEnumerator FadeLayerVolume(AudioSource source, float target, float duration)
    {
        float startVol = source.volume;
        float time = 0;
        while(time < duration)
        {
            //Time使用
            time += Time.deltaTime;
            source.volume = Mathf.Clamp(startVol, target, time / duration);
            yield return null;
        }
        source.volume = target;
    }

    public void SetAllLayersVolume(float volume, float duration)
    {
        for(int i = 0; i < _layerSources.Count; i++)
        {
            SetLayerVolume(i, volume, duration);
        }
    }

    public void ApplyPreset(BgmPreset preset, float duration = 0.5f)
    {
        for(int i = 0; i < preset.layerVolumes.Length; i++)
        {
            //レイヤーが存在する場合のみ音量を変更
            if(i < _layerSources.Count)
            {
                SetLayerVolume(i,preset.layerVolumes[i], duration);
            }
        }
    }
}
