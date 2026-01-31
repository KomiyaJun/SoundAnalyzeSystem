using UnityEngine;

[CreateAssetMenu(fileName = "NewBgmPreset", menuName = "Sound/BgmPreset")]
public class BgmPreset : ScriptableObject
{
    [Range(0f, 1f)] public float[] layerVolumes;
}
