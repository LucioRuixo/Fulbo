using UnityEngine;

namespace Fulbo
{
    public static class FloatExtensions
    {
        public static float Half(this float value) => value * 0.5f;

        public static float Rounded(this float value, int decimals = 0)
        {
            decimals = Mathf.Clamp(decimals, 0, 10);
            return Mathf.Round(value * Mathf.Pow(10f, decimals)) * Mathf.Pow(0.1f, decimals);
        }
    }
}