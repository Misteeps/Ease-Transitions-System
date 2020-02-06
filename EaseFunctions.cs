using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace EaseTransitionsSystem
{
    public enum EaseFunctions { Linear, Quadratic, Cubic, Quartic, Quintic, Sine, Circular, Exponential, Elastic, Back, Bounce, /*Custom*/ }
    public enum EaseDirection { In, Out, InOut }

    public static class Ease
    {
        static public float CalculateEase(EaseFunctions function, EaseDirection direction, float x)
        {
            switch (function)
            {
                case EaseFunctions.Linear: return Linear(x);

                case EaseFunctions.Quadratic:
                    switch (direction)
                    {
                        case EaseDirection.In: return QuadraticIn(x);
                        case EaseDirection.Out: return QuadraticOut(x);
                        case EaseDirection.InOut: return QuadraticInOut(x);

                        default: Debug.LogError("Ease Function Enum Out of Bounds Error"); return 0;
                    }
                case EaseFunctions.Cubic:
                    switch (direction)
                    {
                        case EaseDirection.In: return CubicIn(x);
                        case EaseDirection.Out: return CubicOut(x);
                        case EaseDirection.InOut: return CubicInOut(x);

                        default: Debug.LogError("Ease Function Enum Out of Bounds Error"); return 0;
                    }
                case EaseFunctions.Quartic:
                    switch (direction)
                    {
                        case EaseDirection.In: return QuarticIn(x);
                        case EaseDirection.Out: return QuarticOut(x);
                        case EaseDirection.InOut: return QuarticInOut(x);

                        default: Debug.LogError("Ease Function Enum Out of Bounds Error"); return 0;
                    }
                case EaseFunctions.Quintic:
                    switch (direction)
                    {
                        case EaseDirection.In: return QuinticIn(x);
                        case EaseDirection.Out: return QuinticOut(x);
                        case EaseDirection.InOut: return QuinticInOut(x);

                        default: Debug.LogError("Ease Function Enum Out of Bounds Error"); return 0;
                    }
                case EaseFunctions.Sine:
                    switch (direction)
                    {
                        case EaseDirection.In: return SineIn(x);
                        case EaseDirection.Out: return SineOut(x);
                        case EaseDirection.InOut: return SineInOut(x);

                        default: Debug.LogError("Ease Function Enum Out of Bounds Error"); return 0;
                    }
                case EaseFunctions.Circular:
                    switch (direction)
                    {
                        case EaseDirection.In: return CircularIn(x);
                        case EaseDirection.Out: return CircularOut(x);
                        case EaseDirection.InOut: return CircularInOut(x);

                        default: Debug.LogError("Ease Function Enum Out of Bounds Error"); return 0;
                    }
                case EaseFunctions.Exponential:
                    switch (direction)
                    {
                        case EaseDirection.In: return ExponentialIn(x);
                        case EaseDirection.Out: return ExponentialOut(x);
                        case EaseDirection.InOut: return ExponentialInOut(x);

                        default: Debug.LogError("Ease Function Enum Out of Bounds Error"); return 0;
                    };
                case EaseFunctions.Elastic:
                    switch (direction)
                    {
                        case EaseDirection.In: return ElasticIn(x);
                        case EaseDirection.Out: return ElasticOut(x);
                        case EaseDirection.InOut: return ElasticInOut(x);

                        default: Debug.LogError("Ease Function Enum Out of Bounds Error"); return 0;
                    }
                case EaseFunctions.Back:
                    switch (direction)
                    {
                        case EaseDirection.In: return BackIn(x);
                        case EaseDirection.Out: return BackOut(x);
                        case EaseDirection.InOut: return BackInOut(x);

                        default: Debug.LogError("Ease Function Enum Out of Bounds Error"); return 0;
                    }
                case EaseFunctions.Bounce:
                    switch (direction)
                    {
                        case EaseDirection.In: return BounceIn(x);
                        case EaseDirection.Out: return BounceOut(x);
                        case EaseDirection.InOut: return BounceInOut(x);

                        default: Debug.LogError("Ease Function Enum Out of Bounds Error"); return 0;
                    }

                default: Debug.LogError("Ease Function Enum Out of Bounds Error"); return 0;
            }
        }
        static public float CalculateEaseInverse(EaseFunctions function, EaseDirection direction, float x)
        {
            switch (function)
            {
                case EaseFunctions.Linear: return Linear(x);

                case EaseFunctions.Quadratic:
                    switch (direction)
                    {
                        case EaseDirection.In: return QuadraticInInverse(x);
                        case EaseDirection.Out: return QuadraticOutInverse(x);
                        case EaseDirection.InOut: return QuadraticInOutInverse(x);

                        default: Debug.LogError("Ease Function Inverse Enum Out of Bounds Error"); return 0;
                    }
                case EaseFunctions.Cubic:
                    switch (direction)
                    {
                        case EaseDirection.In: return CubicInInverse(x);
                        case EaseDirection.Out: return CubicOutInverse(x);
                        case EaseDirection.InOut: return CubicInOutInverse(x);

                        default: Debug.LogError("Ease Function Inverse Enum Out of Bounds Error"); return 0;
                    }
                case EaseFunctions.Quartic:
                    switch (direction)
                    {
                        case EaseDirection.In: return QuarticInInverse(x);
                        case EaseDirection.Out: return QuarticOutInverse(x);
                        case EaseDirection.InOut: return QuarticInOutInverse(x);

                        default: Debug.LogError("Ease Function Inverse Enum Out of Bounds Error"); return 0;
                    }
                case EaseFunctions.Quintic:
                    switch (direction)
                    {
                        case EaseDirection.In: return QuinticInInverse(x);
                        case EaseDirection.Out: return QuinticOutInverse(x);
                        case EaseDirection.InOut: return QuinticInOutInverse(x);

                        default: Debug.LogError("Ease Function Inverse Enum Out of Bounds Error"); return 0;
                    }
                case EaseFunctions.Sine:
                    switch (direction)
                    {
                        case EaseDirection.In: return SineInInverse(x);
                        case EaseDirection.Out: return SineOutInverse(x);
                        case EaseDirection.InOut: return SineInOutInverse(x);

                        default: Debug.LogError("Ease Function Inverse Enum Out of Bounds Error"); return 0;
                    }
                case EaseFunctions.Circular:
                    switch (direction)
                    {
                        case EaseDirection.In: return CircularInInverse(x);
                        case EaseDirection.Out: return CircularOutInverse(x);
                        case EaseDirection.InOut: return CircularInOutInverse(x);

                        default: Debug.LogError("Ease Function Inverse Enum Out of Bounds Error"); return 0;
                    }
                case EaseFunctions.Exponential:
                    switch (direction)
                    {
                        case EaseDirection.In: return ExponentialInInverse(x);
                        case EaseDirection.Out: return ExponentialOutInverse(x);
                        case EaseDirection.InOut: return ExponentialInOutInverse(x);

                        default: Debug.LogError("Ease Function Inverse Enum Out of Bounds Error"); return 0;
                    };
                case EaseFunctions.Elastic:
                    switch (direction)
                    {
                        case EaseDirection.In: return ElasticInInverse(x);
                        case EaseDirection.Out: return ElasticOutInverse(x);
                        case EaseDirection.InOut: return ElasticInOutInverse(x);

                        default: Debug.LogError("Ease Function Inverse Enum Out of Bounds Error"); return 0;
                    }
                case EaseFunctions.Back:
                    switch (direction)
                    {
                        case EaseDirection.In: return BackInInverse(x);
                        case EaseDirection.Out: return BackOutInverse(x);
                        case EaseDirection.InOut: return BackInOutInverse(x);

                        default: Debug.LogError("Ease Function Inverse Enum Out of Bounds Error"); return 0;
                    }
                case EaseFunctions.Bounce:
                    switch (direction)
                    {
                        case EaseDirection.In: return BounceInInverse(x);
                        case EaseDirection.Out: return BounceOutInverse(x);
                        case EaseDirection.InOut: return BounceInOutInverse(x);

                        default: Debug.LogError("Ease Function Inverse Enum Out of Bounds Error"); return 0;
                    }

                default: Debug.LogError("Ease Function Inverse Enum Out of Bounds Error"); return 0;
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

        // Inverse Functions don't exist.
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

        // Inverse Functions don't exist.
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

        // Inverse Functions don't exist.
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