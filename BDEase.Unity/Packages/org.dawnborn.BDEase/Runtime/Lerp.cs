using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BDEase
{
    /// Runs a function of ratio-of-elapsed-time on every tick until completed.
    [Serializable]
    public struct Lerp<T> : IEnumerable<YieldInstruction>
    {
        public MonoBehaviour DefaultCoroutineStarter;
        public YieldInstruction YieldInstruction;
        public bool IsFixed;
        public Lerp<T> SetFixed(bool @fixed)
        {
            YieldInstruction = @fixed ? new WaitForFixedUpdate() : null;
            IsFixed = @fixed;
            return this;
        }
        public float Now => IsFixed ? Time.fixedTime : Time.time;
        public float Duration;

        /// Cancel token.
        public Func<Lerp.Run> GetRun;
        public Lerp<T> WithCancelToken(out Action cancel, Lerp.Run cancelState = Lerp.Run.End)
        {
            GetRun = Actions.MakeGetter(out cancel).Piped(Lerp.Run.Run, cancelState);
            return this;
        }

        /// Strictly lerp-y semantics: Action(Lerp(Start, End, ease(elapsed/duration))).
        public Func<float, float> Ease;
        public T Start;
        public Action<T> Action;
        public T End;

        public event Action OnComplete;
        public Lerp<T> AddOnComplete(Action onComplete) { OnComplete += onComplete; return this; }

        /// Creates a function which performs a lerp at the indicated value.
        public IEnumerator<YieldInstruction> GetEnumerator()
        {
            IArith<T> arith = Arith<T>.Default;
            Lerp.Run run = default;
            for (float start = Now, elapsed = 0f;
                elapsed < Duration;
                elapsed = Now - start
            )
            {
                run = GetRun?.Invoke() ?? Lerp.Run.Run;
                if (run != Lerp.Run.Run) break;
                Action(arith.LerpUnclamped(Start, End, Ease.ClampInvoke(elapsed / Duration)));
                yield return YieldInstruction;
            }
            switch (run)
            {
                case Lerp.Run.Run:  // Fallthrough.
                case Lerp.Run.End:
                    Action(End);
                    OnComplete?.Invoke();
                    yield break;
                case Lerp.Run.Start:
                    Action(Start);
                    OnComplete?.Invoke();
                    yield break;
                case Lerp.Run.Abort:
                    OnComplete?.Invoke();
                    yield break;
            }
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void StartCoroutine() => DefaultCoroutineStarter.StartCoroutine(this.GetEnumerator());
        public override string ToString() => $"DCS:{DefaultCoroutineStarter} YI:{YieldInstruction} Fixed?:{IsFixed} Dur:{Duration} GR:{GetRun} Ease:{Ease} Start:{Start} Action:{Action} End:{End} OnC:{OnComplete}";
    }
    public static class Lerp
    {
        public static Lerp<T> MakeLerp<T>(this MonoBehaviour thiz, T start, T end, Action<T> action, float duration = .5f, Func<float, float> ease = default) => new()
        {
            DefaultCoroutineStarter = thiz,
            Start = start,
            Action = action,
            End = end,
            Duration = duration,
            Ease = ease ?? Easings.Linear,
        };
        public enum Run
        {
            Run = default,
            Start,
            End,
            Abort
        }
        static Lerp() => UnityAriths.Initialize();
    }
}
