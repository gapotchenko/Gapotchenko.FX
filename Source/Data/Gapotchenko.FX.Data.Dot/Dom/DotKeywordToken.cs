namespace Gapotchenko.FX.Data.Dot.Dom
{
    /// <summary>
    /// Represents a keyword token in DOT document.
    /// </summary>
    public sealed class DotKeywordToken : DotPrimitiveToken
    {
        /// <summary>
        /// Initializes a new <see cref="DotKeywordToken"/> instance.
        /// </summary>
        /// <param name="kind">Token kind.</param>
        /// <param name="text">Token text.</param>
        public DotKeywordToken(DotKeywordTokenKind kind, string? text = default)
            : base(kind.ToDotTokenKind(), text)
        {
            Kind = kind;
        }

        /// <summary>
        /// Keyword kind.
        /// </summary>
        public DotKeywordTokenKind Kind { get; }
    }
}
