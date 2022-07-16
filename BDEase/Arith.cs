using System;
using System.Collections.Generic;
using System.Reflection;

namespace BDEase
{
    /// Static types & classes for doing arithmetic through generics, similar to I/EqualityComparer.
    /// Particularly, see BDEase.Unity, where VectorX & color are supported.
    public interface IArith<T>
    {
        /// Returns a+b
        T Add(T a, T b);
        /// Returns a*b
        T Scale(float a, T b);
        /// Returns a * b.
        float Dot(T a, T b);
    }
    /// Marker interface; this type provides the default arithmetic
    /// for any IArith<T> with which it's marked.
    /// Remember to call Arith.RegisterAssembly somewhere (a static initializer from a required class?) to ensure this.
    // Implementation note, we only handle the first such. Could be fixed.
    public interface IDefaultArith { }

    /// Extension methods, constants, and type registration.
    /// Extensions generalize IArith<T> with operations implied by the existing ones (like dotproduct gets us length).
    ///
    public static class Arith
    {
        // Used for (very fuzzy) matching.
        public const float Epsilon = 1e-06f;

        public const float PI = (float)Math.PI;
        public const float TAU = 2 * PI;
        public const float HALF_PI = PI / 2f;

        #region Unity "Mathf" imports
        public static float Sqrt(float a) => (float)Math.Sqrt(a);
        public static float Clamp(float a, float min, float max) => Math.Min(Math.Max(a, min), max);
        public static float Clamp01(float a) => Clamp(a, 0f, 1f);
        public static float Repeat(float a, float length) => Clamp((float)(a - Math.Floor(a / length) * length), 0.0f, length);

        public static float Pow(float x, float y) => (float)Math.Pow(x, y);
        public static float Cos(float x) => (float)Math.Cos(x);
        public static float Sin(float x) => (float)Math.Sin(x);

        #endregion

        #region Arithmetic in terms of existing arithmetic.
        public static float Length2<T>(this IArith<T> thiz, T a) => thiz.Dot(a, a);
        public static float Length<T>(this IArith<T> thiz, T a) => Sqrt(thiz.Length2(a));
        public static T Clamp<T>(this IArith<T> thiz, T a, float length)
        {
            float len = thiz.Length2(a);
            if (len <= (length * length)) return a;
            if (len == 0) return default;
            return thiz.Scale(length / Sqrt(len), a);
        }
        public static float Normalize<T>(this IArith<T> thiz, T a, out T b)
        {
            float len = thiz.Length(a);
            b = thiz.Scale(1f / len, a);
            return len;
        }
        public static bool Approximately<T>(this IArith<T> thiz, T a) => thiz.Approximately(a, Epsilon);
        public static bool Approximately<T>(this IArith<T> thiz, T a, float epsilon) => thiz.Length2(a) < (epsilon * epsilon);
        public static bool Approximately<T>(this IArith<T> thiz, T a, T b, float epsilon) => thiz.Approximately(thiz.Difference(b, a), epsilon);
        public static T Negate<T>(this IArith<T> thiz, T a) => thiz.Scale(-1f, a);
        public static T Difference<T>(this IArith<T> thiz, T a, T b) => thiz.Add(a, thiz.Negate(b));
        public static T LerpUnclamped<T>(this IArith<T> thiz, T a, T b, float ratio) => thiz.Add(a, thiz.Scale(ratio, thiz.Difference(b, a)));
        public static T Lerp<T>(this IArith<T> thiz, T a, T b, float ratio) => thiz.LerpUnclamped(a, b, Clamp01(ratio));
        public static T Project<T>(this IArith<T> thiz, T a, T b)
        {
            float length2 = thiz.Length2(b);
            if (length2 < Epsilon) return default;
            float dot = thiz.Dot(a, b);
            return thiz.Scale(dot / length2, b);
        }
        #endregion

        #region Static factory registration
        static readonly Dictionary<Type, object> registry = new();
        /// Populates map from e.g. float->FloatArith.
        /// We require IDefaultArith's instance ALSO implement IArith<that type>.
        public static void RegisterDefault(Type t, IDefaultArith instance) => registry[t] = instance;
        public static IArith<T> GetDefault<T>() => registry.TryGetValue(typeof(T), out var arith) ? (IArith<T>)arith : null;
        public static void RegisterAssembly(Assembly assembly)
        {
            Type iarithDefault = typeof(IDefaultArith);
            Type iarithG = typeof(IArith<>);
            foreach (var t in assembly.GetTypes())
            {
                bool isDefault = false;
                foreach (var i in t.GetInterfaces())
                {
                    if (iarithDefault.IsAssignableFrom(i))
                    {
                        isDefault = true;
                        break;
                    }
                }
                if (!isDefault) continue;

                Type underlying = default;
                foreach (var i in t.GetInterfaces())
                {
                    if (!i.IsGenericType) continue;
                    Type iG = i.GetGenericTypeDefinition();
                    if (iarithG == iG)
                    {
                        underlying = i.GetGenericArguments()[0];
                        break;
                    }
                }
                if (underlying != null) RegisterDefault(underlying, Activator.CreateInstance(t) as IDefaultArith);
            }
        }
        static bool initialized = false;
        static Arith() => Initialize();
        internal static void Initialize()
        {
            if (initialized) return;
            initialized = true;
            RegisterAssembly(typeof(Arith).GetTypeInfo().Assembly);
        }
        #endregion
    }
    /// VERY similar to EqualityComparer, provides the generic type dispatch through Arith<T>.Default.
    public class Arith<T> : IArith<T>
    {
        public static IArith<T> Default { get; private set; }
        static Arith()
        {
            Arith.Initialize();
            Default ??= Arith.GetDefault<T>();
            if (Default == null)
            {
                string name = typeof(T).Name;
                throw new Exception($"Couldn't find `IArith<T>` for T={name}; remember to register one!");
            }
        }

        T IArith<T>.Add(T a, T b) => Default.Add(a, b);
        float IArith<T>.Dot(T a, T b) => Default.Dot(a, b);
        T IArith<T>.Scale(float a, T b) => Default.Scale(a, b);
    }
    public struct IntArith : IArith<int>, IDefaultArith
    {
        int IArith<int>.Add(int a, int b) => a + b;
        float IArith<int>.Dot(int a, int b) => a * b;
        int IArith<int>.Scale(float a, int b) => (int)(a * b);
    }
    public struct FloatArith : IArith<float>, IDefaultArith
    {
        float IArith<float>.Add(float a, float b) => a + b;
        float IArith<float>.Dot(float a, float b) => a * b;
        float IArith<float>.Scale(float a, float b) => a * b;
    }

    /// Provides operations which suppose a circle, where 0f == TAU.
    /// Natively prefers
    public struct RadianArith : IArith<float>
    {
        public static readonly float Max = Arith.TAU;
        public static float Shortest(float a) => a <= Arith.PI ? a : (a - Arith.PI);
        // public static float AngleClamp(float a, float max)
        // {
        //     if (max < 0f) return 0f;
        //     if (max > Arith.PI) return Arith.Clamp(a, 0f, Max);
        //     float scale = 1f;
        //     if (a > Arith.PI)
        //     {
        //         scale = -1f;
        //         a = Max - a;
        //     }
        //     a = Arith.Repeat(a, Arith.TAU);
        //     a = Math.Min(a, max);
        //     a *= scale;
        //     a = Arith.Repeat(a, Arith.TAU);
        //     return a;
        // }
        float IArith<float>.Add(float a, float b) => Arith.Repeat(a + b, Max);
        /// This is only physically meaningful as an area or something.
        float IArith<float>.Dot(float a, float b) => Shortest(a) * Shortest(b);
        float IArith<float>.Scale(float a, float b) => Arith.Repeat(a * b, Max);
    }
    /// As RadianArith, but 0f == 360f.
    public struct DegreeArith : IArith<float>
    {
        public static readonly float Max = 360f;
        public static float Shortest(float a) => a <= 180f ? a : (a - 180f);

        float IArith<float>.Add(float a, float b) => Arith.Repeat(a + b, Max);
        /// This is only physically meaningful as an area or something.
        /// And who would do that in degree-space?!
        float IArith<float>.Dot(float a, float b) => Shortest(a) * Shortest(b);
        float IArith<float>.Scale(float a, float b) => Arith.Repeat(a * b, Max);
    }
}