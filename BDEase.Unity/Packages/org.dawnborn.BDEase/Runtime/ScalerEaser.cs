using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BDEase
{
    /// Combines an easer with scaling & clamping on input & output.
    /// Thus, it doesn't run on [0->1] but on all ints.
    [Serializable]
    public struct ScalerEaser
    {
        public static readonly ScalerEaser Default = new()
        {
            Easer = new(),
            InScale = 1f,
            OutScale = 1f,
        };
        public Easer Easer;
        public float InScale;
        public float OutScale;

        public float Apply(float f) => OutScale * Easer.ClampInvoke(f / InScale);
    }
}
