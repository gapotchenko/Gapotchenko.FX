namespace Gapotchenko.FX.Data.Dot.Dom
{
    /// <summary>
    /// Represents a punctuation token in DOT document.
    /// </summary>
    public sealed class DotPunctuationToken : DotPrimitiveToken
    {
        /// <summary>
        /// Initializes a new <see cref="DotPunctuationToken"/> instance.
        /// </summary>
        /// <param name="kind">Token kind.</param>
        /// <param name="text">Token text.</param>
        public DotPunctuationToken(DotPunctuationTokenKind kind, string? text = default)
            : base(kind.ToDotTokenKind(), text)
        {
            Kind = kind;
        }

        /// <summary>
        /// Punctuation kind.
        /// </summary>
        public DotPunctuationTokenKind Kind { get; }
    }
}
