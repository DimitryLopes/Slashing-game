using UnityEngine;

public static class FloatExtensions
{
    public static float Truncate(this float value, int decimals)
    {
        float factor = Mathf.Pow(10f, decimals);
        return Mathf.Floor(value * factor) / factor;
    }
}
