using FPMathLib;
using UnityEngine;

public static class VectorExtensions
{
    public static FPVector2 ToFPVector2(this Vector2 vector)
    {
        return new FPVector2(FP.FromFloat(vector.x), FP.FromFloat(vector.y));
    }

    public static FPVector3 ToFPVector3(this Vector3 vector)
    {
        return new FPVector3(
            FP.FromFloat(vector.x),
            FP.FromFloat(vector.y),
            FP.FromFloat(vector.z)
        );
    }
}
