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
    public struct TargetV
    {
        static TargetV() => Lerp.Initialize();
        public readonly static TargetV Default = new(12f, .25f, 2f);

        [Tooltip("Output Speed scaling factor")]
        public float VScale;
        [Tooltip("Time elapsed factor for VByT curve")]
        public float StartupT;
        [Tooltip("Space from target factor for VByX curve")]
        public float BrakingX;
        /// Ease Target V by elapsed time.
        public Easer VByT;
        /// Ease Target V by distance to target.
        public Easer VByX;
        [Tooltip("Min space to return default")]
        public float XMin;
        [Tooltip("Min output to return default")]
        public float VMin;
        public TargetV(float vScale, float startupT, float brakingX, Easer vByT = default, Easer vByX = default, float xMin = Arith.Epsilon, float vMin = Arith.Epsilon)
        {
            VScale = vScale;
            StartupT = startupT;
            BrakingX = brakingX;
            VByT = vByT;
            VByX = vByX;
            XMin = xMin;
            VMin = vMin;
        }

        /// Returns an ideal velocity (or whatever!) based on time elapsed & position difference target-current.
        /// Returns default when the error or position are below some thresholds.
        public T Apply<T>(float elapsed, T errorX)
        {
            IArith<T> arith = Arith<T>.Default;
            float lenX = arith.Length(errorX);
            /// Scaling won't help, we're just SO close.
            if (lenX <= XMin) return default;
            float vMag = VScale * Math.Min(
                VByT.ClampInvoke(elapsed / StartupT),
                VByX.ClampInvoke(lenX / BrakingX)
            );
            if (vMag <= VMin) return default;
            return arith.Scale(vMag / lenX, errorX);
        }
    }
}