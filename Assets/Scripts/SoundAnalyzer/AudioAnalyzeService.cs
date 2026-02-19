using UnityEngine;

public class AudioAnalyzeService
{
    public static IAudioAnalyzer Instance { get; private set; }

    public static void Provide(IAudioAnalyzer analyzer)
    {
        Instance = analyzer;
    }

    public static void Clear()
    {
        Instance = null;
    }

}
