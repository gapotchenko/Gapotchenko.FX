namespace Gapotchenko.FX.Data.Dot.Dom
{
    /// <summary>
    /// Punctuation DOT document tokens.
    /// </summary>
    public enum DotPunctuationTokenKind
    {
        /// <summary>
        /// Represents <c>{</c> token.
        /// </summary>
        ScopeStart,

        /// <summary>
        /// Represents <c>}</c> token.
        /// </summary>
        ScopeEnd,

        /// <summary>
        /// Represents <c>;</c> token.
        /// </summary>
        Semicolon,

        /// <summary>
        /// Represents <c>=</c> token.
        /// </summary>
        Equal,

        /// <summary>
        /// Represents <c>[</c> token.
        /// </summary>
        ListStart,

        /// <summary>
        /// Represents <c>]</c> token.
        /// </summary>
        ListEnd,

        /// <summary>
        /// Represents <c>,</c> token.
        /// </summary>
        Comma,

        /// <summary>
        /// Represents <c>:</c> token.
        /// </summary>
        Colon,
    }
}
