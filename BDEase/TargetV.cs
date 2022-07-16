using System;

namespace BDEase
{
    /// Transforms a difference in position into an ideal velocity.
    /// Assumes we want to ramp up (=VByT ease) for `StartupT` sec, and ramp down (=VByX ease) for `BrakingX` units.
    /// In between, assumes want full VScale speed.
    /// This can be used sort of like a tween directly using velocity instead of time.
    [Serializable]
    public struct TargetV<T>
    {
        public readonly static TargetV<T> Default = new(12f, .25f, 1f, default, default);

        public float VScale;
        /// Target velocity during startup (min(startupT, brakingX)).
        public float StartupT;
        /// Distance from target position to begin braking using ease.
        public float BrakingX;
        /// Ease Target V by elapsed time.
        public Func<float, float> VByT;
        /// Ease Target V by distance to target.
        public Func<float, float> VByX;
        public TargetV(float vScale, float startupT, float brakingX, Func<float, float> vByT = default, Func<float, float> vByX = default)
        {
            VScale = vScale;
            StartupT = startupT;
            BrakingX = brakingX;
            VByT = vByT;
            VByX = vByX;
        }

        /// Returns an ideal velocity based on time elapsed & position difference.
        public T Apply(float elapsed, T x)
        {
            IArith<T> arith = Arith<T>.Default;
            float lenX = arith.Length(x);
            if (lenX < Arith.Epsilon) return default;
            float vMag = VScale * Math.Min(
                VByT.ClampInvoke(elapsed / StartupT),
                VByX.ClampInvoke(lenX / BrakingX)
            );
            return arith.Scale(vMag / lenX, x);
        }
    }
}