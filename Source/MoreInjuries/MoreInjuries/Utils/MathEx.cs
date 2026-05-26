using UnityEngine;

namespace MoreInjuries.Utils;

public static class MathEx
{
    extension (Mathf)
    {
        public static float Modulo(float a, float b) => a - (b * Mathf.Floor(a / b));

        public static float Logistic(float value, float midpoint, float sharpness) =>
            1f / (1f + Mathf.Exp(-sharpness * (value - midpoint)));
    }
}
