using System;

namespace BDEase
{
    // https://github.com/ai/easings.net/blob/master/src/easings/easingsFunctions.ts
    public static class Easings
    {
        const float PI = (float)Math.PI;
        const float TAU = 2 * PI;
        const float HALF_PI = PI / 2f;
        const float c1 = 1.70158f;
        const float c2 = c1 * 1.525f;
        const float c3 = c1 + 1;
        const float c4 = TAU / 3f;
        const float c5 = TAU / 4.5f;

        /// Flips X around x=.5; good for "yoyo".
        public static Func<float, float> FlipX(Func<float, float> ease = default)
        {
            ease ??= Linear;
            return (x) => ease(1 - x);
        }
        /// Flips Y around y=.5; good for quick & dirty switch ease in<->ease out.
        public static Func<float, float> FlipY(Func<float, float> ease = default)
        {
            ease ??= Linear;
            return (x) => 1 - ease(x);
        }

        /// AKA identity, etc.
        public static float Linear(float x) => x;

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

        static float Pow(float x, float y) => (float)Math.Pow(x, y);
        static float Cos(float x) => (float)Math.Cos(x);
        static float Sin(float x) => (float)Math.Sin(x);
        static float Sqrt(float x) => (float)Math.Sqrt(x);

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