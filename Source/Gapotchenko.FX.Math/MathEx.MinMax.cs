using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Gapotchenko.FX.Math
{
    partial class MathEx
    {
        /// <summary>
        /// Returns the smaller of two values.
        /// </summary>
        /// <typeparam name="T">The type of values to compare.</typeparam>
        /// <param name="val1">The first of two values to compare.</param>
        /// <param name="val2">The second of two values to compare.</param>
        /// <returns>
        /// Parameter <paramref name="val1"/> or <paramref name="val2"/>, whichever is smaller.
        /// </returns>
        public static T Min<T>(T val1, T val2)
        {
            if (val1 == null)
                return val2;
            if (val2 == null)
                return val1;

            // Give a preference to the first value when both values are equal.
            // This is important for reference types and composite value types.
            if (Comparer<T>.Default.Compare(val1, val2) <= 0)
                return val1;
            else
                return val2;
        }

        /// <summary>
        /// Returns the larger of two values.
        /// </summary>
        /// <typeparam name="T">The type of values to compare.</typeparam>
        /// <param name="val1">The first of two values to compare.</param>
        /// <param name="val2">The second of two values to compare.</param>
        /// <returns>
        /// Parameter <paramref name="val1"/> or <paramref name="val2"/>, whichever is larger.
        /// </returns>
        public static T Max<T>(T val1, T val2)
        {
            if (val1 == null)
                return val2;
            if (val2 == null)
                return val1;

            // Give a preference to the first value when both values are equal.
            // This is important for reference types and composite value types.
            if (Comparer<T>.Default.Compare(val1, val2) >= 0)
                return val1;
            else
                return val2;
        }

        /// <summary>
        /// Returns the smaller of three values.
        /// </summary>
        /// <typeparam name="T">The type of values to compare.</typeparam>
        /// <param name="val1">The first of three values to compare.</param>
        /// <param name="val2">The second of three values to compare.</param>
        /// <param name="val3">The third of three values to compare.</param>
        /// <returns>
        /// Parameter <paramref name="val1"/>, <paramref name="val2"/> or <paramref name="val3"/>, whichever is smaller.
        /// </returns>
        public static T Min<T>(T val1, T val2, T val3) => Min(Min(val1, val2), val3);

        /// <summary>
        /// Returns the larger of three values.
        /// </summary>
        /// <typeparam name="T">The type of values to compare.</typeparam>
        /// <param name="val1">The first of three values to compare.</param>
        /// <param name="val2">The second of three values to compare.</param>
        /// <param name="val3">The third of three values to compare.</param>
        /// <returns>
        /// Parameter <paramref name="val1"/>, <paramref name="val2"/> or <paramref name="val3"/>, whichever is larger.
        /// </returns>
        public static T Max<T>(T val1, T val2, T val3) => Max(Max(val1, val2), val3);
    }
}
