using System;

namespace BDEase.Labs
{
    using static BDEase.Actions;

    /// Extensions for actions to make them more fluent.
    /// Intended for a generic Lerp framework, I ended up using exactly none.
    /// The problem is that a real assembled pipeline for an engine,
    /// using a duration-based lerp on startup, space based lerp for braking,
    /// and dT-based convergence loop for a PID controller just doesn't work
    /// as a single function-of-float -- there's too many monads.
    /// But it seems like a shame to throw out all of these adapters.
    /// So... lock 'em away!
    public static class FluentActions
    {
        /// Discard func return value.
        public static Action Void<TOut>(this Func<TOut> thiz) => () => thiz();
        /// Discard func return value.
        public static Action<TIn> Void<TIn, TOut>(this Func<TIn, TOut> thiz) => (tin) => thiz(tin);
        /// Pass through input value.
        public static Func<T, T> Passing<T>(this Action thiz, T _ = default) => (t) => { thiz(); return t; };
        /// Pass through input value.
        public static Func<T, T> Passing<T>(this Action<T> thiz, T _ = default) => (t) => { thiz(t); return t; };

        public static Func<T> Const<T>(this Action thiz, T @const = default) => () => { thiz(); return @const; };
        public static Func<TIn, TOut> Const<TIn, TOut>(this Action<TIn> thiz, TOut @const = default) => (tin) => { thiz(tin); return @const; };

        /// Take a parameter you don't really take
        public static Action<T> Swallow<T>(this Action thiz, T _ = default) => (t) => thiz();
        /// Take a parameter you don't really take.
        public static Func<TIn, TOut> Swallow<TIn, TOut>(this Func<TOut> thiz, TIn _ = default) => (t) => thiz();

        /// Chain output->input.
        public static Func<TIn, TOut> Pipe<TIn, TMid, TOut>(this Func<TIn, TMid> thiz, Func<TMid, TOut> then)
        => (tin) => then(thiz(tin));
        /// Chain output->input.
        public static Func<TIn, TOut> Pipe<TIn, TMid, TOut>(this Func<TIn, TMid> thiz, Func<TIn, TMid, TOut> then)
        => (tin) => then(tin, thiz(tin));
        /// Chain output->input.
        public static Action<TIn> Pipe<TIn, TMid>(this Func<TIn, TMid> thiz, Action<TMid> then)
        => (tin) => then(thiz(tin));
        /// Chain output->input.
        public static Action<TIn> Pipe<TIn, TMid>(this Func<TIn, TMid> thiz, Action<TIn, TMid> then)
        => (tin) => then(tin, thiz(tin));

        /// Returns a function which appends an additional function (such as logging).
        public static Action AndAlso(this Action a, Action b)
        => () => { a(); b(); };
        /// Returns a function which appends an additional function (such as logging).
        public static Action<T> AndAlso<T>(this Action<T> a, Action b)
        => (t) => { a(t); b(); };
        /// Returns a function which appends an additional function (such as logging).
        public static Action<T> AndAlso<T>(this Action<T> a, Action<T> b)
        => (t) => { a(t); b(t); };
        /// Returns a function which appends an additional function (such as logging).
        public static Func<T> AndAlso<T>(this Func<T> a, Action b)
        => () => { T t = a(); b(); return t; };
        /// Returns a function which appends an additional function (such as logging).
        public static Func<TIn, TOut> AndAlso<TIn, TOut>(this Func<TIn, TOut> a, Action b)
        => (tin) => { TOut tout = a(tin); b(); return tout; };
        /// Returns a function which appends an additional function (such as logging).
        public static Func<TIn, TOut> AndAlso<TIn, TOut>(this Func<TIn, TOut> a, Action<TIn> b)
        => (tin) => { TOut tout = a(tin); b(tin); return tout; };

        /// Returns a function which appends an additional function (such as logging).
        /// This isn't quite a pipe: the returned value is that of A, NOT B (even though B gets a crack)!
        public static Func<TIn, TOut> AndAlsoOutput<TIn, TOut>(this Func<TIn, TOut> a, Action<TOut> b)
        => (tin) => { TOut tout = a(tin); b(tout); return tout; };
        /// Returns a function which appends an additional function (such as logging).
        /// This isn't quite a pipe: the returned value is that of A, NOT B (even though B gets a crack)!
        public static Func<TIn, TOut> AndAlsoOutput<TIn, TOut>(this Func<TIn, TOut> a, Action<TIn, TOut> b)
        => (tin) => { TOut tout = a(tin); b(tin, tout); return tout; };
    }
}
