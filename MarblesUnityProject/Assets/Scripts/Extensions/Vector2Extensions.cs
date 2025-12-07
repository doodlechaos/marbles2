using FPMathLib;
using UnityEngine;

public static class Vector2Extensions
{
    public static FPVector2 ToFPVector2(this Vector2 vector)
    {
        return new FPVector2(FP.FromFloat(vector.x), FP.FromFloat(vector.y));
    }
}
