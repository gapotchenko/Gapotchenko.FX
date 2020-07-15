using System;
using System.Runtime.CompilerServices;

#if !TFF_MATHF

namespace System
{
    /// <summary>
    /// <para>
    /// Provides constants and static methods for trigonometric, logarithmic, and other common mathematical functions for single-precision floating-point numbers.
    /// </para>
    /// <para>
    /// This is a polyfill provided by Gapotchenko.FX.
    /// </para>
    /// </summary>
    public static class MathF
    {
        /// <summary>
        /// Represents the natural logarithmic base, specified by the constant, e.
        /// </summary>
        public const float E = (float)Math.E;

        /// <summary>
        /// Represents the ratio of the circumference of a circle to its diameter, specified by the constant, π.
        /// </summary>
        public const float PI = (float)Math.PI;

        /// <summary>
        /// Returns the absolute value of a single-precision floating-point number.
        /// </summary>
        /// <param name="x">
        /// A number that is greater than or equal to <see cref="float.MinValue"/>,
        /// but less than or equal to <see cref="float.MaxValue"/>.
        /// </param>
        /// <returns>A single-precision floating-point number, x, such that 0 ≤ x ≤ <see cref="float.MaxValue"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Abs(float x) => Math.Abs(x);

        /// <summary>
        /// Returns the angle whose cosine is the specified number.
        /// </summary>
        /// <param name="x">
        /// A number representing a cosine,
        /// where x must be greater than or equal to -1,
        /// but less than or equal to 1.
        /// </param>
        /// <returns>
        /// An angle, θ, measured in radians, such that 0 ≤ θ ≤ π.
        /// -or-
        /// <see cref="float.NaN"/> if x &lt; -1 or x &gt; 1 or x equals <see cref="float.NaN"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Acos(float x) => (float)Math.Acos(x);

        // TODO: Acosh -> MathEx.Acosh

        /// <summary>
        /// Returns the angle whose sine is the specified number.
        /// </summary>
        /// <param name="x">
        /// A number representing a sine,
        /// where x must be greater than or equal to -1,
        /// but less than or equal to 1.
        /// </param>
        /// <returns>
        /// An angle, θ, measured in radians, such that -π/2 ≤ θ ≤ π/2.
        /// -or-
        /// <see cref="float.NaN"/> if x &lt; -1 or x &gt; 1 or x equals <see cref="float.NaN"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Asin(float x) => (float)Math.Asin(x);

        // TODO: Asinh -> MathEx.Asinh

        /// <summary>
        /// Returns the angle whose tangent is the specified number.
        /// </summary>
        /// <param name="x">A number representing a tangent.</param>
        /// <returns>
        /// An angle, θ, measured in radians, such that -π/2 ≤ θ ≤ π/2.
        /// -or-
        /// <see cref="float.NaN"/> if x equals System.Single.NaN,
        /// -π/2 rounded to single precision (-1.5707964) if x equals <see cref="float.NegativeInfinity"/>,
        /// or π/2 rounded to single precision (1.5707964) if x equals <see cref="float.PositiveInfinity"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Atan(float x) => (float)Math.Atan(x);

        /// <summary>
        /// Returns the angle whose tangent is the quotient of two specified numbers.
        /// </summary>
        /// <param name="y">The y coordinate of a point.</param>
        /// <param name="x">The x coordinate of a point.</param>
        /// <returns>
        /// <para>
        /// An angle, θ, measured in radians, such that -π ≤ θ ≤ π, and tan(θ) = y / x, where (x, y) is a point in the Cartesian plane.
        /// </para>
        /// <para>
        /// Observe the following ranges in corresponding quadrants:
        /// <list type="number">
        /// <item>0 &lt; θ &lt; π/2</item>
        /// <item>π/2 &lt; θ ≤ π</item>
        /// <item>-π &lt; θ &lt; -π/2</item>
        /// <item>-π/2 &lt; θ &lt; 0</item>
        /// </list>
        /// </para>
        /// <para>
        /// For points on the boundaries of the quadrants, the return value is the following:
        /// <list type="bullet">
        /// <item>If y is 0 and x is not negative, θ = 0</item>
        /// <item>If y is 0 and x is negative, θ = π</item>
        /// <item>If y is positive and x is 0, θ = π/2</item>
        /// <item>If y is negative and x is 0, θ = -π/2</item>
        /// <item>If y is 0 and x is 0, θ = 0</item>
        /// </list>
        /// </para>
        /// <para>
        /// If x or y is <see cref="float.NaN"/>,
        /// or if x and y are either <see cref="float.PositiveInfinity"/> or <see cref="float.NegativeInfinity"/>, the method returns <see cref="float.NaN"/>.
        /// </para>
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Atan2(float y, float x) => (float)Math.Atan2(x, y);

        // TODO: Atanh -> MathEx.Atanh

        // TODO: BitDecrement

        // TODO: BitIncrement

        // TODO: Cbrt -> MathEx.Cbrt

        /// <summary>
        /// Returns the smallest integral value that is greater than or equal to the specified single-precision floating-point number.
        /// </summary>
        /// <param name="x">A single-precision floating-point number.</param>
        /// <returns>
        /// The smallest integral value that is greater than or equal to <paramref name="x"/>.
        /// If x is equal to <see cref="float.NaN"/>, <see cref="float.NegativeInfinity"/>, or <see cref="float.PositiveInfinity"/>, that value is returned.
        /// Note that this method returns a <see cref="float"/> instead of an integral type.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Ceiling(float x) => (float)Math.Ceiling(x);

        // TODO: CopySign -> MathEx.CopySign

        /// <summary>
        /// Returns the cosine of the specified angle.
        /// </summary>
        /// <param name="x">An angle, measured in radians.</param>
        /// <returns>
        /// The cosine of x.
        /// If x is equal to <see cref="float.NaN"/>, <see cref="float.NegativeInfinity"/>, or <see cref="float.PositiveInfinity"/>, this method returns <see cref="float.NaN"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Cos(float x) => (float)Math.Cos(x);

        // TODO
    }
}

#else

[assembly: TypeForwardedTo(typeof(MathF))]

#endif
