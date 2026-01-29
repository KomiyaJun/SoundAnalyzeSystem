using UnityEngine;

public class SoundBridge : MonoBehaviour
{
    //prefab且つDontDestroyOnLoadを利用しているSoundManagerに対する橋渡しを行うクラス

    [SerializeField] private float defaultFadeDuration = 1.0f;

    public void SetEnviroment(string snapshotName)
    {
        SoundService.Instance.SetEnvironment(snapshotName);
    }

    public void PlaySE(SoundData data)
    {
        SoundService.Instance.PlaySE(data);
    }

    public void StopBGM()
    {
        SoundService.Instance.StopBGM();
    }

}
