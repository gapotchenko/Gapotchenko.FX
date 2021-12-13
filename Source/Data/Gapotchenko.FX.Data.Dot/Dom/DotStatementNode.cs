namespace Gapotchenko.FX.Data.Dot.Dom
{
    /// <summary>
    /// Represents DOT document statement.
    /// </summary>
    public abstract class DotStatementNode : DotNode
    {
        /// <summary>
        /// Gets or sets a statement terminator token.
        /// </summary>
        public DotPunctuationToken? SemicolonToken { get; set; }
    }
}
