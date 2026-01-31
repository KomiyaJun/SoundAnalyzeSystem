using UnityEngine;

public class SoundBridge : MonoBehaviour
{
    //prefab且つDontDestroyOnLoadを利用しているSoundManagerに対する橋渡しを行うクラス

    [SerializeField] private float defaultFadeDuration = 1.0f;

    public void SetEnviroment(string snapshotName)
    {
        SoundService.Instance.SetEnvironment(snapshotName);
    }

    public void PlayBGM(SoundData data)
    {
        SoundService.Instance.PlayBGM(data);
    }

    public void PlaySE(SoundData data)
    {
        SoundService.Instance.PlaySE(data);
    }

    public void PlayLayerBGM(LayeredSoundData data)
    {
        SoundService.Instance.PlayLayeredBGM(data);
    }

    public void ActivateLayer(int index) => SoundService.Instance.SetLayerVolume(index, 1.0f);
    public void DeactivateLayer(int index) => SoundService.Instance.SetLayerVolume(index, 1.0f);

    public void SetAllLayersVolume(float volume)
    {
        SoundService.Instance.SetAllLayersVolume(volume);
    }

    public void ApplyPreset(BgmPreset preset)
    {
        SoundService.Instance.ApplyPreset(preset);
    }

    public void StopBGM()
    {
        SoundService.Instance.StopBGM();
    }

}
