// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.Math.Intervals;

partial class Interval
{
    /// <summary>
    /// Converts the string representation of an interval to an equivalent <see cref="Interval{T}"/> object.
    /// </summary>
    /// <inheritdoc cref="Parse{T}(ReadOnlySpan{char}, IFormatProvider?, IComparer{T}?)"/>
    /// <param name="input">A string that contains an interval to convert.</param>
    /// <param name="provider"><inheritdoc/></param>
    /// <param name="comparer"><inheritdoc/></param>
    [return: NotNullIfNotNull(nameof(input))]
    public static Interval<T>? Parse<T>(string? input, IFormatProvider? provider = null, IComparer<T>? comparer = null) =>
        input is null ?
            null :
            Parse(input.AsSpan(), provider, comparer);

    /// <summary>
    /// Converts the specified read-only span of characters that represents an interval to an equivalent <see cref="Interval{T}"/> object.
    /// </summary>
    /// <typeparam name="T">The type of interval values.</typeparam>
    /// <param name="input">A read-only span of characters that contains an interval to convert.</param>
    /// <param name="provider">An object the supplies culture-specific formatting information about <paramref name="input"/>.</param>
    /// <param name="comparer">
    /// An <see cref="IComparer{T}"/> implementation to use when comparing values in the interval,
    /// or <see langword="null"/> to use the default <see cref="IComparer{T}"/> implementation for the type <typeparamref name="T"/>.
    /// </param>
    /// <returns>An object that is equivalent to the interval specified in the <paramref name="input"/> parameter.</returns>
    /// <exception cref="ArgumentException"><paramref name="provider"/> cannot be converted to culture information.</exception>
    /// <exception cref="FormatException"><paramref name="input"/> has an invalid format.</exception>
    /// <exception cref="NotSupportedException"><typeparamref name="T"/> type does not support conversion from string.</exception>
    public static Interval<T> Parse<T>(ReadOnlySpan<char> input, IFormatProvider? provider = null, IComparer<T>? comparer = null) =>
        Interval<T>.Create(
            IntervalParser.Parse<T>(input, provider),
            comparer);

    // ------------------------------------------------------------------------

    /// <summary>
    /// Tries to convert the specified read-only span of characters that represents an interval to an equivalent <see cref="Interval{T}"/> object,
    /// and returns a value that indicates whether the conversion succeeded.
    /// </summary>
    /// <inheritdoc cref="TryParse{T}(ReadOnlySpan{char}, IFormatProvider?, IComparer{T}?)"/>
    /// <param name="input"><inheritdoc/></param>
    /// <param name="result">
    /// When this method returns, contains the <see cref="Interval{T}"/> equivalent of the interval that is contained in <paramref name="input"/>, if the conversion succeeded;
    /// or <see langword="null"/> if the conversion failed.
    /// </param>
    /// <param name="provider"><inheritdoc/></param>
    /// <param name="comparer"><inheritdoc/></param>
    /// <returns><see langword="true"/> if the <paramref name="input"/> parameter was converted successfully; otherwise, <see langword="false"/>.</returns>
    public static bool TryParse<T>(
        ReadOnlySpan<char> input,
        [NotNullWhen(true)] out Interval<T>? result,
        IFormatProvider? provider = null,
        IComparer<T>? comparer = null) =>
        (result = TryParse(input, provider, comparer)) is not null;

    // ------------------------------------------------------------------------

    /// <summary>
    /// Tries to convert the specified read-only span of characters that represents an interval to an equivalent <see cref="Interval{T}"/> object.
    /// </summary>
    /// <typeparam name="T">The type of interval values.</typeparam>
    /// <param name="input">A read-only span of characters that contains an interval to convert.</param>
    /// <param name="provider">An object the supplies culture-specific formatting information about <paramref name="input"/>.</param>
    /// <param name="comparer">
    /// An <see cref="IComparer{T}"/> implementation to use when comparing values in the interval,
    /// or <see langword="null"/> to use the default <see cref="IComparer{T}"/> implementation for the type <typeparamref name="T"/>.
    /// </param>
    /// <returns>
    /// An object that is equivalent to the interval specified in the <paramref name="input"/> parameter if conversion was successful;
    /// otherwise, <see langword="null"/>.
    /// </returns>
    /// <exception cref="ArgumentException"><paramref name="provider"/> cannot be converted to culture information.</exception>
    /// <exception cref="NotSupportedException"><typeparamref name="T"/> type does not support conversion from string.</exception>
    public static Interval<T>? TryParse<T>(ReadOnlySpan<char> input, IFormatProvider? provider = null, IComparer<T>? comparer = null) =>
        Interval<T>.Create(
            IntervalParser.TryParse<T>(input, provider),
            comparer);
}
