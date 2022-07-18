using System;
using System.Collections;
using System.Collections.Generic;
using BDEase;
using UnityEngine;
using UnityEngine.Events;

namespace BDEase
{
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class Accelerator : MonoBehaviour
    {
        static Accelerator()
        {
            Lerp.Initialize();
            Arith = Arith<Vector2>.Default;
        }
        static readonly IArith<Vector2> Arith;

        public TargetV VByTX = TargetV.Default;
        public bool UseAByV = true;
        public PID<Vector2> AByV = PID<Vector2>.Default;
        public float Timeout = 15f;

        Rigidbody2D body;
        Rigidbody2D Body => body ??= GetComponent<Rigidbody2D>();

        float startT = float.NaN;
        protected float elapsedStart => Time.fixedTime - startT;
        float convergeT = float.NaN;
        protected float elapsedConverge => Time.fixedTime - convergeT;
        Vector2 lastError = default, cumError = default;

        protected Vector2 targetX;
        protected Vector2 errorX;
        protected Vector2 targetV;
        protected Vector2 errorV;
        protected Vector2 targetA;

        public virtual void Restart()
        {
            convergeT = float.NaN;
            startT = Time.fixedTime;
            lastError = default;
            cumError = default;
        }
        public virtual void BeginConverge() => convergeT = Time.fixedTime;
        public virtual void Abort() => convergeT = float.NegativeInfinity;

        protected virtual void BeforeStep() { }
        protected virtual void AfterStep() { }
        protected virtual void OnComplete() { }

        void CallComplete()
        {
            OnComplete();
            startT = float.NaN;
            convergeT = float.NaN;
            lastError = cumError = default;
            return;
        }

        protected void FixedUpdate()
        {
            if (float.IsNaN(startT)) return;
            float elapsedConverge = this.elapsedConverge;
            if (elapsedConverge >= Timeout)
            {
                CallComplete();
                return;
            }

            BeforeStep();
            errorX = targetX - Body.position;
            targetV = VByTX.Apply(elapsedStart, errorX);
            errorV = targetV - Body.velocity;
            if (elapsedConverge > 0f)
            {
                if (Arith.Approximately(errorX) && Arith.Approximately(errorV))
                {
                    Body.velocity = targetV;
                    Body.position = targetX;
                    CallComplete();
                    return;
                }
            }
            if (!UseAByV)
            {
                AfterStep();
                Body.velocity = targetV;
                return;
            }
            targetA = AByV.Apply(Time.fixedDeltaTime, errorV, ref lastError, ref cumError);
            AfterStep();
            Body.AddForce(Body.mass * targetA);
        }
    }
}