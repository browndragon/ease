using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BDEase
{
    /// A little funny: identify clocks by the yield instruction that sets their cadence.
    /// Each clock can thus return the delta since its previous yield.
    /// Wacky, right?
    public static class YieldInstructions
    {
        static YieldInstructions() => UnityAriths.Initialize();
        public static YieldInstruction Update = null;
        public static WaitForFixedUpdate FixedUpdate = new();

        public static float GetDelta(this YieldInstruction thiz)
        {
            if (thiz is WaitForFixedUpdate) return Time.fixedDeltaTime;
            return Time.deltaTime;
        }
        /// Turns `eachTick` into a coroutine executed on each... tick...
        public static IEnumerator GetCoroutine(this YieldInstruction thiz, Func<float, bool> eachTick)
        {
            do { yield return thiz; } while (eachTick(thiz.GetDelta()));
        }
        /// Turns `eachTick` into a coroutine executed on each... tick...
        public static IEnumerator GetCoroutine(this YieldInstruction thiz, Func<bool> eachTick)
        {
            do { yield return thiz; } while (eachTick());
        }
    }
}