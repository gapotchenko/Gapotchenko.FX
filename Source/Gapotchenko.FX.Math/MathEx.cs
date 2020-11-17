using Gapotchenko.FX.Math.Properties;
using System;
using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.Math
{
    /// <summary>
    /// Provides extended static methods for mathematical functions.
    /// </summary>
    public static partial class MathEx
    {
        /// <summary>
        /// Swaps two values.
        /// </summary>
        /// <typeparam name="T">The type of values to swap.</typeparam>
        /// <param name="val1">The first of two values to swap.</param>
        /// <param name="val2">The second of two values to swap.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Swap<T>(ref T val1, ref T val2)
        {
            var temp = val1;
            val1 = val2;
            val2 = temp;
        }

        /// <summary>
        /// Returns the factorial of the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The factorial of <paramref name="value"/>.</returns>
        public static int Factorial(int value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), Resources.ArgumentCannotBeNegative);

            int result = 1;
            for (int factor = 2; factor <= value; factor++)
                result = checked(result * factor);
            return result;
        }

        /// <summary>
        /// Returns the factorial of the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The factorial of <paramref name="value"/>.</returns>
        public static long Factorial(long value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), Resources.ArgumentCannotBeNegative);

            long result = 1;
            for (int factor = 2; factor <= value; factor++)
                result = checked(result * factor);
            return result;
        }

        /// <summary>
        /// Returns the linear interpolation between the specified <paramref name="a">start</paramref> and <paramref name="b">end</paramref> values by the given <paramref name="t">coefficient</paramref>.
        /// </summary>
        /// <param name="a">The start value.</param>
        /// <param name="b">The end value.</param>
        /// <param name="t">The interpolation coefficient between the start and end values.</param>
        /// <returns>The linearly interpolated value between <paramref name="a"/> and <paramref name="b"/> by <paramref name="t"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Lerp(float a, float b, float t) => a + (b - a) * t;

        /// <summary>
        /// Returns the linear interpolation between the specified <paramref name="a">start</paramref> and <paramref name="b">end</paramref> values by the given <paramref name="t">coefficient</paramref>.
        /// </summary>
        /// <param name="a">The start value.</param>
        /// <param name="b">The end value.</param>
        /// <param name="t">The interpolation coefficient between the start and end values.</param>
        /// <returns>The linearly interpolated value between <paramref name="a"/> and <paramref name="b"/> by <paramref name="t"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Lerp(double a, double b, double t) => a + (b - a) * t;
    }
}
