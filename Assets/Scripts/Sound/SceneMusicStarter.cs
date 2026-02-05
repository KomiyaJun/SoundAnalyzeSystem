using UnityEngine;

public class SceneMusicStarter : MonoBehaviour
{
    [SerializeField] private float crossFadeTime = 2.0f;
    [Header("単一BGM再生[false] or レイヤー再生[true]")]
    [SerializeField] private bool isLayeredBGM = false;

    [Header("単一のBGM再生")]
    [SerializeField] private SoundData sceneBGM;

    [Header("レイヤー分割されたBGM再生")]
    [SerializeField] private LayeredSoundData layeredBGM;

    [Header("環境音設定")]
    [SerializeField] private bool isAmbientON = false;
    [SerializeField] private SoundData ambientSound;

    void Start()
    {
        if (!isLayeredBGM)
        {
            //シーン開始時に再生リクエストを送信
            if (sceneBGM != null)
            {
                SoundService.Instance.PlayBGM(sceneBGM, crossFadeTime);
            }
        }
        else
        {
            if(layeredBGM != null)
            {
                SoundService.Instance.PlayLayeredBGM(layeredBGM);
                SoundService.Instance.SetAllLayersVolume(1);
            }
        }

        if (isAmbientON)
        {
            if (ambientSound == null) return;
            SoundService.Instance.PlayAmbient(ambientSound);
        }
    }


}
