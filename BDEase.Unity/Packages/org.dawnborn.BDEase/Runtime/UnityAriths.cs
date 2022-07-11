using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace BDEase
{
    public static class UnityAriths
    {
        static UnityAriths() => Initialize();
        static bool initialized = false;
        internal static void Initialize()
        {
            if (initialized) return;
            initialized = true;
            ArithExt.RegisterAssembly(typeof(UnityAriths).GetTypeInfo().Assembly);
        }
    }
    public struct RadianArith : IArith<float>
    {
        public static readonly float Max = 2 * Mathf.PI;

        float IArith<float>.Add(float a, float b) => Mathf.Repeat(a + b, Max);
        /// Unity provides Clamp, but sadly, this lib doesn't take that dep.
        float IArith<float>.Clamp(float a, float max) => Mathf.Clamp(a, 0f, Max);
        /// This is only physically meaningful as an area or something.
        float IArith<float>.Dot(float a, float b) => a * b;
        float IArith<float>.Scale(float a, float b) => Mathf.Repeat(a * b, Max);
    }
    public struct DegreeArith : IArith<float>
    {
        public static readonly float Max = 360f;

        float IArith<float>.Add(float a, float b) => Mathf.Repeat(a + b, Max);
        /// Unity provides Clamp, but sadly, this lib doesn't take that dep.
        float IArith<float>.Clamp(float a, float max) => Mathf.Clamp(a, 0f, Max);
        /// This is only physically meaningful as an area or something.
        /// And who would do that in degree-space?!
        float IArith<float>.Dot(float a, float b) => a * b;
        float IArith<float>.Scale(float a, float b) => Mathf.Repeat(a * b, Max);
    }
    public struct V2Arith : IArith<Vector2>, IDefaultArith
    {
        Vector2 IArith<Vector2>.Add(Vector2 a, Vector2 b) => a + b;
        Vector2 IArith<Vector2>.Clamp(Vector2 a, float max) => Vector2.ClampMagnitude(a, max);
        float IArith<Vector2>.Dot(Vector2 a, Vector2 b) => Vector2.Dot(a, b);
        Vector2 IArith<Vector2>.Scale(float a, Vector2 b) => a * b;
    }
    public struct V3Arith : IArith<Vector3>, IDefaultArith
    {
        Vector3 IArith<Vector3>.Add(Vector3 a, Vector3 b) => a + b;
        Vector3 IArith<Vector3>.Clamp(Vector3 a, float max) => Vector3.ClampMagnitude(a, max);
        float IArith<Vector3>.Dot(Vector3 a, Vector3 b) => Vector3.Dot(a, b);
        Vector3 IArith<Vector3>.Scale(float a, Vector3 b) => a * b;
    }
    public struct V4Arith : IArith<Vector4>, IDefaultArith
    {
        Vector4 IArith<Vector4>.Add(Vector4 a, Vector4 b) => a + b;
        Vector4 IArith<Vector4>.Clamp(Vector4 a, float max)
        {
            if (a.sqrMagnitude <= (max * max)) return a;
            a.Normalize();
            return max * a;
        }
        float IArith<Vector4>.Dot(Vector4 a, Vector4 b) => Vector4.Dot(a, b);
        Vector4 IArith<Vector4>.Scale(float a, Vector4 b) => a * b;
    }
    public struct ColorArith : IArith<Color>, IDefaultArith
    {
        static readonly IArith<Vector4> AsV4 = Arith<Vector4>.Default;
        Color IArith<Color>.Add(Color a, Color b) => a + b;
        Color IArith<Color>.Clamp(Color a, float max) => AsV4.Clamp(a, max);
        float IArith<Color>.Dot(Color a, Color b) => AsV4.Dot(a, b);
        Color IArith<Color>.Scale(float a, Color b) => a * b;
    }
}