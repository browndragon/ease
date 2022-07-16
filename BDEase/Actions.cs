using System;

namespace BDEase
{
    /// Pipe actions: Creates size(1) get/set pairs to pass values around.
    /// This is most useful for quickly creating cancellation tokens.
    public static class Actions
    {
        /// Calls underlying `thiz` with `transform(input)`.
        public static Action<TNew> AsAction<TOld, TNew>(this Action<TOld> thiz, Func<TNew, TOld> transform)
        => (tnew) => thiz(transform(tnew));
        /// Calls underlying `thiz` with `value`.
        public static Action AsAction<T>(this Action<T> thiz, T value)
        => () => thiz(value);
        /// Calls underlying `thiz` with `true`.
        public static Action AsAction(this Action<bool> thiz) => thiz.AsAction(true);

        /// Transform outputs on return (on each call).
        public static Func<TNew> WithOutputs<TOld, TNew>(this Func<TOld> thiz, Func<TOld, TNew> transform)
        => () => transform(thiz());
        /// Transform outputs on return (on each call).
        public static Func<TNew> WithOutputs<TNew>(this Func<bool> thiz, TNew onTrue, TNew onFalse = default)
        => () => thiz() ? onTrue : onFalse;

        /// Transforms a function(input, prev)=> output into a pair (set(input),get()=>output=prev=func(input, prev)).
        /// Operation performed on Set (gets are nonmutating.)
        /// (this happens to return the setter & outparam the getter; see MakeGetter for the other of the pair).
        /// This also serves as a cache of the previous value.
        public static Action<TIn> MakeSetter<TIn, TOut>(this Func<TIn, TOut, TOut> thiz, out Func<TOut> getter, TOut initial = default)
        {
            TOut has = initial;
            getter = () => has;
            return (tin) => has = thiz(tin, has);
        }
        /// As MakeSetter but returns/outparams the other of the pair.
        public static Func<TOut> MakeGetter<TIn, TOut>(this Func<TIn, TOut, TOut> thiz, out Action<TIn> setter, TOut initial = default)
        {
            setter = thiz.MakeSetter(out var getter, initial);
            return getter;
        }
        /// As MakeSetter but the transforming function ignores the previous value.
        public static Action<TIn> MakeSetter<TIn, TOut>(this Func<TIn, TOut> thiz, out Func<TOut> getter, TOut initial = default)
        => ((Func<TIn, TOut, TOut>)((tin, _) => thiz(tin))).MakeSetter(out getter, initial);
        /// As MakeSetter but returns/outparams the other of the pair.
        public static Func<TOut> MakeGetter<TIn, TOut>(this Func<TIn, TOut> thiz, out Action<TIn> setter, TOut initial = default)
        {
            setter = thiz.MakeSetter(out var getter, initial);
            return getter;
        }
    }
}
