using System;
using System.Threading.Tasks;

namespace BDEase
{
    public static class Actions
    {
        /// Creates a Size(1) pipe to detect whether the out-returned Action has been called.
        public static Func<bool> MakeSignalPair(out Action set)
        {
            bool has = false;
            set = () => has = true;
            return () => has;
        }
        /// As `MakeSignalPair()` but the action can pass information.
        public static Func<T> MakeSignalPair<T>(out Action<T> set)
        {
            T value = default;
            set = (T t) => value = t;
            return () => value;
        }
        /// Creates a function which takes individual delta seconds and calls action(lerp(start, end, elapsed)) until elapsed > duration.
        public static Func<float, bool> Lerped<T>(this IArith<T> thiz, Action<T> action, float duration, T start, T end, Func<float, float> ease = default, Func<bool> isCanceled = default)
        {
            ease ??= Easings.Linear;
            float elapsed = 0f;
            return (f) =>
            {
                if (isCanceled != null && isCanceled()) return false;
                if (elapsed >= duration)
                {
                    action(thiz.Lerp(start, end, ease(1f)));
                    return false;
                }
                action(thiz.Lerp(start, end, ease(elapsed / duration)));
                elapsed += f;
                return true;
            };
        }
    }
}
