using UnityEditor.U2D.Aseprite;
using UnityEngine;

public enum VolumeType
{
    Master,
    BGM,
    SE,
    UI,
    Ambient,
}

public interface ISoundManager
{
    //SE再生
    void PlaySE(SoundData data);
    
    //BGM再生
    void PlayBGM(SoundData data, float fadeDuration = 1.0f);

    //BGM停止
    void StopBGM(float fadeDuration = 1.0f);

    //環境変化(水中で音を曇らせるなど)
    void SetEnvironment(string snapShotName, float duration = 1.0f);

    //音量変更
    void SetVolume(VolumeType type, float volume);

    //レイヤーBGMの再生
    void PlayLayeredBGM(LayeredSoundData data);

    //レイヤーの全ての音量を均一で変更
    void SetAllLayersVolume(float volume, float duration = 0.5f);

    //レイヤーのindexに該当する部分の音量を調整
    void SetLayerVolume(BgmPartType part, float volume, float duration = 0.5f);

    //音量プリセットを使って音量を調節
    void ApplyPreset(BgmPreset preset, float duratio = 1.0f);

    //環境音の再生
    void PlayAmbient(SoundData data, float fadeDuration = 1.0f);

    //環境音の停止
    void StopAmbient(float fadeDuration = 1.0f);
}
