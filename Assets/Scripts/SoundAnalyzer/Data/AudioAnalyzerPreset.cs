using MyGame.AudioSetting;
using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "AudioAnalyzerPreset", menuName = "Sound/AudioAnalyzerPreset")]
public class AudioAnalyzerPreset : ScriptableObject
{
    [Header("•ªÍ‘ÎÛƒp[ƒg")]
    [SerializeField] private List<BgmPartType> targetParts = new List<BgmPartType>();

    public IReadOnlyList<BgmPartType> TargetParts => targetParts;
}
