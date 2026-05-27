using System.Collections.Generic;
using UnityEngine;

namespace MoreInjuries.Utils;

public static class MathEx
{
    extension(Mathf)
    {
        public static float Round(float value, int digits) => MathF.Round(value, digits);

        public static float Modulo(float a, float b) => a - (b * MathF.Floor(a / b));

        public static float Logistic(float value, float midpoint, float sharpness) =>
            1f / (1f + MathF.Exp(-sharpness * (value - midpoint)));

        public static float InverseHillFactor(float x, float halfEffect, float exponent = 2f) =>
            1f / (1f + MathF.Pow(x / halfEffect, exponent));
    }
}
