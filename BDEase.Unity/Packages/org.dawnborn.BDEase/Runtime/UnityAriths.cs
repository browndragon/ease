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
        public static void Initialize()
        {
            if (initialized) return;
            initialized = true;
            Arith.RegisterAssembly(typeof(UnityAriths).GetTypeInfo().Assembly);
        }
    }
    public struct V2Arith : IArith<Vector2>, IDefaultArith
    {
        Vector2 IArith<Vector2>.Add(Vector2 a, Vector2 b) => a + b;
        float IArith<Vector2>.Dot(Vector2 a, Vector2 b) => Vector2.Dot(a, b);
        Vector2 IArith<Vector2>.Scale(float a, Vector2 b) => a * b;
    }
    public struct V3Arith : IArith<Vector3>, IDefaultArith
    {
        Vector3 IArith<Vector3>.Add(Vector3 a, Vector3 b) => a + b;
        float IArith<Vector3>.Dot(Vector3 a, Vector3 b) => Vector3.Dot(a, b);
        Vector3 IArith<Vector3>.Scale(float a, Vector3 b) => a * b;
    }
    public struct V4Arith : IArith<Vector4>, IDefaultArith
    {
        Vector4 IArith<Vector4>.Add(Vector4 a, Vector4 b) => a + b;
        float IArith<Vector4>.Dot(Vector4 a, Vector4 b) => Vector4.Dot(a, b);
        Vector4 IArith<Vector4>.Scale(float a, Vector4 b) => a * b;
    }
    public struct ColorArith : IArith<Color>, IDefaultArith
    {
        static readonly IArith<Vector4> AsV4 = Arith<Vector4>.Default;
        Color IArith<Color>.Add(Color a, Color b) => a + b;
        float IArith<Color>.Dot(Color a, Color b) => AsV4.Dot(a, b);
        Color IArith<Color>.Scale(float a, Color b) => a * b;
    }
}