using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

namespace MyGame.AudioSetting
{
    public class UnitySoundManager : MonoBehaviour, ISoundManager
    {
        [SerializeField] private AudioMixer mixer;
        [SerializeField] private int sePoolSize = 20;
        [SerializeField] private AudioSource sePrefab;
        [SerializeField] private AudioSource _ambientSource;


        private List<AudioSource> _layerSources = new List<AudioSource>();
        private List<AudioSource> _sePool = new List<AudioSource>();

        private Dictionary<AudioSource, Coroutine> _activeLayerFades = new Dictionary<AudioSource, Coroutine>();

        [Header("BGMSettings")]
        private AudioSource _activeBgmSource;
        private AudioSource _inactiveBgmSource;
        private Coroutine _fadeCoroutine;

        [Header("MixerNameSettings")]
        [SerializeField] private const string KeyMasterVolume = "MasterVolume";
        [SerializeField] private const string KeyBGMVolume = "BGMVolume";
        [SerializeField] private const string KeySEVolume = "SEVolume";
        [SerializeField] private const string KeyUIVolume = "UIVolume";
        [SerializeField] private const string KeyAmbientVolume = "AmbientVolume";


        private void Awake()
        {
            //SoundServiceに既に登録されていた場合削除
            if (SoundService.Instance != null)
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

            source.volume = data.volume;
            source.pitch = data.useRandomPitch
                ? data.pitch + Random.Range(-data.pitchRandomRange, data.pitchRandomRange)
                : data.pitch;

            source.gameObject.SetActive(true);
            source.Play();
            StartCoroutine(ReturnToPool(source));
        }

        //BGMを再生
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
            _fadeCoroutine = StartCoroutine(CrossFadeRoutine(data.volume, fadeDuration));
        }

        //レイヤーでBGMを再生
        public void PlayLayeredBGM(LayeredSoundData data)
        {
            //既存のBGMを停止
            StopBGM(data.fadeDuration);
            foreach (var kvp in _activeLayerFades)
            {
                if (kvp.Value != null) StopCoroutine(kvp.Value);
            }
            _activeLayerFades.Clear();

            foreach (var source in _layerSources)
            {
                source.Stop();
                source.volume = 0f;
            }


            //レイヤーの配列を取得
            AudioClip[] clips = data.GetAllClips();

            //必要な数だけAudioSourceを用意し、足りなければ追加する
            PrepareLayerSources(clips.Length);

            //オーディオエンジンの現在の精密の時間を取得する
            double startTime = AudioSettings.dspTime + 0.1; //0.1秒後に一斉に予約をする

            for (int i = 0; i < clips.Length; i++)
            {
                if (clips[i] == null) continue; //空のスロットはスキップする

                _layerSources[i].clip = clips[i];
                _layerSources[i].loop = true;
                _layerSources[i].volume = (i == 0) ? 1f : 0f; //最初のパート(ドラム等)以外は0で開始

                //精密予約再生
                _layerSources[i].PlayScheduled(startTime);
            }
        }

        //イントロ再生後にメイン部分をループ再生
        public void PlayLayeredBGMWithIntro(LayeredSoundData data)
        {
            StopBGM(1.0f);

            foreach (var kvp in _activeLayerFades)
            {
                if (kvp.Value != null) StopCoroutine(kvp.Value);
            }
            _activeLayerFades.Clear();

            foreach (var source in _layerSources)
            {
                source.Stop();
                source.volume = 0f;
            }


            int layerCount = 4;
            PrepareLayerSources(layerCount * 2);    //イントロ用とループ用で2セット用意

            double introStartTime = AudioSettings.dspTime + 0.1;

            //イントロの長さを取得
            double introDuration = (double)data.introDrums.samples / data.introDrums.frequency;

            //ループ開始はイントロの開始+イントロの長さ
            double loopStartTime = introStartTime + introDuration;

            for (int i = 0; i < layerCount; i++)
            {
                //イントロの予約
                AudioSource introSrc = _layerSources[i];
                introSrc.clip = GetIntroClipByIndex(data, i);
                introSrc.loop = false;
                introSrc.PlayScheduled(introStartTime);

                //メインループの予約
                AudioSource loopSrc = _layerSources[i + layerCount];
                loopSrc.clip = GetLoopClipByIndex(data, i);
                loopSrc.loop = true;
                loopSrc.PlayScheduled(loopStartTime);

                //音量設定
                introSrc.volume = 1.0f;
                loopSrc.volume = 1.0f;
            }
        }

        //環境音開始
        public void PlayAmbient(SoundData data, float fadeDuration)
        {
            if (_ambientSource.clip == data.clip && _ambientSource.isPlaying) return;    //同じ音が鳴っていた場合は行わない
            StartCoroutine(FadeAmbient(data.clip, data.volume, fadeDuration));
        }

        //BGMを停止
        public void StopBGM(float fadeDuration)
        {
            if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
            _fadeCoroutine = StartCoroutine(FadeOutRoutine(fadeDuration));

            for (int i = 0; i < _layerSources.Count; i++)
            {
                AudioSource source = _layerSources[i];
                if (_activeLayerFades.ContainsKey(source) && _activeLayerFades[source] != null)
                {
                    StopCoroutine(_activeLayerFades[source]);
                }

                _activeLayerFades[source] = StartCoroutine(FadeOutAndStopCompletely(_layerSources[i], fadeDuration));
            }
        }

        //環境音停止
        public void StopAmbient(float fadeDuration)
        {
            StartCoroutine(FadeAmbient(null, 0, fadeDuration));
        }

        //音量設定
        public void SetVolume(VolumeType type, float volume)
        {
            //0を入れると計算不可能になってエラーが出るので、極小値を入れる
            float clampedValume = Mathf.Clamp(volume, 0.0001f, 1f);

            //デシベル変換
            float decibal = 20f * Mathf.Log10(clampedValume);

            string paramName = "";
            string saveKey = "";

            //enumに応じたパラメータ名を指定
            switch (type)
            {
                case VolumeType.Master: paramName = KeyMasterVolume; saveKey = KeyMasterVolume; break;
                case VolumeType.BGM: paramName = KeyBGMVolume; saveKey = KeyBGMVolume; break;
                case VolumeType.SE: paramName = KeySEVolume; saveKey = KeySEVolume; break;
                case VolumeType.UI: paramName = KeyUIVolume; saveKey = KeyUIVolume; break;
                case VolumeType.Ambient: paramName = KeyAmbientVolume; saveKey = KeyAmbientVolume; break;

            }
            ;

            mixer.SetFloat(paramName, decibal);

            //値を保存
            PlayerPrefs.SetFloat(saveKey, volume);
            PlayerPrefs.Save();
        }

        //レイヤーのボリュームを変更(part)
        public void SetLayerVolume(BgmPartType part, float volume, float duration)
        {
            int index = (int)part;
            ExecuteLayerFade(index, volume, duration);
        }
        //レイヤーのボリュームを変更(index)
        public void SetLayerVolume(int index, float volume, float duration)
        {
            ExecuteLayerFade(index, volume, duration);
        }

        //環境設定を適応
        public void SetEnvironment(string snapshotName, float duration)
        {
            AudioMixerSnapshot snapshot = mixer.FindSnapshot(snapshotName);

            if (snapshot != null)
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

        //全てのレイヤーの音量を設定
        public void SetAllLayersVolume(float volume, float duration)
        {
            for (int i = 0; i < _layerSources.Count; i++)
            {
                ExecuteLayerFade(i, volume, duration);
            }
        }

        //プリセットを適応
        public void ApplyPreset(BgmPreset preset, float duration = 0.5f)
        {
            float[] targetVolumes = preset.GetVolumeArray();


            for (int i = 0; i < targetVolumes.Length; i++)
            {
                //レイヤーが存在する場合のみ音量を変更
                if (i < _layerSources.Count)
                {
                    ExecuteLayerFade(i, targetVolumes[i], duration);
                }
            }
        }


        //----------------以下内部処理----------------

        //保存された音量設定をロードする
        private void LoadVolumes()
        {
            SetVolume(VolumeType.Master, PlayerPrefs.GetFloat(KeyMasterVolume, 1.0f));
            SetVolume(VolumeType.BGM, PlayerPrefs.GetFloat(KeyBGMVolume, 1.0f));
            SetVolume(VolumeType.SE, PlayerPrefs.GetFloat(KeySEVolume, 1.0f));
            SetVolume(VolumeType.UI, PlayerPrefs.GetFloat(KeyUIVolume, 1.0f));
            SetVolume(VolumeType.Ambient, PlayerPrefs.GetFloat(KeyAmbientVolume, 1.0f));
        }

        //オーディオソースをレイヤー分用意
        private void PrepareLayerSources(int count)
        {
            while (_layerSources.Count < count)
            {
                var source = gameObject.AddComponent<AudioSource>();
                source.outputAudioMixerGroup = mixer.FindMatchingGroups("BGM")[0];
                _layerSources.Add(source);
            }
        }

        //音量変更の共通処理
        private void ExecuteLayerFade(int index, float volume, float duration)
        {
            if (index < 0 || index >= _layerSources.Count) return;

            AudioSource source = _layerSources[index];

            if (_activeLayerFades.ContainsKey(source) && _activeLayerFades[source] != null)
            {
                StopCoroutine(_activeLayerFades[source]);
            }

            _activeLayerFades[source] = StartCoroutine(FadeLayerVolumeRoutine(source, volume, duration));
        }

        //音量の変更
        private System.Collections.IEnumerator FadeLayerVolumeRoutine(AudioSource source, float target, float duration)
        {
            float startVol = source.volume;
            float time = 0;
            while (time < duration)
            {
                time += Time.deltaTime; //Time使用
                source.volume = Mathf.Lerp(startVol, target, time / duration);
                yield return null;
            }
            source.volume = target;

            _activeLayerFades[source] = null;
        }
        //プールを返す
        private System.Collections.IEnumerator ReturnToPool(AudioSource source)
        {
            yield return new WaitWhile(() => source.isPlaying);
            source.gameObject.SetActive(false);
        }

        //BGM切り替え時のクロスフェード処理
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

        //停止時のフェード処理
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

        //完全にBGMを停止
        private System.Collections.IEnumerator FadeOutAndStopCompletely(AudioSource source, float duration)
        {
            float startVol = source.volume;
            float time = 0;
            while (time < duration)
            {
                time += Time.deltaTime;//Time使用
                source.volume = Mathf.Lerp(startVol, 0f, time / duration);
                yield return null;
            }
            source.volume = 0;
            source.Stop();

            if (_activeLayerFades.ContainsKey(source))
            {
                _activeLayerFades[source] = null;
            }
        }

        //環境音切り替え
        private System.Collections.IEnumerator FadeAmbient(AudioClip clip, float targetVolume, float duration)
        {
            float startVol = _ambientSource.volume;
            for (float t = 0; t < duration; t += Time.deltaTime)    //Time使用
            {
                _ambientSource.volume = Mathf.Lerp(startVol, 0, t / duration);
                yield return null;
            }

            if (clip == null)
            {
                _ambientSource.Stop();
                _ambientSource.clip = null;
                yield break;
            }

            _ambientSource.clip = clip;
            _ambientSource.loop = true;
            _ambientSource.Play();

            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                _ambientSource.volume = Mathf.Lerp(0, targetVolume, t / duration);
                yield return null;
            }
            _ambientSource.volume = targetVolume;
        }

        //利用可能なオーディオソースを返す
        private AudioSource GetAvailableSources() => _sePool.Find(s => !s.gameObject.activeSelf);

        //レイヤーサウンドデータの各クリップをindexで取得するヘルパー関数
        private AudioClip GetIntroClipByIndex(LayeredSoundData data, int i) => i switch
        {
            0 => data.introMelody,
            1 => data.introCords,
            2 => data.introBass,
            3 => data.introDrums,
            _ => null,
        };
        private AudioClip GetLoopClipByIndex(LayeredSoundData data, int i) => i switch
        {
            0 => data.melody,
            1 => data.chords,
            2 => data.bass,
            3 => data.drums,
            _ => null,
        };

    }
}