using UnityEngine;

namespace MoreInjuries.Utils;

public static class MathEx
{
    extension(Mathf)
    {
        public static float Modulo(float a, float b) => a - (b * Mathf.Floor(a / b));

        public static float Logistic(float value, float midpoint, float sharpness) =>
            1f / (1f + Mathf.Exp(-sharpness * (value - midpoint)));

        public static float InverseHillFactor(float x, float halfEffect, float exponent = 2f) =>
            1f / (1f + Mathf.Pow(x / halfEffect, exponent));
    }
}
