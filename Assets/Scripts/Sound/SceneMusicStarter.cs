using UnityEngine;

public class SceneMusicStarter : MonoBehaviour
{
    [SerializeField] private SoundData sceneBGM;
    [SerializeField] private float crossFadeTime = 2.0f;
    void Start()
    {
        //シーン開始時に再生リクエストを送信
        if(sceneBGM != null)
        {
            SoundService.Instance.PlayBGM(sceneBGM,crossFadeTime);
        }
    }


}
