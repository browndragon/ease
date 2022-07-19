using System;
using System.Collections;
using UnityEngine;

namespace BDEase
{
    /// Transforms a difference in position & time-elapsed into a target velocity (or whatever!).
    /// Assumes we want to ramp up (=VByT ease) for `StartupT` sec, and ramp down (=VByX ease) for `BrakingX` units.
    /// In between, assumes want full VScale speed.
    /// This can be used sort of like a tween directly using velocity instead of time.
    /// It *doesn't* have any state, other than elapsed time & physical position!
    [Serializable]
    public struct StartTargetCurve
    {
        static StartTargetCurve() => UnityAriths.Initialize();
        public readonly static StartTargetCurve Default = new(.25f, 2f);

        [Tooltip("Time elapsed factor for VByT curve")]
        public float StartupT;
        [Tooltip("Space from target factor for VByX curve")]
        public float BrakingX;
        /// Ease Target V by elapsed time.
        public Easer VByT;
        /// Ease Target V by distance to target.
        public Easer VByX;

        public StartTargetCurve(float startupT, float brakingX, Easer vByT = default, Easer vByX = default)
        {
            StartupT = startupT;
            BrakingX = brakingX;
            VByT = vByT;
            VByX = vByX;
        }

        /// Returns an ideal velocity (or whatever!) based on time elapsed & position difference target-current.
        /// Returns default when the error or position are below some thresholds.
        public T Apply<T>(float elapsed, T errorX)
        {
            IArith<T> arith = Arith<T>.Default;
            float lenX = arith.Length(errorX);
            /// If the lenX is too small, the scaling below will go poorly.
            /// So abort now; with tiny error comes nil solution.
            if (lenX < Arith.Epsilon) return default;
            float vMag = Math.Min(
                StartupT > 0f ? VByT.ClampInvoke(elapsed / StartupT) : 1f,
                BrakingX > 0f ? VByX.ClampInvoke(lenX / BrakingX) : 1f
            );
            return arith.Scale(vMag / lenX, errorX);
        }
    }
}