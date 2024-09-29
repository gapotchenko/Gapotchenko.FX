using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.Math;

using Math = System.Math;

partial class MathEx
{
    // This part of MathEx class defines the optimized clamp implementations for primitive types.
    // They are made invisible in editor but compiler catches them up.

    /// <summary>
    /// Returns a 8-bit unsigned integer <paramref name="value"/> clamped to specified inclusive [<paramref name="min"/>; <paramref name="max"/>] range.
    /// </summary>
    /// <param name="value">The value to clamp.</param>
    /// <param name="min">The lower bound of the range.</param>
    /// <param name="max">The upper bound of the range.</param>
    /// <returns>
    /// <paramref name="value"/> if <paramref name="min"/> ≤ <paramref name="value"/> ≤ <paramref name="max"/>.
    /// <br/>-or-<br/>
    /// <paramref name="min"/> if <paramref name="value"/> &lt; <paramref name="min"/>.
    /// <br/>-or-<br/>
    /// <paramref name="max"/> if <paramref name="value"/> &gt; <paramref name="max"/>.
    /// </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte Clamp(byte value, byte min, byte max)
    {
#if TFF_MATH_CLAMP
        return Math.Clamp(value, min, max);
#else
        if (min > max)
            ThrowMinCannotBeGreaterThanMaxException(min, max);

        if (value < min)
            return min;
        else if (value > max)
            return max;

        return value;
#endif
    }

    /// <summary>
    /// Returns a 8-bit signed integer <paramref name="value"/> clamped to specified inclusive [<paramref name="min"/>; <paramref name="max"/>] range.
    /// </summary>
    /// <param name="value">The value to clamp.</param>
    /// <param name="min">The lower bound of the range.</param>
    /// <param name="max">The upper bound of the range.</param>
    /// <returns>
    /// <paramref name="value"/> if <paramref name="min"/> ≤ <paramref name="value"/> ≤ <paramref name="max"/>.
    /// <br/>-or-<br/>
    /// <paramref name="min"/> if <paramref name="value"/> &lt; <paramref name="min"/>.
    /// <br/>-or-<br/>
    /// <paramref name="max"/> if <paramref name="value"/> &gt; <paramref name="max"/>.
    /// </returns>
    [CLSCompliant(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte Clamp(sbyte value, sbyte min, sbyte max)
    {
#if TFF_MATH_CLAMP
        return Math.Clamp(value, min, max);
#else
        if (min > max)
            ThrowMinCannotBeGreaterThanMaxException(min, max);

        if (value < min)
            return min;
        else if (value > max)
            return max;

        return value;
#endif
    }

    /// <summary>
    /// Returns a 16-bit signed integer <paramref name="value"/> clamped to specified inclusive [<paramref name="min"/>; <paramref name="max"/>] range.
    /// </summary>
    /// <param name="value">The value to clamp.</param>
    /// <param name="min">The lower bound of the range.</param>
    /// <param name="max">The upper bound of the range.</param>
    /// <returns>
    /// <paramref name="value"/> if <paramref name="min"/> ≤ <paramref name="value"/> ≤ <paramref name="max"/>.
    /// <br/>-or-<br/>
    /// <paramref name="min"/> if <paramref name="value"/> &lt; <paramref name="min"/>.
    /// <br/>-or-<br/>
    /// <paramref name="max"/> if <paramref name="value"/> &gt; <paramref name="max"/>.
    /// </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short Clamp(short value, short min, short max)
    {
#if TFF_MATH_CLAMP
        return Math.Clamp(value, min, max);
#else
        if (min > max)
            ThrowMinCannotBeGreaterThanMaxException(min, max);

        if (value < min)
            return min;
        else if (value > max)
            return max;

        return value;
#endif
    }

    /// <summary>
    /// Returns a 16-bit unsigned integer <paramref name="value"/> clamped to specified inclusive [<paramref name="min"/>; <paramref name="max"/>] range.
    /// </summary>
    /// <param name="value">The value to clamp.</param>
    /// <param name="min">The lower bound of the range.</param>
    /// <param name="max">The upper bound of the range.</param>
    /// <returns>
    /// <paramref name="value"/> if <paramref name="min"/> ≤ <paramref name="value"/> ≤ <paramref name="max"/>.
    /// <br/>-or-<br/>
    /// <paramref name="min"/> if <paramref name="value"/> &lt; <paramref name="min"/>.
    /// <br/>-or-<br/>
    /// <paramref name="max"/> if <paramref name="value"/> &gt; <paramref name="max"/>.
    /// </returns>
    [CLSCompliant(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort Clamp(ushort value, ushort min, ushort max)
    {
#if TFF_MATH_CLAMP
        return Math.Clamp(value, min, max);
#else
        if (min > max)
            ThrowMinCannotBeGreaterThanMaxException(min, max);

        if (value < min)
            return min;
        else if (value > max)
            return max;

        return value;
#endif
    }

    /// <summary>
    /// Returns a 32-bit signed integer <paramref name="value"/> clamped to specified inclusive [<paramref name="min"/>; <paramref name="max"/>] range.
    /// </summary>
    /// <param name="value">The value to clamp.</param>
    /// <param name="min">The lower bound of the range.</param>
    /// <param name="max">The upper bound of the range.</param>
    /// <returns>
    /// <paramref name="value"/> if <paramref name="min"/> ≤ <paramref name="value"/> ≤ <paramref name="max"/>.
    /// <br/>-or-<br/>
    /// <paramref name="min"/> if <paramref name="value"/> &lt; <paramref name="min"/>.
    /// <br/>-or-<br/>
    /// <paramref name="max"/> if <paramref name="value"/> &gt; <paramref name="max"/>.
    /// </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Clamp(int value, int min, int max)
    {
#if TFF_MATH_CLAMP
        return Math.Clamp(value, min, max);
#else
        if (min > max)
            ThrowMinCannotBeGreaterThanMaxException(min, max);

        if (value < min)
            return min;
        else if (value > max)
            return max;

        return value;
#endif
    }

    /// <summary>
    /// Returns a 32-bit unsigned integer <paramref name="value"/> clamped to specified inclusive [<paramref name="min"/>; <paramref name="max"/>] range.
    /// </summary>
    /// <param name="value">The value to clamp.</param>
    /// <param name="min">The lower bound of the range.</param>
    /// <param name="max">The upper bound of the range.</param>
    /// <returns>
    /// <paramref name="value"/> if <paramref name="min"/> ≤ <paramref name="value"/> ≤ <paramref name="max"/>.
    /// <br/>-or-<br/>
    /// <paramref name="min"/> if <paramref name="value"/> &lt; <paramref name="min"/>.
    /// <br/>-or-<br/>
    /// <paramref name="max"/> if <paramref name="value"/> &gt; <paramref name="max"/>.
    /// </returns>
    [CLSCompliant(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Clamp(uint value, uint min, uint max)
    {
#if TFF_MATH_CLAMP
        return Math.Clamp(value, min, max);
#else
        if (min > max)
            ThrowMinCannotBeGreaterThanMaxException(min, max);

        if (value < min)
            return min;
        else if (value > max)
            return max;

        return value;
#endif
    }

    /// <summary>
    /// Returns a 64-bit signed integer <paramref name="value"/> clamped to specified inclusive [<paramref name="min"/>; <paramref name="max"/>] range.
    /// </summary>
    /// <param name="value">The value to clamp.</param>
    /// <param name="min">The lower bound of the range.</param>
    /// <param name="max">The upper bound of the range.</param>
    /// <returns>
    /// <paramref name="value"/> if <paramref name="min"/> ≤ <paramref name="value"/> ≤ <paramref name="max"/>.
    /// <br/>-or-<br/>
    /// <paramref name="min"/> if <paramref name="value"/> &lt; <paramref name="min"/>.
    /// <br/>-or-<br/>
    /// <paramref name="max"/> if <paramref name="value"/> &gt; <paramref name="max"/>.
    /// </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Clamp(long value, long min, long max)
    {
#if TFF_MATH_CLAMP
        return Math.Clamp(value, min, max);
#else
        if (min > max)
            ThrowMinCannotBeGreaterThanMaxException(min, max);

        if (value < min)
            return min;
        else if (value > max)
            return max;

        return value;
#endif
    }

    /// <summary>
    /// Returns a 64-bit unsigned integer <paramref name="value"/> clamped to specified inclusive [<paramref name="min"/>; <paramref name="max"/>] range.
    /// </summary>
    /// <param name="value">The value to clamp.</param>
    /// <param name="min">The lower bound of the range.</param>
    /// <param name="max">The upper bound of the range.</param>
    /// <returns>
    /// <paramref name="value"/> if <paramref name="min"/> ≤ <paramref name="value"/> ≤ <paramref name="max"/>.
    /// <br/>-or-<br/>
    /// <paramref name="min"/> if <paramref name="value"/> &lt; <paramref name="min"/>.
    /// <br/>-or-<br/>
    /// <paramref name="max"/> if <paramref name="value"/> &gt; <paramref name="max"/>.
    /// </returns>
    [CLSCompliant(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong Clamp(ulong value, ulong min, ulong max)
    {
#if TFF_MATH_CLAMP
        return Math.Clamp(value, min, max);
#else
        if (min > max)
            ThrowMinCannotBeGreaterThanMaxException(min, max);

        if (value < min)
            return min;
        else if (value > max)
            return max;

        return value;
#endif
    }

    /// <summary>
    /// Returns a <see cref="decimal"/> <paramref name="value"/> clamped to specified inclusive [<paramref name="min"/>; <paramref name="max"/>] range.
    /// </summary>
    /// <param name="value">The value to clamp.</param>
    /// <param name="min">The lower bound of the range.</param>
    /// <param name="max">The upper bound of the range.</param>
    /// <returns>
    /// <paramref name="value"/> if <paramref name="min"/> ≤ <paramref name="value"/> ≤ <paramref name="max"/>.
    /// <br/>-or-<br/>
    /// <paramref name="min"/> if <paramref name="value"/> &lt; <paramref name="min"/>.
    /// <br/>-or-<br/>
    /// <paramref name="max"/> if <paramref name="value"/> &gt; <paramref name="max"/>.
    /// </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal Clamp(decimal value, decimal min, decimal max)
    {
#if TFF_MATH_CLAMP
        return Math.Clamp(value, min, max);
#else
        if (min > max)
            ThrowMinCannotBeGreaterThanMaxException(min, max);

        if (value < min)
            return min;
        else if (value > max)
            return max;

        return value;
#endif
    }

    /// <summary>
    /// Returns a <see cref="float"/> <paramref name="value"/> clamped to specified inclusive [<paramref name="min"/>; <paramref name="max"/>] range.
    /// </summary>
    /// <param name="value">The value to clamp.</param>
    /// <param name="min">The lower bound of the range.</param>
    /// <param name="max">The upper bound of the range.</param>
    /// <returns>
    /// <paramref name="value"/> if <paramref name="min"/> ≤ <paramref name="value"/> ≤ <paramref name="max"/>.
    /// <br/>-or-<br/>
    /// <paramref name="min"/> if <paramref name="value"/> &lt; <paramref name="min"/>.
    /// <br/>-or-<br/>
    /// <paramref name="max"/> if <paramref name="value"/> &gt; <paramref name="max"/>.
    /// </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Clamp(float value, float min, float max)
    {
#if TFF_MATH_CLAMP
        return Math.Clamp(value, min, max);
#else
        if (min > max)
            ThrowMinCannotBeGreaterThanMaxException(min, max);

        if (value < min)
            return min;
        else if (value > max)
            return max;

        return value;
#endif
    }

    /// <summary>
    /// Returns a <see cref="double"/> <paramref name="value"/> clamped to specified inclusive [<paramref name="min"/>; <paramref name="max"/>] range.
    /// </summary>
    /// <param name="value">The value to clamp.</param>
    /// <param name="min">The lower bound of the range.</param>
    /// <param name="max">The upper bound of the range.</param>
    /// <returns>
    /// <paramref name="value"/> if <paramref name="min"/> ≤ <paramref name="value"/> ≤ <paramref name="max"/>.
    /// <br/>-or-<br/>
    /// <paramref name="min"/> if <paramref name="value"/> &lt; <paramref name="min"/>.
    /// <br/>-or-<br/>
    /// <paramref name="max"/> if <paramref name="value"/> &gt; <paramref name="max"/>.
    /// </returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Clamp(double value, double min, double max)
    {
#if TFF_MATH_CLAMP
        return Math.Clamp(value, min, max);
#else
        if (min > max)
            ThrowMinCannotBeGreaterThanMaxException(min, max);

        if (value < min)
            return min;
        else if (value > max)
            return max;

        return value;
#endif
    }
}
