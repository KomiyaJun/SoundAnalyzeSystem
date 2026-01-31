using UnityEngine;

[CreateAssetMenu(fileName = "NewLayeredSoundData", menuName = "Sound/LayeredBGM")]
public class LayeredSoundData : ScriptableObject
{
    public AudioClip[] layers;
    public float fadeDuration = 1.0f;
}
