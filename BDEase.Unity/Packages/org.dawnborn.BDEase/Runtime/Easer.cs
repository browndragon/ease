using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BDEase
{
    [Serializable]
    public struct Easer
    {
        public Func<float, float> Func;
        public AnimationCurve Curve;
        public Easings.Ease EaseEnum;

        public float Ease(float f)
        {
            if (Func != null) return Func(f);
            if (Curve != null && Curve.length > 0) return Curve.Evaluate(f);
            return EaseEnum.Apply(f);
        }
    }
}
