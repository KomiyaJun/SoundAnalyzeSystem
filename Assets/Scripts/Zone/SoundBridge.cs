using Unity.Android.Gradle.Manifest;
using UnityEngine;

namespace MyGame.AudioSetting
{
    public class SoundBridge : MonoBehaviour
    {
        //prefab且つDontDestroyOnLoadを利用しているSoundManagerに対する橋渡しを行うクラス

        [SerializeField] private float defaultFadeDuration = 1.0f;

        public void PlaySE(SoundData data)
        {
            SoundService.Instance.PlaySE(data);
        }

        public void PlayBGM(SoundData data)
        {
            SoundService.Instance.PlayBGM(data, defaultFadeDuration);
        }

        public void PlayLayeredBGM(LayeredSoundData data)
        {
            SoundService.Instance.PlayLayeredBGM(data);
        }

        public void PlayLayeredBGMWithIntro(LayeredSoundData data)
        {
            SoundService.Instance.PlayLayeredBGMWithIntro(data);
        }

        public void PlayAmbient(SoundData data)
        {
            SoundService.Instance.PlayAmbient(data, defaultFadeDuration);
        }

        public void StopBGM()
        {
            SoundService.Instance.StopBGM(defaultFadeDuration);
        }

        public void StopAmbient()
        {
            SoundService.Instance.StopAmbient(defaultFadeDuration);
        }

        public void ActivateLayer(BgmPartType part) => SoundService.Instance.SetLayerVolume(part, defaultFadeDuration);
        public void DeactivateLayer(BgmPartType part) => SoundService.Instance.SetLayerVolume(part, defaultFadeDuration);

        public void SetEnviroment(string snapshotName)
        {
            SoundService.Instance.SetEnvironment(snapshotName, defaultFadeDuration);
        }

        public void SetAllLayersVolume(float volume)
        {
            SoundService.Instance.SetAllLayersVolume(volume, defaultFadeDuration);
        }

        public void ApplyPreset(BgmPreset preset)
        {
            SoundService.Instance.ApplyPreset(preset, defaultFadeDuration);
        }

    }
}