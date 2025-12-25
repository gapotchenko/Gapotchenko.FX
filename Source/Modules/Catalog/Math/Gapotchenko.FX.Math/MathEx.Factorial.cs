// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2020

namespace Gapotchenko.FX.Math;

partial class MathEx
{
    /// <inheritdoc cref="Factorial(uint)"/>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is negative.</exception>
    public static int Factorial(int value)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(value);

        int result = 1;
        checked
        {
            for (int factor = 2; factor <= value; ++factor)
                result *= factor;
        }
        return result;
    }

    /// <summary>
    /// Returns the factorial of the specified value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The factorial of <paramref name="value"/>.</returns>
    /// <exception cref="OverflowException"><paramref name="value"/> is too big to calculate a factorial value for.</exception>
    [CLSCompliant(false)]
    public static uint Factorial(uint value)
    {
        uint result = 1;
        checked
        {
            for (uint factor = 2; factor <= value; ++factor)
                result *= factor;
        }
        return result;
    }

    /// <inheritdoc cref="Factorial(int)"/>
    public static long Factorial(long value)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(value);

        long result = 1;
        checked
        {
            for (int factor = 2; factor <= value; ++factor)
                result *= factor;
        }
        return result;
    }

    /// <inheritdoc cref="Factorial(uint)"/>
    [CLSCompliant(false)]
    public static ulong Factorial(ulong value)
    {
        ulong result = 1;
        checked
        {
            for (ulong factor = 2; factor <= value; ++factor)
                result *= factor;
        }
        return result;
    }
}
