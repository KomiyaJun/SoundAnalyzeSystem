using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "BeatData", menuName = "Sound/BeatData")]
public class BeatData : ScriptableObject
{
    public string audioClipName;    
    public float bpm;

    //ビートが出た時間のリスト
    public List<float> beatTimestamps = new List<float>();
}
