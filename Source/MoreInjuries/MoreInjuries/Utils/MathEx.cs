using UnityEngine;

namespace MoreInjuries.Utils;

public static class MathEx
{
    public static float Modulo(float a, float b) => a - (b * Mathf.Floor(a / b));
}