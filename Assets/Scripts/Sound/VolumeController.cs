using UnityEngine;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider seSlider;
    [SerializeField] private Slider uiSlider;

    private const string KeyMasterVolume = "MasterVolume";
    private const string KeyBGMVolume = "BGMVolume";
    private const string KeySEVolume = "SEVolume";
    private const string KeyUIVolume = "UIVolume";

    private void Start()
    {
        //起動時に保存されている値をスライダーに反映する
        masterSlider.value = PlayerPrefs.GetFloat(KeyMasterVolume, 1.0f);
        bgmSlider.value = PlayerPrefs.GetFloat(KeyBGMVolume, 1.0f);
        seSlider.value = PlayerPrefs.GetFloat(KeySEVolume, 1.0f);
        uiSlider.value = PlayerPrefs.GetFloat(KeyUIVolume, 1.0f);

        //スライダー動作時のイベント登録
        masterSlider.onValueChanged.AddListener(v => SetVol(VolumeType.Master, v));
        bgmSlider.onValueChanged.AddListener(v => SetVol(VolumeType.BGM, v));
        seSlider.onValueChanged.AddListener(v => SetVol(VolumeType.SE, v));
        uiSlider.onValueChanged.AddListener(v => SetVol(VolumeType.UI, v));
    }

    private void SetVol(VolumeType type, float value)
    {
        SoundService.Instance.SetVolume(type, value);
    }
}
