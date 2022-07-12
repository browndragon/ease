using System;
using System.Collections.Generic;
using System.Reflection;

namespace BDEase
{
    public interface IDefaultArith { }
    /// Static types & classes for doing arithmetic through generics, similar to I/EqualityComparer.
    /// Particularly, see BDEase.Unity, where VectorX are supported.
    public interface IArith<T>
    {
        /// Returns a+b
        T Add(T a, T b);
        /// Returns a' parallel to a st ||a'|| <= max.
        T Clamp(T a, float max);
        /// Returns a*b
        T Scale(float a, T b);
        /// Returns a dotproduct b (~= a*b).
        float Dot(T a, T b);
    }
    /// Extension methods that generalize.
    public static class ArithExt
    {
        public static readonly float Epsilon = 1e-06f;

        #region Unity "Mathf" imports
        public static float Sqrt(float a) => (float)Math.Sqrt(a);
        public static float Clamp01(float a) => Math.Min(Math.Max(a, 0f), 1f);
        #endregion

        #region Arithmetic in terms of existing arithmetic.
        public static float Length2<T>(this IArith<T> thiz, T a) => thiz.Dot(a, a);
        public static float Length<T>(this IArith<T> thiz, T a) => Sqrt(thiz.Length2(a));
        public static bool Approximately<T>(this IArith<T> thiz, T a) => thiz.Approximately(a, Epsilon);
        public static bool Approximately<T>(this IArith<T> thiz, T a, float epsilon) => thiz.Length2(a) < (epsilon * epsilon);
        public static bool Approximately<T>(this IArith<T> thiz, T a, T b, float epsilon) => thiz.Approximately(thiz.Difference(b, a), epsilon);
        public static float Normalize<T>(this IArith<T> thiz, T a, out T b)
        {
            float len = thiz.Length(a);
            b = thiz.Scale(1f / len, a);
            return len;
        }
        public static T Negate<T>(this IArith<T> thiz, T a) => thiz.Scale(-1f, a);
        public static T Difference<T>(this IArith<T> thiz, T a, T b) => thiz.Add(a, thiz.Negate(b));
        public static T LerpUnclamped<T>(this IArith<T> thiz, T a, T b, float ratio) => thiz.Add(a, thiz.Scale(ratio, thiz.Difference(b, a)));
        public static T Lerp<T>(this IArith<T> thiz, T a, T b, float ratio) => thiz.LerpUnclamped(a, b, Clamp01(ratio));

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
                if (underlying != null)
                {
                    RegisterDefault(underlying, Activator.CreateInstance(t) as IDefaultArith);
                }
            }
        }
        static bool initialized = false;
        static ArithExt() => Initialize();
        internal static void Initialize()
        {
            if (initialized) return;
            initialized = true;
            RegisterAssembly(typeof(ArithExt).GetTypeInfo().Assembly);
        }
        #endregion
    }
    /// VERY similar to EqualityComparer, provides the generic type dispatch through Arith<T>.Default.
    public class Arith<T> : IArith<T>
    {
        public static IArith<T> Default { get; private set; }
        static Arith()
        {
            ArithExt.Initialize();
            Default ??= ArithExt.GetDefault<T>();
            if (Default == null)
            {
                string name = typeof(T).Name;
                throw new Exception($"Couldn't find `IArith<T>` for T={name}; remember to register one!");
            }
        }
        T IArith<T>.Add(T a, T b) => Default.Add(a, b);
        T IArith<T>.Clamp(T a, float max) => Default.Clamp(a, max);
        float IArith<T>.Dot(T a, T b) => Default.Dot(a, b);
        T IArith<T>.Scale(float a, T b) => Default.Scale(a, b);
    }
    public struct FloatArith : IArith<float>, IDefaultArith
    {
        float IArith<float>.Add(float a, float b) => a + b;
        /// Unity provides Clamp, but sadly, this lib doesn't take that dep.
        float IArith<float>.Clamp(float a, float max) => Math.Min(Math.Max(a, -max), +max);
        float IArith<float>.Dot(float a, float b) => a * b;
        float IArith<float>.Scale(float a, float b) => a * b;
    }
}