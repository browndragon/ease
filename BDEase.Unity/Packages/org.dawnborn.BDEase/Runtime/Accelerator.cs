using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using BDEase;
using BDUtil;
using UnityEngine;
using UnityEngine.Events;

namespace BDEase
{
    [Serializable]
    public struct Accelerator
    {
        public static Accelerator Default = new(StartTargetCurve.Default, 64f, PID.Default);
        public StartTargetCurve StartTargetCurve;
        public float Velocity;
        public PID PID;
        public float ConvergeT;
        public float Tolerance;

        public Accelerator(StartTargetCurve targetV, float velocity, PID pid, float convergeT = 5f, float tolerance = 1e-2f)
        {
            StartTargetCurve = targetV;
            PID = pid;
            Velocity = velocity;
            ConvergeT = convergeT;
            Tolerance = tolerance;
        }
        [Serializable]
        public struct State<T>
        {
            static State() => UnityAriths.Initialize();
            public readonly static IArith<T> arith = Arith<T>.Default;
            public float ElapsedT;
            public float ConvergingT;
            public T PrevError;
            public T CumError;

            public T TargetX;
            public T ActualX;
            public T ErrorX;
            public void SetErrorX() => ErrorX = arith.Difference(TargetX, ActualX);

            public T TargetV;
            public T ActualV;
            public T ErrorV;
            public void SetErrorV() => ErrorV = arith.Difference(TargetV, ActualV);

            public bool IsConverged(float tolerance) => arith.Approximately(ErrorX, tolerance) && arith.Approximately(ErrorV, tolerance);

            public T TargetA;

            public void Restart()
            {
                ElapsedT = 0f;
                ConvergingT = float.NaN;
                PrevError = CumError = default;
            }
            public void Stop()
            {
                Restart();
                ElapsedT = float.NaN;
            }
            public bool IsStopped => !float.IsFinite(ElapsedT);
            public void StartConverge() => ConvergingT = 0f;
            public void Abort() => ConvergingT = float.NegativeInfinity;
        }
        public enum Exit
        {
            Run = default,
            Timeout,
            Converged
        }
        public Exit Apply<T>(float dT, ref State<T> state)
        {
            state.ElapsedT += dT;
            state.ConvergingT += dT;
            state.TargetV = default;
            state.TargetA = default;
            if (!(state.ElapsedT >= 0f)) return Exit.Timeout;
            if (state.ConvergingT > ConvergeT) return Exit.Timeout;
            state.SetErrorX();
            state.TargetV = State<T>.arith.Scale(Velocity, StartTargetCurve.Apply(state.ElapsedT, state.ErrorX));
            state.SetErrorV();
            if (state.ConvergingT >= 0f && state.IsConverged(Tolerance)) return Exit.Converged;
            state.TargetA = PID.Apply(dT, state.ErrorV, ref state.PrevError, ref state.CumError);
            return Exit.Run;
        }
    }
}