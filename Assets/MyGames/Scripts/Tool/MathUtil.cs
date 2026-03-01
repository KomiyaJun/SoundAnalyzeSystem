using UnityEngine;

public static class MathUtil
{
    //多数利用する計算処理を汎用的にするためのclass

    /// <summary>
    /// inMin～inMaxの入力範囲から、outMin～outMaxの出力上限にLerp
    /// </summary>
    /// <param name="value">現在の値</param>
    /// <param name="inMin">入力下限</param>
    /// <param name="inMax">入力上限</param>
    /// <param name="outMin">出力下限</param>
    /// <param name="outMax">出力上限</param>
    /// <returns></returns>
    public static float Remap(this float value, float inMin, float inMax, float outMin, float outMax)
    {
        float t = Mathf.InverseLerp(inMin,inMax,value);
        return Mathf.Lerp(outMin,outMax,t);
    }
}
