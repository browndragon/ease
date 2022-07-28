# Ease

> Warning! This is soft-deprecated; the important parts have been brought forward into https://github.com/browndragon/util .
> I'll be deleting it shortly.
> The core insight: everything this tried to do worked better by hand. All that's left is a little bit of easing, which I just rolled into BDUtil.Math.

Unity coroutine, tween, ease, convergence lib. This is a prototype; expect changes.

I highly recommend Demigiant's DOTween library if you want _good_ tweening.

This was written so that I could work through some prototypes. I don't really recommend it.
In particular, I wanted a way to say "Move this body at velocity Y to point Z".
That's not quite a tween; it's not time bounded, and should work even in the presence of drag and other forces.
And it turns out that if you want to respect physics, it's actually somewhat tricky to get right (see the Accelerator class).

Provides C# implementations of standard easing functions, and Unity-compatible structures to choose between easing functions or AnimationCurves.

Provides `Registry` which maps `Type`->`object` (not necessarily & indeed probably not objects of that type; 
consider `EqualityComparer<T>` where `typeof(T)` makes a better key than `typeof(IEqualityComparer<T>)` for `IEqualityComparer<T>` instances).
Also provides `Registry.ProvidesAttribute(typeof(T))`.
`registry.RegisterByAttribute<TPA>` and `registry.RegisterAssemblyContaining<TPA>` can be used to find types `TR` annotated with some `TPA:ProvidesAttribute`
and invoke their no-arg `new()`, storing `T`->`new TR()`.
This should be invoked from entry point code as soon as is possible; if you don't register something, expect NPEs.
This should probably move into BDUtil!

Provides generic `Arith<T>` classes which provide arithmetic analogously to `EqualityComparer<T>`.
This uses `Registry` on its own type, `Arith.DefaultFor`, which binds based on the underlying type.
Later binds of `DefaultFor` replace earlier (but: did you get all of the references before replacing?).
This defines algebras that let you use floats, VectorNs, colors, etc equivalently with algorithms like...

Provides `TargetV`, which similar to an animation curve maps constant-scaled time elapsed & constant-scaled distance from target to a constant-scaled output.
It supports time-based easing at startup, and space-based easing at braking distances (and simply takes the min of both).
It can be used fairly simply in a coroutine.
Combine with a PID when you want it to work **physically**: using Proportional, Integral and Derivative error (and dT timeslice), produces an update.
For instance, use a TargetV to set a target speed, and a PID to match acceleration to hit that speed.
This is a lossy, messy process; many tuning parameters, such indirection.

Code is split into the `ease/BDEase` directory (pure math) and `ease/BDEase.Unity` (+unity3d types and bindings).
You must `dotnet build` `ease/ease.sln` to copy changes from `ease/BDEase` into `ease/BDEase.Unity/....dll`

This doesn't take any additional dependencies: given Unity's attitude towards dep management vs the dep management this code would take, it seems...
fraught.
