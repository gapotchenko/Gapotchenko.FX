namespace Gapotchenko.FX.Data.Dot.Dom
{
    /// <summary>
    /// Represents an arrow token in DOT document.
    /// </summary>
    public sealed class DotArrowToken : DotPrimitiveToken
    {
        /// <summary>
        /// Initializes a new <see cref="DotArrowToken"/> instance.
        /// </summary>
        /// <param name="kind">Token kind.</param>
        /// <param name="text">Token text.</param>
        public DotArrowToken(DotArrowTokenKind kind, string? text = default)
            : base(kind.ToDotTokenKind(), text)
        {
            Kind = kind;
        }

        /// <summary>
        /// Arrow kind.
        /// </summary>
        public DotArrowTokenKind Kind { get; }
    }
}
