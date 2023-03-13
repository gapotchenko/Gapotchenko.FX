namespace Gapotchenko.FX.Data.Encoding;

partial class TextDataEncoding
{
    /// <summary>
    /// Provides facilities for a <see cref="TextDataEncoding"/> implementation.
    /// </summary>
    protected static class ImplementationFacilities
    {
        /// <summary>
        /// Capitalizes a character according to the specified options.
        /// </summary>
        /// <param name="c">The character to capitalize.</param>
        /// <param name="options">The options.</param>
        /// <returns>The capitalized character.</returns>
        public static char Capitalize(char c, DataEncodingOptions options) =>
            (options & CaseOptionsMask) switch
            {
                0 => c,
                DataEncodingOptions.Lowercase => char.ToLowerInvariant(c),
                DataEncodingOptions.Uppercase => char.ToUpperInvariant(c),
                _ => throw new ArgumentException("Data encoding options are invalid.", nameof(options))
            };

        /// <summary>
        /// Determines whether the specified characters are the same
        /// when compared using the specified comparison mode.
        /// </summary>
        /// <param name="a">The first character.</param>
        /// <param name="b">The second character.</param>
        /// <param name="isCaseSensitive">The character comparison mode.</param>
        /// <returns>
        /// <see langword="true"/> if the characters are the same;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public static bool CharEqual(char a, char b, bool isCaseSensitive)
        {
            if (isCaseSensitive)
            {
                return a == b;
            }
            else
            {
                Span<char> span = stackalloc char[] { a, b };
                return MemoryExtensions.Equals(span.Slice(0, 1), span.Slice(1, 1), StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}
