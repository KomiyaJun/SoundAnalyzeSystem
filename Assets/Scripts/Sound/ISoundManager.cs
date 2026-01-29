using UnityEngine;

public interface ISoundManager
{
    //SEÄ¶
    void PlaySE(SoundData data);
    
    //BGMÄ¶
    void PlayBGM(SoundData data, float fadeDuration = 1.0f);

    //BGM’â~
    void StopBGM(float fadeDuration = 1.0f);

    //ŠÂ‹«•Ï‰»(…’†‚Å‰¹‚ğ“Ü‚ç‚¹‚é‚È‚Ç)
    void SetEnviroment(string snapShotName, float duration = 1.0f);
}
