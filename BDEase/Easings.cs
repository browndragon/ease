using System;

namespace BDEase
{
    using static Arith;
    /// An Ease. maps [0f,1f]->[0f-delta,1f+delta] (sometimes they might squeak outside of the range).
    public static class Easings
    {
        public static float ClampInvoke(this Func<float, float> thiz, float value)
        {
            value = Clamp01(value);
            value = thiz?.Invoke(value) ?? value;
            return value;
        }

        public enum Enum
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

        public static float Apply(this Enum thiz, float @in) => thiz switch
        {
            Enum.Linear => Ease.Linear(@in),
            Enum.InQuad => Ease.InQuad(@in),
            Enum.OutQuad => Ease.OutQuad(@in),
            Enum.InOutQuad => Ease.InOutQuad(@in),
            Enum.InCubic => Ease.InCubic(@in),
            Enum.OutCubic => Ease.OutCubic(@in),
            Enum.InOutCubic => Ease.InOutCubic(@in),
            Enum.InQuart => Ease.InQuart(@in),
            Enum.OutQuart => Ease.OutQuart(@in),
            Enum.InOutQuart => Ease.InOutQuart(@in),
            Enum.InQuint => Ease.InQuint(@in),
            Enum.OutQuint => Ease.OutQuint(@in),
            Enum.InOutQuint => Ease.InOutQuint(@in),
            Enum.InSine => Ease.InSine(@in),
            Enum.OutSine => Ease.OutSine(@in),
            Enum.InOutSine => Ease.InOutSine(@in),
            Enum.InExpo => Ease.InExpo(@in),
            Enum.OutExpo => Ease.OutExpo(@in),
            Enum.InOutExpo => Ease.InOutExpo(@in),
            Enum.InCirc => Ease.InCirc(@in),
            Enum.OutCirc => Ease.OutCirc(@in),
            Enum.InOutCirc => Ease.InOutCirc(@in),
            Enum.InBack => Ease.InBack(@in),
            Enum.OutBack => Ease.OutBack(@in),
            Enum.InOutBack => Ease.InOutBack(@in),
            Enum.InElastic => Ease.InElastic(@in),
            Enum.OutElastic => Ease.OutElastic(@in),
            Enum.InOutElastic => Ease.InOutElastic(@in),
            Enum.InBounce => Ease.InBounce(@in),
            Enum.OutBounce => Ease.OutBounce(@in),
            Enum.InOutBounce => Ease.InOutBounce(@in),
            _ => throw new NotImplementedException($"Unrecognized {thiz}"),
        };

        public static readonly Func<float, float> Linear = Ease.Linear;
        public static readonly Func<float, float> InQuad = Ease.InQuad;
        public static readonly Func<float, float> OutQuad = Ease.OutQuad;
        public static readonly Func<float, float> InOutQuad = Ease.InOutQuad;
        public static readonly Func<float, float> InCubic = Ease.InCubic;
        public static readonly Func<float, float> OutCubic = Ease.OutCubic;
        public static readonly Func<float, float> InOutCubic = Ease.InOutCubic;
        public static readonly Func<float, float> InQuart = Ease.InQuart;
        public static readonly Func<float, float> OutQuart = Ease.OutQuart;
        public static readonly Func<float, float> InOutQuart = Ease.InOutQuart;
        public static readonly Func<float, float> InQuint = Ease.InQuint;
        public static readonly Func<float, float> OutQuint = Ease.OutQuint;
        public static readonly Func<float, float> InOutQuint = Ease.InOutQuint;
        public static readonly Func<float, float> InSine = Ease.InSine;
        public static readonly Func<float, float> OutSine = Ease.OutSine;
        public static readonly Func<float, float> InOutSine = Ease.InOutSine;
        public static readonly Func<float, float> InExpo = Ease.InExpo;
        public static readonly Func<float, float> OutExpo = Ease.OutExpo;
        public static readonly Func<float, float> InOutExpo = Ease.InOutExpo;
        public static readonly Func<float, float> InCirc = Ease.InCirc;
        public static readonly Func<float, float> OutCirc = Ease.OutCirc;
        public static readonly Func<float, float> InOutCirc = Ease.InOutCirc;
        public static readonly Func<float, float> InBack = Ease.InBack;
        public static readonly Func<float, float> OutBack = Ease.OutBack;
        public static readonly Func<float, float> InOutBack = Ease.InOutBack;
        public static readonly Func<float, float> InElastic = Ease.InElastic;
        public static readonly Func<float, float> OutElastic = Ease.OutElastic;
        public static readonly Func<float, float> InOutElastic = Ease.InOutElastic;
        public static readonly Func<float, float> InBounce = Ease.InBounce;
        public static readonly Func<float, float> OutBounce = Ease.OutBounce;
        public static readonly Func<float, float> InOutBounce = Ease.InOutBounce;

        /// https://gist.github.com/Kryzarel/bba64622057f21a1d6d44879f9cd7bd4
        public static class Ease
        {
            public static float Linear(float t) => t;

            public static float InQuad(float t) => t * t;
            public static float OutQuad(float t) => 1 - InQuad(1 - t);
            public static float InOutQuad(float t)
            {
                if (t < 0.5) return InQuad(t * 2) / 2;
                return 1 - InQuad((1 - t) * 2) / 2;
            }

            public static float InCubic(float t) => t * t * t;
            public static float OutCubic(float t) => 1 - InCubic(1 - t);
            public static float InOutCubic(float t)
            {
                if (t < 0.5) return InCubic(t * 2) / 2;
                return 1 - InCubic((1 - t) * 2) / 2;
            }

            public static float InQuart(float t) => t * t * t * t;
            public static float OutQuart(float t) => 1 - InQuart(1 - t);
            public static float InOutQuart(float t)
            {
                if (t < 0.5) return InQuart(t * 2) / 2;
                return 1 - InQuart((1 - t) * 2) / 2;
            }

            public static float InQuint(float t) => t * t * t * t * t;
            public static float OutQuint(float t) => 1 - InQuint(1 - t);
            public static float InOutQuint(float t)
            {
                if (t < 0.5) return InQuint(t * 2) / 2;
                return 1 - InQuint((1 - t) * 2) / 2;
            }

            public static float InSine(float t) => -Cos(t * PI / 2);
            public static float OutSine(float t) => Sin(t * PI / 2);
            public static float InOutSine(float t) => (Cos(t * PI) - 1) / -2;

            public static float InExpo(float t) => Pow(2, 10 * (t - 1));
            public static float OutExpo(float t) => 1 - InExpo(1 - t);
            public static float InOutExpo(float t)
            {
                if (t < 0.5) return InExpo(t * 2) / 2;
                return 1 - InExpo((1 - t) * 2) / 2;
            }

            public static float InCirc(float t) => -(Sqrt(1 - t * t) - 1);
            public static float OutCirc(float t) => 1 - InCirc(1 - t);
            public static float InOutCirc(float t)
            {
                if (t < 0.5) return InCirc(t * 2) / 2;
                return 1 - InCirc((1 - t) * 2) / 2;
            }

            public static float InElastic(float t) => 1 - OutElastic(1 - t);
            public static float OutElastic(float t)
            {
                float p = 0.3f;
                return Pow(2, -10 * t) * Sin((t - p / 4) * (2 * PI) / p) + 1;
            }
            public static float InOutElastic(float t)
            {
                if (t < 0.5) return InElastic(t * 2) / 2;
                return 1 - InElastic((1 - t) * 2) / 2;
            }

            public static float InBack(float t)
            {
                float s = 1.70158f;
                return t * t * ((s + 1) * t - s);
            }
            public static float OutBack(float t) => 1 - InBack(1 - t);
            public static float InOutBack(float t)
            {
                if (t < 0.5) return InBack(t * 2) / 2;
                return 1 - InBack((1 - t) * 2) / 2;
            }

            public static float InBounce(float t) => 1 - OutBounce(1 - t);
            public static float OutBounce(float t)
            {
                float div = 2.75f;
                float mult = 7.5625f;

                if (t < 1 / div)
                {
                    return mult * t * t;
                }
                else if (t < 2 / div)
                {
                    t -= 1.5f / div;
                    return mult * t * t + 0.75f;
                }
                else if (t < 2.5 / div)
                {
                    t -= 2.25f / div;
                    return mult * t * t + 0.9375f;
                }
                else
                {
                    t -= 2.625f / div;
                    return mult * t * t + 0.984375f;
                }
            }
            public static float InOutBounce(float t)
            {
                if (t < 0.5) return InBounce(t * 2) / 2;
                return 1 - InBounce((1 - t) * 2) / 2;
            }
        }
    }
}