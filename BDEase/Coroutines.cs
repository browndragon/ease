using System;
using System.Collections;
using System.Collections.Generic;

namespace BDEase
{
    /// Utilities for Unity-style enumerator coroutines.
    public static class Coroutines
    {
        /// Transforms a coroutine in T (usually float?) by adding a side effect `action`.
        /// This action can trigger halt by returning false; re-yields each `t` in `thiz`.
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> thiz, Func<T, bool> action)
        {
            foreach (T t in thiz)
            {
                if (!action(t)) break;
                yield return t;
            }
        }
        /// Transforms a coroutine in T (usually float?) by adding a side effect `action`.
        /// This action can trigger halt by returning false; re-yields each `t` in `thiz`.
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> thiz, Func<bool> action)
        {
            foreach (T t in thiz)
            {
                if (!action()) break;
                yield return t;
            }
        }
        /// Transforms a coroutine in T (usually float?) by adding a side effect `action`.
        /// Re-yields each `t` in `thiz`.
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> thiz, Action<T> action)
        {
            foreach (T t in thiz)
            {
                action(t);
                yield return t;
            }
        }
        /// Transforms a coroutine in T (usually float?) by adding a side effect `action`.
        /// Re-yields each `t` in `thiz`.
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> thiz, Action action)
        {
            foreach (T t in thiz)
            {
                action();
                yield return t;
            }
        }
        /// Transforms a typeless coroutine by adding a side effect `action`.
        /// This action can trigger halt by returning false; re-yields each `t` in `thiz`.
        public static IEnumerable ForEach(this IEnumerable thiz, Func<bool> action)
        {
            foreach (object t in thiz)
            {
                if (!action()) break;
                yield return t;
            }
        }
        /// Transforms a typeless coroutine by adding a side effect `action`.
        /// Re-yields each `t` in `thiz`.
        public static IEnumerable ForEach(this IEnumerable thiz, Action action)
        {
            foreach (object t in thiz)
            {
                action();
                yield return t;
            }
        }

        /// Transforms a coroutine (of T) by adding ending/cleanup actions (in a try/finally block).
        /// Since the underlying is "just" an enumerable, there's no good semantic for success/failure.
        /// However, you _can_ provide a @catch to handle thrown exceptions, which are otherwise simply logged and swallowed
        /// (though: ending iteration, and still triggering @finally).
        public static IEnumerable<T> AndThen<T>(this IEnumerable<T> thiz, Action @finally, Action<Exception> @catch = default)
        {
            @catch ??= Logging.DefaultException;
            using IEnumerator<T> enumerator = thiz.GetEnumerator();
            while (true)
            {
                try { if (!enumerator.MoveNext()) break; }
                catch (Exception e)
                {
                    @catch?.Invoke(e);
                    @finally?.Invoke();
                    yield break;
                }
                T item = enumerator.Current;
                yield return item;
            }
            @finally?.Invoke();
        }
        /// Transforms a coroutine (of nongeneric) by adding ending/cleanup actions (in a try/finally block).
        /// Since the underlying is "just" an enumerable, there's no good semantic for success/failure.
        /// However, you _can_ provide a @catch to handle thrown exceptions, which are otherwise simply logged and swallowed
        /// (though: ending iteration, and still triggering @finally).
        public static IEnumerable AndThen(this IEnumerable thiz, Action @finally, Action<Exception> @catch = default)
        {
            @catch ??= Logging.DefaultException;
            IEnumerator enumerator = thiz.GetEnumerator();
            using IDisposable disposeMe = enumerator as IDisposable;
            while (true)
            {
                try { if (!enumerator.MoveNext()) break; }
                catch (Exception e)
                {
                    @catch?.Invoke(e);
                    @finally?.Invoke();
                    yield break;
                }
                object item = enumerator.Current;
                yield return item;
            }
            @finally?.Invoke();
        }

        /// Concatenates one or more coroutines together.
        /// Unlike AndThen(Action), this is weakly conditional; if `thiz` (or any previous in `nexts`) throws, `nexts` won't even start.
        public static IEnumerable<T> AndThen<T>(this IEnumerable<T> thiz, params IEnumerable<T>[] nexts)
        {
            foreach (T value in thiz) yield return value;
            foreach (IEnumerable next in nexts) foreach (T value in next) yield return value;
        }
        /// Concatenates one or more coroutines together.
        /// Unlike AndThen(Action), this is weakly conditional; if `thiz` (or any previous in `nexts`) throws, `nexts` won't even start.
        public static IEnumerable AndThen(this IEnumerable thiz, params IEnumerable[] nexts)
        {
            foreach (object value in thiz) yield return value;
            foreach (IEnumerable next in nexts) foreach (object value in next) yield return value;
        }

        /// Augments any coroutine by adding an abort switch, checked immediately on returning from yield ("soft abort").
        /// This simply stops iterating.
        public static IEnumerable<T> Unless<T>(this IEnumerable<T> thiz, Func<bool> abort)
        {
            foreach (T value in thiz)
            {
                yield return value;
                if (abort()) yield break;
            }
        }
        /// Augments any coroutine by adding an abort switch, checked immediately on returning from yield ("soft abort").
        /// This simply stops iterating.
        public static IEnumerable Unless(this IEnumerable thiz, Func<bool> abort)
        {
            foreach (object value in thiz)
            {
                yield return value;
                if (abort()) yield break;
            }
        }
    }
}