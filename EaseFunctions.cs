using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace EaseTransitionsSystem
{
    public enum EaseFunctions
    {
        Linear,
        QuadraticIn, QuadraticOut, QuadraticInOut,
        CubicIn, CubicOut, CubicInOut,
        QuarticIn, QuarticOut, QuarticInOut,
        QuinticIn, QuinticOut, QuinticInOut,
        SineIn, SineOut, SineInOut,
        CircularIn, CircularOut, CircularInOut,
        ExponentialIn, ExponentialOut, ExponentialInOut,
        ElasticIn, ElasticOut, ElasticInOut,
        BackIn, BackOut, BackInOut,
        BounceIn, BounceOut, BounceInOut
    }

    public static class EaseFunction
    {
        static public float Ease(EaseFunctions function, float x)
        {
            switch (function)
            {
                default:

                case EaseFunctions.Linear: return Linear(x);

                case EaseFunctions.QuadraticIn: return QuadraticIn(x);
                case EaseFunctions.QuadraticOut: return QuadraticOut(x);
                case EaseFunctions.QuadraticInOut: return QuadraticInOut(x);

                case EaseFunctions.CubicIn: return CubicIn(x);
                case EaseFunctions.CubicOut: return CubicOut(x);
                case EaseFunctions.CubicInOut: return CubicInOut(x);

                case EaseFunctions.QuarticIn: return QuarticIn(x);
                case EaseFunctions.QuarticOut: return QuarticOut(x);
                case EaseFunctions.QuarticInOut: return QuarticInOut(x);

                case EaseFunctions.QuinticIn: return QuinticIn(x);
                case EaseFunctions.QuinticOut: return QuinticOut(x);
                case EaseFunctions.QuinticInOut: return QuinticInOut(x);

                case EaseFunctions.SineIn: return SineIn(x);
                case EaseFunctions.SineOut: return SineOut(x);
                case EaseFunctions.SineInOut: return SineInOut(x);

                case EaseFunctions.CircularIn: return CircularIn(x);
                case EaseFunctions.CircularOut: return CircularOut(x);
                case EaseFunctions.CircularInOut: return CircularInOut(x);

                case EaseFunctions.ExponentialIn: return ExponentialIn(x);
                case EaseFunctions.ExponentialOut: return ExponentialOut(x);
                case EaseFunctions.ExponentialInOut: return ExponentialInOut(x);

                case EaseFunctions.ElasticIn: return ElasticIn(x);
                case EaseFunctions.ElasticOut: return ElasticOut(x);
                case EaseFunctions.ElasticInOut: return ElasticInOut(x);

                case EaseFunctions.BackIn: return BackIn(x);
                case EaseFunctions.BackOut: return BackOut(x);
                case EaseFunctions.BackInOut: return BackInOut(x);

                case EaseFunctions.BounceIn: return BounceIn(x);
                case EaseFunctions.BounceOut: return BounceOut(x);
                case EaseFunctions.BounceInOut: return BounceInOut(x);
            }
        }
        static public float EaseInverse(EaseFunctions function, float x)
        {
            switch (function)
            {
                default:

                case EaseFunctions.Linear: return Linear(x);

                case EaseFunctions.QuadraticIn: return QuadraticInInverse(x);
                case EaseFunctions.QuadraticOut: return QuadraticOutInverse(x);
                case EaseFunctions.QuadraticInOut: return QuadraticInOutInverse(x);

                case EaseFunctions.CubicIn: return CubicInInverse(x);
                case EaseFunctions.CubicOut: return CubicOutInverse(x);
                case EaseFunctions.CubicInOut: return CubicInOutInverse(x);

                case EaseFunctions.QuarticIn: return QuarticInInverse(x);
                case EaseFunctions.QuarticOut: return QuarticOutInverse(x);
                case EaseFunctions.QuarticInOut: return QuarticInOutInverse(x);

                case EaseFunctions.QuinticIn: return QuinticInInverse(x);
                case EaseFunctions.QuinticOut: return QuinticOutInverse(x);
                case EaseFunctions.QuinticInOut: return QuinticInOutInverse(x);

                case EaseFunctions.SineIn: return SineInInverse(x);
                case EaseFunctions.SineOut: return SineOutInverse(x);
                case EaseFunctions.SineInOut: return SineInOutInverse(x);

                case EaseFunctions.CircularIn: return CircularInInverse(x);
                case EaseFunctions.CircularOut: return CircularOutInverse(x);
                case EaseFunctions.CircularInOut: return CircularInOutInverse(x);

                case EaseFunctions.ExponentialIn: return ExponentialInInverse(x);
                case EaseFunctions.ExponentialOut: return ExponentialOutInverse(x);
                case EaseFunctions.ExponentialInOut: return ExponentialInOutInverse(x);

                case EaseFunctions.ElasticIn: return ElasticInInverse(x);
                case EaseFunctions.ElasticOut: return ElasticOutInverse(x);
                case EaseFunctions.ElasticInOut: return ElasticInOutInverse(x);

                case EaseFunctions.BackIn: return BackInInverse(x);
                case EaseFunctions.BackOut: return BackOutInverse(x);
                case EaseFunctions.BackInOut: return BackInOutInverse(x);

                case EaseFunctions.BounceIn: return BounceInInverse(x);
                case EaseFunctions.BounceOut: return BounceOutInverse(x);
                case EaseFunctions.BounceInOut: return BounceInOutInverse(x);
            }
        }


        // Ease Functions : https://easings.net/

        #region Misc
        static private float Linear(float x)
        {
            return x;
        }
        #endregion Misc

        #region Quadratic
        static private float QuadraticIn(float x)
        {
            return x * x;
        }
        static private float QuadraticOut(float x)
        {
            return x * (2 - x);
        }
        static private float QuadraticInOut(float x)
        {
            if (x < 0.5f)
                return 2 * x * x;

            return (-2 * x * x) + (4 * x) - 1;
        }

        static private float QuadraticInInverse(float x)
        {
            return Mathf.Sqrt(x);
        }
        static private float QuadraticOutInverse(float x)
        {
            return 1 - Mathf.Sqrt(-x + 1);
        }
        static private float QuadraticInOutInverse(float x)
        {
            if (x < 0.5f)
                return Mathf.Sqrt(x / 2);

            return (-4 + Mathf.Sqrt(8 * (-x + 1))) / -4;
        }
        #endregion Quadratic

        #region Cubic
        static private float CubicIn(float x)
        {
            return x * x * x;
        }
        static private float CubicOut(float x)
        {
            x = x - 1;
            return x * x * x + 1;
        }
        static private float CubicInOut(float x)
        {
            if (x < 0.5f)
                return 4 * x * x * x;

            x = 2 * x - 2;
            return 0.5f * x * x * x + 1;
        }

        static private float CubicInInverse(float x)
        {
            return Mathf.Pow(x, 1f / 3f);
        }
        static private float CubicOutInverse(float x)
        {
            x = x - 1;
            return -Mathf.Pow(-x, 1f / 3f) + 1;
        }
        static private float CubicInOutInverse(float x)
        {
            if (x < 0.5f)
                return Mathf.Pow(x / 4f, 1f / 3f);

            x = x - 1;
            return 0.62996f * -Mathf.Pow(-x, 1f / 3f) + 1;
        }
        #endregion Cubic

        #region Quartic
        static private float QuarticIn(float x)
        {
            return x * x * x * x;
        }
        static private float QuarticOut(float x)
        {
            x = x - 1;
            return 1 - (x * x * x * x);
        }
        static private float QuarticInOut(float x)
        {
            if (x < 0.5f)
                return 8 * x * x * x * x;

            x = x - 1;
            return -8 * x * x * x * x + 1;
        }

        static private float QuarticInInverse(float x)
        {
            return Mathf.Pow(x, 1f / 4f);
        }
        static private float QuarticOutInverse(float x)
        {
            x = -x + 1;
            return -Mathf.Pow(x, 1f / 4f) + 1;
        }
        static private float QuarticInOutInverse(float x)
        {
            if (x < 0.5f)
                return Mathf.Pow(x / 8f, 1f / 4f);

            x = -(x - 1) / 8f;
            return -Mathf.Pow(x, 1f / 4f) + 1;
        }
        #endregion Quartic

        #region Quintic
        static private float QuinticIn(float x)
        {
            return x * x * x * x * x;
        }
        static private float QuinticOut(float x)
        {
            x = x - 1;
            return x * x * x * x * x + 1;
        }
        static private float QuinticInOut(float x)
        {
            if (x < 0.5f)
                return 16 * x * x * x * x * x;

            x = 2 * x - 2;
            return 0.5f * x * x * x * x * x + 1;
        }

        static private float QuinticInInverse(float x)
        {
            return Mathf.Pow(x, 1f / 5f);
        }
        static private float QuinticOutInverse(float x)
        {
            x = x - 1;
            return -Mathf.Pow(-x, 1f / 5f) + 1;
        }
        static private float QuinticInOutInverse(float x)
        {
            if (x < 0.5f)
                return Mathf.Pow(x / 16f, 1f / 5f);

            x = x - 1;
            return 0.57435f * -Mathf.Pow(-x, 1f / 5f) + 1;
        }
        #endregion Quintic

        #region Sine
        static private float SineIn(float x)
        {
            return Mathf.Sin((x - 1) * (Mathf.PI / 2)) + 1;
        }
        static private float SineOut(float x)
        {
            return Mathf.Sin(x * (Mathf.PI / 2));
        }
        static private float SineInOut(float x)
        {
            return 0.5f * (1 - Mathf.Cos(x * Mathf.PI));
        }

        static private float SineInInverse(float x)
        {
            return (2 / Mathf.PI) * Mathf.Asin(x - 1) + 1;
        }
        static private float SineOutInverse(float x)
        {
            return (2 / Mathf.PI) * Mathf.Asin(x);
        }
        static private float SineInOutInverse(float x)
        {
            return (1 / Mathf.PI) * Mathf.Acos(1 - (2 * x));
        }
        #endregion Sine

        #region Circular
        static private float CircularIn(float x)
        {
            return 1 - Mathf.Sqrt(1 - (x * x));
        }
        static private float CircularOut(float x)
        {
            return Mathf.Sqrt((2 - x) * x);
        }
        static private float CircularInOut(float x)
        {
            if (x < 0.5f)
                return 0.5f * (1 - Mathf.Sqrt(1 - 4 * (x * x)));

            return 0.5f * (Mathf.Sqrt(-((2 * x) - 3) * ((2 * x) - 1)) + 1);
        }

        static private float CircularInInverse(float x)
        {
            return Mathf.Sqrt(x * (2 - x));
        }
        static private float CircularOutInverse(float x)
        {
            return 1 - Mathf.Sqrt(1 - (x * x));
        }
        static private float CircularInOutInverse(float x)
        {
            if (x < 0.5f)
                return Mathf.Sqrt(-x * (x - 1));

            return 1 - Mathf.Sqrt(x * (-x + 1));
        }
        #endregion Circular

        #region Exponential
        static private float ExponentialIn(float x)
        {
            if (x == 0)
                return x;

            return Mathf.Pow(2, 10 * (x - 1));
        }
        static private float ExponentialOut(float x)
        {
            if (x == 1)
                return x;

            return 1 - Mathf.Pow(2, -10 * x);
        }
        static private float ExponentialInOut(float x)
        {
            if (x == 0 || x == 1)
                return x;

            if (x < 0.5f)
                return 0.5f * Mathf.Pow(2, (20 * x) - 10);

            return -0.5f * Mathf.Pow(2, (-20 * x) + 10) + 1;
        }

        static private float ExponentialInInverse(float x)
        {
            x = Mathf.Clamp01(x);

            if (x == 0)
                return x;

            return Mathf.Log(x, 2.71828f) / 6.93147f + 1;
        }
        static private float ExponentialOutInverse(float x)
        {
            x = Mathf.Clamp01(x);

            if (x == 1)
                return x;

            return -Mathf.Log(1 - x, 2.71828f) / 6.93147f;
        }
        static private float ExponentialInOutInverse(float x)
        {
            x = Mathf.Clamp01(x);

            if (x == 0 || x == 1)
                return x;

            if (x < 0.5f)
                return Mathf.Log(2048 * x, 2.71828f) / 13.86294f;

            return -Mathf.Log((1 - x) / 512, 2.71828f) / 13.86294f;
        }
        #endregion Exponential

        // Inverse Functions WIP

        #region Elastic
        static private float ElasticIn(float x)
        {
            return Mathf.Sin(13 * (Mathf.PI / 2) * x) * Mathf.Pow(2, 10 * (x - 1));
        }
        static private float ElasticOut(float x)
        {
            return Mathf.Sin(-13 * (Mathf.PI / 2) * (x + 1)) * Mathf.Pow(2, -10 * x) + 1;
        }
        static private float ElasticInOut(float x)
        {
            if (x < 0.5f)
                return 0.5f * Mathf.Sin(13 * (Mathf.PI / 2) * (2 * x)) * Mathf.Pow(2, 10 * ((2 * x) - 1));

            return 0.5f * (Mathf.Sin(-13 * (Mathf.PI / 2) * ((2 * x - 1) + 1)) * Mathf.Pow(2, -10 * (2 * x - 1)) + 2);
        }

        static private float ElasticInInverse(float x)
        {
            return x;
        }
        static private float ElasticOutInverse(float x)
        {
            return x;
        }
        static private float ElasticInOutInverse(float x)
        {
            return x;
        }
        #endregion Elastic

        #region Back
        static private float BackIn(float x)
        {
            return x * x * x - x * Mathf.Sin(x * Mathf.PI);
        }
        static private float BackOut(float x)
        {
            x = -x + 1;
            return 1 - (x * x * x - x * Mathf.Sin(x * Mathf.PI));
        }
        static private float BackInOut(float x)
        {
            x = x * 2;
            if (x < 1)
                return 0.5f * (x * x * x - x * Mathf.Sin(x * Mathf.PI));

            x = -(x - 2);
            return 0.5f * (1 - (x * x * x - x * Mathf.Sin(x * Mathf.PI))) + 0.5f;
        }

        static private float BackInInverse(float x)
        {
            return x;
        }
        static private float BackOutInverse(float x)
        {
            return x;
        }
        static private float BackInOutInverse(float x)
        {
            return x;
        }
        #endregion Back

        #region Bounce
        static private float BounceIn(float x)
        {
            return 1 - BounceOut(1 - x);
        }
        static private float BounceOut(float x)
        {
            if (x < 0.3636f)
                return (121 * x * x) / 16.0f;
            else if (x < 0.7272f)
                return (9.075f * x * x) - (9.9f * x) + 3.4f;
            else if (x < 0.9f)
                return (12.0665f * x * x) - (19.6355f * x) + 8.8981f;
            else
                return (10.8f * x * x) - (20.52f * x) + 10.72f;
        }
        static private float BounceInOut(float x)
        {
            if (x < 0.5f)
                return 0.5f * BounceIn(x * 2);

            return 0.5f * BounceOut(x * 2 - 1) + 0.5f;
        }

        static private float BounceInInverse(float x)
        {
            return x;
        }
        static private float BounceOutInverse(float x)
        {
            return x;
        }
        static private float BounceInOutInverse(float x)
        {
            return x;
        }
        #endregion Bounce
    }
}