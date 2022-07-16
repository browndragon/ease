using System;

namespace BDEase
{
    using static Arith;
    /// An ease maps [0f,1f]->[0f-delta,1f+delta] (sometimes they might squeak outside of the range).
    // https://github.com/ai/easings.net/blob/master/src/easings/easingsFunctions.ts
    public static class Easings
    {
        public enum Ease
        {
            Linear = default,
            InQuad,
            OutQuad,
            InOutQuad,
            InCubic,
            OutCubic,
            InOutCubic,
            InQuart,
            OutQuart,
            InOutQuart,
            InQuint,
            OutQuint,
            InOutQuint,
            InSine,
            OutSine,
            InOutSine,
            InExpo,
            OutExpo,
            InOutExpo,
            InCirc,
            OutCirc,
            InOutCirc,
            InBack,
            OutBack,
            InOutBack,
            InElastic,
            OutElastic,
            InOutElastic,
            InBounce,
            OutBounce,
            InOutBounce
        }

        public static float Apply(this Ease thiz, float @in) => thiz switch
        {
            Ease.Linear => EaseLinear(@in),
            Ease.InQuad => EaseInQuad(@in),
            Ease.OutQuad => EaseOutQuad(@in),
            Ease.InOutQuad => EaseInOutQuad(@in),
            Ease.InCubic => EaseInCubic(@in),
            Ease.OutCubic => EaseOutCubic(@in),
            Ease.InOutCubic => EaseInOutCubic(@in),
            Ease.InQuart => EaseInQuart(@in),
            Ease.OutQuart => EaseOutQuart(@in),
            Ease.InOutQuart => EaseInOutQuart(@in),
            Ease.InQuint => EaseInQuint(@in),
            Ease.OutQuint => EaseOutQuint(@in),
            Ease.InOutQuint => EaseInOutQuint(@in),
            Ease.InSine => EaseInSine(@in),
            Ease.OutSine => EaseOutSine(@in),
            Ease.InOutSine => EaseInOutSine(@in),
            Ease.InExpo => EaseInExpo(@in),
            Ease.OutExpo => EaseOutExpo(@in),
            Ease.InOutExpo => EaseInOutExpo(@in),
            Ease.InCirc => EaseInCirc(@in),
            Ease.OutCirc => EaseOutCirc(@in),
            Ease.InOutCirc => EaseInOutCirc(@in),
            Ease.InBack => EaseInBack(@in),
            Ease.OutBack => EaseOutBack(@in),
            Ease.InOutBack => EaseInOutBack(@in),
            Ease.InElastic => EaseInElastic(@in),
            Ease.OutElastic => EaseOutElastic(@in),
            Ease.InOutElastic => EaseInOutElastic(@in),
            Ease.InBounce => EaseInBounce(@in),
            Ease.OutBounce => EaseOutBounce(@in),
            Ease.InOutBounce => EaseInOutBounce(@in),
            _ => throw new NotImplementedException($"Unrecognized {thiz}"),
        };
        public static float ClampInvoke(this Func<float, float> thiz, float value)
        {
            value = Clamp01(value);
            value = thiz?.Invoke(value) ?? value;
            return value;
        }

        public static class FuncCache
        {
            public static readonly Func<float, float> Linear = EaseLinear;
            public static readonly Func<float, float> InQuad = EaseInQuad;
            public static readonly Func<float, float> OutQuad = EaseOutQuad;
            public static readonly Func<float, float> InOutQuad = EaseInOutQuad;
            public static readonly Func<float, float> InCubic = EaseInCubic;
            public static readonly Func<float, float> OutCubic = EaseOutCubic;
            public static readonly Func<float, float> InOutCubic = EaseInOutCubic;
            public static readonly Func<float, float> InQuart = EaseInQuart;
            public static readonly Func<float, float> OutQuart = EaseOutQuart;
            public static readonly Func<float, float> InOutQuart = EaseInOutQuart;
            public static readonly Func<float, float> InQuint = EaseInQuint;
            public static readonly Func<float, float> OutQuint = EaseOutQuint;
            public static readonly Func<float, float> InOutQuint = EaseInOutQuint;
            public static readonly Func<float, float> InSine = EaseInSine;
            public static readonly Func<float, float> OutSine = EaseOutSine;
            public static readonly Func<float, float> InOutSine = EaseInOutSine;
            public static readonly Func<float, float> InExpo = EaseInExpo;
            public static readonly Func<float, float> OutExpo = EaseOutExpo;
            public static readonly Func<float, float> InOutExpo = EaseInOutExpo;
            public static readonly Func<float, float> InCirc = EaseInCirc;
            public static readonly Func<float, float> OutCirc = EaseOutCirc;
            public static readonly Func<float, float> InOutCirc = EaseInOutCirc;
            public static readonly Func<float, float> InBack = EaseInBack;
            public static readonly Func<float, float> OutBack = EaseOutBack;
            public static readonly Func<float, float> InOutBack = EaseInOutBack;
            public static readonly Func<float, float> InElastic = EaseInElastic;
            public static readonly Func<float, float> OutElastic = EaseOutElastic;
            public static readonly Func<float, float> InOutElastic = EaseInOutElastic;
            public static readonly Func<float, float> InBounce = EaseInBounce;
            public static readonly Func<float, float> OutBounce = EaseOutBounce;
            public static readonly Func<float, float> InOutBounce = EaseInOutBounce;
        }

        const float c1 = 1.70158f;
        const float c2 = c1 * 1.525f;
        const float c3 = c1 + 1;
        const float c4 = TAU / 3f;
        const float c5 = TAU / 4.5f;

        /// Flips X around x=.5; good for "yoyo".
        public static Func<float, float> FlipX(Func<float, float> ease = default)
        {
            ease ??= EaseLinear;
            return (x) => ease(1 - x);
        }
        /// Flips Y around y=.5; good for quick & dirty switch ease in<->ease out.
        public static Func<float, float> FlipY(Func<float, float> ease = default)
        {
            ease ??= EaseLinear;
            return (x) => 1 - ease(x);
        }

        static float BounceOut(float x)
        {
            const float n1 = 7.5625f;
            const float d1 = 2.75f;

            if (x < 1 / d1)
            {
                return n1 * x * x;
            }
            else if (x < 2 / d1)
            {
                return n1 * (x -= 1.5f / d1) * x + 0.75f;
            }
            else if (x < 2.5 / d1)
            {
                return n1 * (x -= 2.25f / d1) * x + 0.9375f;
            }
            else
            {
                return n1 * (x -= 2.625f / d1) * x + 0.984375f;
            }
        }

        /// AKA identity, etc.
        public static float EaseLinear(float x) => x;
        public static float EaseInQuad(float x) => x * x;
        public static float EaseOutQuad(float x) => 1 - Pow(1 - x, 2);
        public static float EaseInOutQuad(float x) => x < 0.5f ? 2 * x * x : 1 - Pow(-2 * x + 2, 2) / 2;
        public static float EaseInCubic(float x) => Pow(x, 3);
        public static float EaseOutCubic(float x) => 1f - Pow(1 - x, 3);
        public static float EaseInOutCubic(float x) => x < 0.5f ? (4 * Pow(x, 3)) : (1 - Pow(-2 * x + 2, 3) / 2);
        public static float EaseInQuart(float x) => Pow(x, 4f);
        public static float EaseOutQuart(float x) => 1f - Pow(1 - x, 4);
        public static float EaseInOutQuart(float x) => x < 0.5 ? 8 * Pow(x, 4f) : 1 - Pow(-2 * x + 2, 4) / 2;
        public static float EaseInQuint(float x) => Pow(x, 5f);
        public static float EaseOutQuint(float x) => 1f - Pow(1f - x, 5f);
        public static float EaseInOutQuint(float x) => x < 0.5f ? 16f * Pow(x, 5f) : 1f - Pow(-2f * x + 2f, 5f) / 2f;
        public static float EaseInSine(float x) => 1f - Cos(HALF_PI * x);
        public static float EaseOutSine(float x) => Sin(HALF_PI * x);
        public static float EaseInOutSine(float x) => -(Cos(PI * x) - 1) / 2f;
        public static float EaseInExpo(float x) => x == 0f ? 0f : Pow(2f, 10f * x - 10f);
        public static float EaseOutExpo(float x) => x == 1f ? 1f : 1f - Pow(2f, -10f * x);
        public static float EaseInOutExpo(float x) => x switch
        {
            0f => 0f,
            1f => 1f,
            _ => x < .5f
                ? Pow(2f, 20f * x - 10f) / 20f
                : (2f - Pow(2f, -20f * x + 10f) / 2f)
        };
        public static float EaseInCirc(float x) => 1 - Sqrt(1f - Pow(x, 2f));
        public static float EaseOutCirc(float x) => Sqrt(1f - Pow(x - 1f, 2f));
        public static float EaseInOutCirc(float x) => x < .5f
            ? (1 - Sqrt(1f - Pow(x, 2f))) / 2f
            : (Sqrt(1f - Pow(-2f * x + 2f, 2f)) + 1f) / 2f;

        public static float EaseInBack(float x) => c3 * Pow(x, 3f) - c1 * Pow(x, 2f);
        public static float EaseOutBack(float x) => 1f + c3 * Pow(x - 1f, 3f) - c1 * Pow(x - 1f, 2f);
        public static float EaseInOutBack(float x) => x < 0.5
                ? (Pow(2 * x, 2) * ((c2 + 1) * 2 * x - c2)) / 2
                : (Pow(2 * x - 2, 2) * ((c2 + 1) * (x * 2 - 2) + c2) + 2) / 2;
        public static float EaseInElastic(float x) => x switch
        {
            0f => 0f,
            1f => 1f,
            _ => -Pow(2, 10 * x - 10) * Sin((x * 10 - 10.75f) * c4)
        };
        public static float EaseOutElastic(float x) => x switch
        {
            0f => 0f,
            1f => 1f,
            _ => Pow(2, -10 * x) * Sin((x * 10 - 0.75f) * c4) + 1
        };
        public static float EaseInOutElastic(float x) => x switch
        {
            0f => 0f,
            1f => 1f,
            _ => x < .5f
                ? -(Pow(2, 20 * x - 10) * Sin((20 * x - 11.125f) * c5)) / 2
                : ((Pow(2, -20 * x + 10) * Sin((20 * x - 11.125f) * c5)) / 2 + 1)
        };
        public static float EaseInBounce(float x) => 1 - BounceOut(1 - x);
        public static float EaseOutBounce(float x) => BounceOut(x);
        public static float EaseInOutBounce(float x) => x < .5f
                ? (1 - BounceOut(1 - 2 * x)) / 2
                : (1 + BounceOut(2 * x - 1)) / 2;
    }
}