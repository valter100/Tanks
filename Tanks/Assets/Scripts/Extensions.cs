using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    /// <summary>
    /// Returns the result of memberwise multiplication
    /// between this and the provided Vector2.
    /// </summary>
    public static Vector2 Mul(this Vector2 u, Vector2 v)
    {
        return new Vector2(u.x * v.x, u.y * v.y);
    }

    /// <summary>
    /// Returns the result of memberwise multiplication
    /// between this and the provided Vector3.
    /// </summary>
    public static Vector3 Mul(this Vector3 u, Vector3 v)
    {
        return new Vector3(u.x * v.x, u.y * v.y, u.z * v.z);
    }

    /// <summary>
    /// Returns a Vector2 constructed using this Vector3's x and y.
    /// </summary>
    public static Vector2 ToV2(this Vector3 vector)
    {
        return new Vector2(vector.x, vector.y);
    }

    /// <summary>
    /// Inflates the edges of this Rectangle by the provided amount.
    /// </summary>
    public static void Inflate(this ref Rect rect, float x, float y)
    {
        rect.x -= x;
        rect.y -= y;
        rect.width += x * 2.0f;
        rect.height += y * 2.0f;
    }

    /// <summary>
    /// Returns a new float to be used, instead of this float, as argument t in any lerp function.
    /// Near perfect accuracy. Moderate effect. See plotted functions: https://www.desmos.com/calculator/8mw89orcen
    /// </summary>
    public static float LerpValueSigmoidCurve(this float x)
    {
        // Custom sigmoid curve

        float b = 0.008f;
        return Mathf.Clamp01((-1.0f + 2.0f * b) / (1.0f + Mathf.Pow(2.0f, 14.0f * (x - 0.5f))) + 1.0f + b);
    }

    /// <summary>
    /// Returns a new float to be used, instead of this float, as argument t in any lerp function.
    /// Perfect accuracy. Subtle effect. See plotted functions: https://www.desmos.com/calculator/8mw89orcen
    /// </summary>
    public static float LerpValueSmoothstep(this float x)
    {
        // Classic smoothstep function

        x = Mathf.Clamp01(x);
        return x * x * (3.0f - 2.0f * x);
    }

}
