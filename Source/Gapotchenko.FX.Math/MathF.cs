using System.Runtime.CompilerServices;

#if !TFF_MATHF

#pragma warning disable IDE0130 // Namespace does not match folder structure

namespace System;

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
    // public static float Acosh(float x);

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
    // public static float Asinh(float x);

    /// <summary>
    /// Returns the angle whose tangent is the specified number.
    /// </summary>
    /// <param name="x">A number representing a tangent.</param>
    /// <returns>
    /// An angle, θ, measured in radians, such that -π/2 ≤ θ ≤ π/2.
    /// -or-
    /// <see cref="float.NaN"/> if x equals <see cref="float.NaN"/>,
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
    // public static float Atanh(float x);

    // TODO: BitDecrement
    // public static float BitDecrement(float x);

    // TODO: BitIncrement
    // public static float BitIncrement(float x);

    // TODO: Cbrt -> MathEx.Cbrt
    // public static float Cbrt(float x);

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
    // public static float CopySign(float x, float y);

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

    /// <summary>
    /// Returns the hyperbolic cosine of the specified angle.
    /// </summary>
    /// <param name="x">An angle, measured in radians.</param>
    /// <returns>
    /// The hyperbolic cosine of <paramref name="x"/>.
    /// If x is equal to <see cref="float.NegativeInfinity"/> or <see cref="float.PositiveInfinity"/>, <see cref="float.PositiveInfinity"/> is returned.
    /// If x is equal to <see cref="float.NaN"/>, <see cref="float.NaN"/> is returned.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Cosh(float x) => (float)Math.Cosh(x);

    /// <summary>
    /// Returns e raised to the specified power.
    /// </summary>
    /// <param name="x">A number specifying a power.</param>
    /// <returns>
    /// The number e raised to the power x.
    /// If x equals <see cref="float.NaN"/> or <see cref="float.PositiveInfinity"/>, that value is returned.
    /// If x equals <see cref="float.NegativeInfinity"/>, 0 is returned.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Exp(float x) => (float)Math.Exp(x);

    /// <summary>
    /// Returns the largest integral value less than or equal to the specified single-precision floating-point number.
    /// </summary>
    /// <param name="x">A single-precision floating-point number.</param>
    /// <returns>
    /// The largest integral value less than or equal to <paramref name="x"/>.
    /// If x is equal to <see cref="float.NaN"/>, <see cref="float.NegativeInfinity"/>, or <see cref="float.PositiveInfinity"/>, that value is returned.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Floor(float x) => (float)Math.Floor(x);

    // TODO: FusedMultiplyAdd -> MathEx.FusedMultiplyAdd
    // public static float FusedMultiplyAdd(float x, float y, float z);

    /// <summary>
    /// Returns the remainder resulting from the division of a specified number by another specified number.
    /// </summary>
    /// <param name="x">A dividend.</param>
    /// <param name="y">A divisor.</param>
    /// <returns>
    /// A number equal to x - (y Q), where Q is the quotient of x / y rounded to the nearest integer
    /// (if x / y falls halfway between two integers, the even integer is returned).
    /// If x - (y Q) is zero, the value +0 is returned if x is positive, or -0 if x is negative.
    /// If y = 0, <see cref="float.NaN"/> is returned.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float IEEERemainder(float x, float y) => (float)Math.IEEERemainder(x, y);

    // TODO: ILogB -> MathEx.ILogB
    // public static int ILogB(float x);

    /// <summary>
    /// Returns the natural (base e) logarithm of a specified number.
    /// </summary>
    /// <param name="x">The number whose logarithm is to be found.</param>
    /// <returns>
    /// The natural (base e) logarithm of <paramref name="x"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Log(float x) => (float)Math.Log(x);

    /// <summary>
    /// Returns the logarithm of a specified number in a specified base.
    /// </summary>
    /// <param name="x">The number whose logarithm is to be found.</param>
    /// <param name="y">The base.</param>
    /// <returns>The logarithm of <paramref name="x"/> in base <paramref name="y"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Log(float x, float y) => (float)Math.Log(x, y);

    /// <summary>
    /// Returns the base 10 logarithm of a specified number.
    /// </summary>
    /// <param name="x">A number whose logarithm is to be found.</param>
    /// <returns>The base 10 logarithm of <paramref name="x"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Log10(float x) => (float)Math.Log10(x);

    // TODO: Log2 -> MathEx.Log2
    // public static float Log2(float x);

    /// <summary>
    /// Returns the larger of two single-precision floating-point numbers.
    /// </summary>
    /// <param name="x">The first of two single-precision floating-point numbers to compare.</param>
    /// <param name="y">The second of two single-precision floating-point numbers to compare.</param>
    /// <returns>
    /// Parameter x or y, whichever is larger.
    /// If x, or y, or both x and y are equal to <see cref="float.NaN"/>, <see cref="float.NaN"/> is returned.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Max(float x, float y) => Math.Max(x, y);

    // TODO: MaxMagnitude -> MathEx.MaxMagnitude
    // public static float MaxMagnitude(float x, float y);

    /// <summary>
    /// Returns the smaller of two single-precision floating-point numbers.
    /// </summary>
    /// <param name="x">The first of two single-precision floating-point numbers to compare.</param>
    /// <param name="y">The second of two single-precision floating-point numbers to compare.</param>
    /// <returns>
    /// Parameter x or y, whichever is smaller.
    /// If x, y, or both x and y are equal to <see cref="float.NaN"/>, <see cref="float.NaN"/> is returned.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Min(float x, float y) => Math.Min(x, y);

    // TODO: MinMagnitude -> MathEx.MinMagnitude
    // public static float MinMagnitude(float x, float y);

    /// <summary>
    /// Returns a specified number raised to the specified power.
    /// </summary>
    /// <param name="x">A single-precision floating-point number to be raised to a power.</param>
    /// <param name="y">A single-precision floating-point number that specifies a power.</param>
    /// <returns>The number x raised to the power y.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Pow(float x, float y) => (float)Math.Pow(x, y);

    /// <summary>
    /// Rounds a single-precision floating-point value to the nearest integer,
    /// and uses the specified rounding convention for midpoint values.
    /// </summary>
    /// <param name="x">A single-precision floating-point number to be rounded.</param>
    /// <param name="mode">Specification for how to round <paramref name="x"/> if it is midway between two other numbers.</param>
    /// <returns>
    /// The integer nearest x.
    /// If x is halfway between two integers, one of which is even and the other odd, then mode determines which of the two is returned.
    /// Note that this method returns a <see cref="float"/> instead of an integral type.
    /// </returns>
    /// <exception cref="ArgumentException"><paramref name="mode"/> is not a valid value of <see cref="MidpointRounding"/>.</exception>
    public static float Round(float x, MidpointRounding mode) => (float)Math.Round(x, mode);

    /// <summary>
    /// Rounds a single-precision floating-point value to a specified number of fractional digits,
    /// and uses the specified rounding convention for midpoint values.
    /// </summary>
    /// <param name="x">A single-precision floating-point number to be rounded.</param>
    /// <param name="digits">The number of fractional digits in the return value.</param>
    /// <param name="mode">Specification for how to round <paramref name="x"/> if it is midway between two other numbers.</param>
    /// <returns>
    /// The number nearest to x that has a number of fractional digits equal to digits.
    /// If x has fewer fractional digits than digits, x is returned unchanged.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="digits"/> is less than 0 or greater than 6.</exception>
    /// <exception cref="ArgumentException"><paramref name="mode"/> is not a valid value of <see cref="MidpointRounding"/>.</exception>
    public static float Round(float x, int digits, MidpointRounding mode)
    {
        ValidateDigits(digits);
        return (float)Math.Round(x, digits, mode);
    }

    /// <summary>
    /// Rounds a single-precision floating-point value to a specified number of fractional digits,
    /// and rounds midpoint values to the nearest even number.
    /// </summary>
    /// <param name="x">A single-precision floating-point number to be rounded.</param>
    /// <param name="digits">The number of fractional digits in the return value.</param>
    /// <returns>
    /// The number nearest to x that contains a number of fractional digits equal to digits.
    /// </returns>
    public static float Round(float x, int digits)
    {
        ValidateDigits(digits);
        return (float)Math.Round(x, digits);
    }

    static void ValidateDigits(int digits)
    {
        if (digits < 0 || digits > 6)
            throw new ArgumentOutOfRangeException(nameof(digits), "Number of digits is less than 0 or greater than 6.");
    }

    /// <summary>
    /// Rounds a single-precision floating-point value to the nearest integral value,
    /// and rounds midpoint values to the nearest even number.
    /// </summary>
    /// <param name="x">A single-precision floating-point number to be rounded.</param>
    /// <returns>
    /// The integer nearest x.
    /// If the fractional component of x is halfway between two integers, one of which is even and the other odd, then the even number is returned.
    /// Note that this method returns a System.Single instead of an integral type.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Round(float x) => (float)Math.Round(x);

    // TODO: ScaleB -> MathEx.ScaleB
    // public static float ScaleB(float x, int n);

    /// <summary>
    /// Returns an integer that indicates the sign of a single-precision floating-point number.
    /// </summary>
    /// <param name="x">A signed number.</param>
    /// <returns>A number that indicates the sign of <paramref name="x"/>.</returns>
    /// <exception cref="ArithmeticException"><paramref name="x"/> is equal to <see cref="float.NaN"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Sign(float x) => Math.Sign(x);

    /// <summary>
    /// Returns the sine of the specified angle.
    /// </summary>
    /// <param name="x">An angle, measured in radians.</param>
    /// <returns>
    /// The sine of <paramref name="x"/>.
    /// If x is equal to <see cref="float.NaN"/>, <see cref="float.NegativeInfinity"/>, or <see cref="float.PositiveInfinity"/>, this method returns <see cref="float.NaN"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Sin(float x) => (float)Math.Sin(x);

    /// <summary>
    /// Returns the hyperbolic sine of the specified angle.
    /// </summary>
    /// <param name="x">An angle, measured in radians.</param>
    /// <returns>
    /// The hyperbolic sine of <paramref name="x"/>.
    /// If x is equal to <see cref="float.NegativeInfinity"/>, <see cref="float.PositiveInfinity"/>, or <see cref="float.NaN"/>, this method returns a System.Single equal to x.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Sinh(float x) => (float)Math.Sinh(x);

    /// <summary>
    /// Returns the square root of a specified number.
    /// </summary>
    /// <param name="x">The number whose square root is to be found.</param>
    /// <returns>
    /// The square root of <paramref name="x"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Sqrt(float x) => (float)Math.Sqrt(x);

    /// <summary>
    /// Returns the tangent of the specified angle.
    /// </summary>
    /// <param name="x">An angle, measured in radians.</param>
    /// <returns>
    /// The tangent of <paramref name="x"/>.
    /// If x is equal to <see cref="float.NaN"/>, <see cref="float.NegativeInfinity"/>, or <see cref="float.PositiveInfinity"/>, this method returns <see cref="float.NaN"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Tan(float x) => (float)Math.Tan(x);

    /// <summary>
    /// Returns the hyperbolic tangent of the specified angle.
    /// </summary>
    /// <param name="x">An angle, measured in radians.</param>
    /// <returns>
    /// The hyperbolic tangent of <paramref name="x"/>.
    /// If x is equal to <see cref="float.NegativeInfinity"/>, this method returns -1.
    /// If value is equal to <see cref="float.PositiveInfinity"/>, this method returns 1.
    /// If x is equal to <see cref="float.NaN"/>, this method returns <see cref="float.NaN"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Tanh(float x) => (float)Math.Tanh(x);

    /// <summary>
    /// Calculates the integral part of a specified single-precision floating-point number.
    /// </summary>
    /// <param name="x">A number to truncate.</param>
    /// <returns>
    /// The integral part of <paramref name="x"/>; that is, the number that remains after any fractional digits have been discarded.
    /// If x is equal to <see cref="float.NegativeInfinity"/>, <see cref="float.PositiveInfinity"/>, or <see cref="float.NaN"/>, this method returns a System.Single equal to x.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Truncate(float x) => (float)Math.Truncate(x);
}

#else

[assembly: TypeForwardedTo(typeof(MathF))]

#endif
