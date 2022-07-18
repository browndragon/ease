using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BDEase
{
    /// Serializable representation of a specific easing function.
    /// It supports arbitrary functions,
    /// It's not polymorphic: that way, Unity inspector "just works".
    [Serializable]
    public struct Easer
    {
        public Func<float, float> Func;
        public AnimationCurve Curve;
        public Easings.Enum Enum;

        public Func<float, float> AsEase
        {
            get
            {
                if (Func != null) return Func;
                if (Curve != null && Curve.length > 0) return Curve.Evaluate;
                return Enum.AsEase();
            }
        }
        public float Ease(float f)
        {
            if (Func != null) return Func(f);
            if (Curve != null && Curve.length > 0) return Curve.Evaluate(f);
            return Enum.Apply(f);
        }
        public float ClampInvoke(float f) => Ease(Arith.Clamp01(f));
    }
}
