using System;

namespace BDEase
{
    /// Produces a https://en.wikipedia.org/wiki/PID_controller .
    /// This is particularly helpful for combining something like a tween with physics,
    /// where the tween sets an ideal position (or velocity), while the output of the PID is the requested force
    /// to be applied to the body.
    [Serializable]
    public struct PID<T>
    {
        // Construction note: PID<Vector2> depends _out_ on BDEaseUnity defining V2Arith,
        // and so in terms of initialization order, we need to try to not grab a reference to that class
        // before they've had a chance to reference Arith.Initialize() & UnityArith.Initialize().
        // Since this is likely a field on a monobehaviour and will use our own static Default, that means
        // waiting to define Arith until, say, their class' init time.
        public readonly static PID<T> Default = new(12f, 2f, 2f, 36f, 36f);
        /// The overall gain for this PID controller.
        public float Gain;
        /// The time the integral term takes to correct for error.
        public float ITime;
        /// The time the derivative term takes to correct for error.
        public float DTime;
        /// Input error clamp
        public float MaxIn;
        /// Output response clamp
        public float MaxOut;

        public PID(float gain, float iTime, float dTime, float maxIn, float maxOut)
        {
            Gain = gain;
            ITime = iTime;
            DTime = dTime;
            MaxIn = maxIn;
            MaxOut = maxOut;
        }

        /// dT: Timestep (usually just Time.fixedDeltaTime).
        /// Error: target - actual ("offset" in, say, position).
        /// Might be capped (with implications for cumulativeError)!
        /// LastError: Maintained at error; used to calculate the derivative.
        /// CumulativeError: Internally maintained sum of observed errors scaled by time.
        /// Returns: The output variable (weighted by error, delta, and cumulative error).
        public T Apply(float dT, T error, ref T lastError, ref T cumulativeError)
        {
            IArith<T> arith = Arith<T>.Default;
            error = arith.Clamp(error, MaxIn);

            // Antiwindup:
            // If error-dot-last is negative, then we need to turn >90deg; enough of a change to wipe cumulative error.
            if (arith.Dot(error, lastError) <= 0f) cumulativeError = default;

            // Derivative: Scale the change in error by DTime/dT.
            T dFactor = arith.Difference(error, lastError);
            dFactor = arith.Scale(DTime / dT, dFactor);
            lastError = error;
            // Integral: update the cumulative error by error*dT; scale by ITime.
            cumulativeError = arith.Add(cumulativeError, arith.Scale(dT, error));
            T iFactor = arith.Scale(1f / ITime, cumulativeError);

            error = arith.Add(error, dFactor);
            error = arith.Add(error, iFactor);
            T res = arith.Scale(Gain, error);

            res = arith.Clamp(res, MaxOut);
            return res;
        }

        /// Returns a correction function given an error function.
        /// This function is keyed in dT.
        public Func<T, T> ApplyFunc(float fixedTimestep)
        {
            IArith<T> arith = Arith<T>.Default;
            PID<T> thiz = this;
            T lastError = default;
            T cumulativeError = default;
            return (t) => thiz.Apply(fixedTimestep, t, ref lastError, ref cumulativeError);
        }
    }
}