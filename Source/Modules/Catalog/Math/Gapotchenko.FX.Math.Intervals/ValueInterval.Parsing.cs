// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.Math.Intervals;

partial class ValueInterval
{
    /// <summary>
    /// Converts the specified read-only span of characters that represents an interval to an equivalent <see cref="ValueInterval{T}"/> object.
    /// </summary>
    /// <typeparam name="T">The type of interval values.</typeparam>
    /// <param name="input">A read-only span of characters that contains an interval to convert.</param>
    /// <param name="provider">An object the supplies culture-specific formatting information about <paramref name="input"/>.</param>
    /// <returns>An object that is equivalent to the interval specified in the <paramref name="input"/> parameter.</returns>
    /// <exception cref="ArgumentException"><paramref name="provider"/> does not provide culture information.</exception>
    /// <exception cref="FormatException"><paramref name="input"/> has an invalid format.</exception>
    /// <exception cref="NotSupportedException"><typeparamref name="T"/> type does not support conversion from string.</exception>
    public static ValueInterval<T> Parse<T>(ReadOnlySpan<char> input, IFormatProvider? provider = null)
        where T : IEquatable<T>?, IComparable<T>? =>
        new(IntervalParser.Parse<T>(input, provider));

    // ------------------------------------------------------------------------

    /// <summary>
    /// Tries to convert the specified read-only span of characters that represents an interval to an equivalent <see cref="ValueInterval{T}"/> object,
    /// and returns a value that indicates whether the conversion succeeded.
    /// </summary>
    /// <inheritdoc cref="TryParse{T}(ReadOnlySpan{char}, IFormatProvider?)"/>
    /// <param name="input"><inheritdoc/></param>
    /// <param name="result">
    /// When this method returns, contains the <see cref="ValueInterval{T}"/> equivalent of the interval that is contained in <paramref name="input"/>, if the conversion succeeded;
    /// or <see langword="null"/> if the conversion failed.
    /// </param>
    /// <param name="provider"><inheritdoc/></param>
    /// <returns><see langword="true"/> if the <paramref name="input"/> parameter was converted successfully; otherwise, <see langword="false"/>.</returns>
    public static bool TryParse<T>(
        ReadOnlySpan<char> input,
        out ValueInterval<T> result,
        IFormatProvider? provider = null)
        where T : IEquatable<T>?, IComparable<T>?
    {
        if (TryParse<T>(input, provider) is { } value)
        {
            result = value;
            return true;
        }
        else
        {
            result = default;
            return false;
        }
    }

    // ------------------------------------------------------------------------

    /// <summary>
    /// Tries to convert the specified read-only span of characters that represents an interval to an equivalent <see cref="ValueInterval{T}"/> object.
    /// </summary>
    /// <typeparam name="T">The type of interval values.</typeparam>
    /// <param name="input">A read-only span of characters that contains an interval to convert.</param>
    /// <param name="provider">An object the supplies culture-specific formatting information about <paramref name="input"/>.</param>
    /// <returns>
    /// An object that is equivalent to the interval specified in the <paramref name="input"/> parameter if conversion was successful;
    /// otherwise, <see langword="null"/>.
    /// </returns>
    /// <exception cref="ArgumentException"><paramref name="provider"/> does not provide culture information.</exception>
    /// <exception cref="NotSupportedException"><typeparamref name="T"/> type does not support conversion from string.</exception>
    public static ValueInterval<T>? TryParse<T>(ReadOnlySpan<char> input, IFormatProvider? provider = null)
        where T : IEquatable<T>?, IComparable<T>? =>
        ValueInterval<T>.Create(IntervalParser.TryParse<T>(input, provider));
}
