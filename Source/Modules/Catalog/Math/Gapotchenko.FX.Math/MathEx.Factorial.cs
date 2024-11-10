// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2020

using Gapotchenko.FX.Math.Properties;

namespace Gapotchenko.FX.Math;

partial class MathEx
{
    /// <summary>
    /// Returns the factorial of the specified value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The factorial of <paramref name="value"/>.</returns>
    /// <exception cref="OverflowException"><paramref name="value"/> is too big to calculate a factorial value for.</exception>
    public static int Factorial(int value)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(nameof(value), Resources.ArgumentCannotBeNegative);

        int result = 1;
        for (int factor = 2; factor <= value; ++factor)
            result = checked(result * factor);
        return result;
    }

    /// <inheritdoc cref="Factorial(int)"/>
    [CLSCompliant(false)]
    public static uint Factorial(uint value)
    {
        uint result = 1;
        for (uint factor = 2; factor <= value; ++factor)
            result = checked(result * factor);
        return result;
    }

    /// <inheritdoc cref="Factorial(int)"/>
    public static long Factorial(long value)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(nameof(value), Resources.ArgumentCannotBeNegative);

        long result = 1;
        for (int factor = 2; factor <= value; ++factor)
            result = checked(result * factor);
        return result;
    }

    /// <inheritdoc cref="Factorial(int)"/>
    [CLSCompliant(false)]
    public static ulong Factorial(ulong value)
    {
        ulong result = 1;
        for (ulong factor = 2; factor <= value; ++factor)
            result = checked(result * factor);
        return result;
    }
}
