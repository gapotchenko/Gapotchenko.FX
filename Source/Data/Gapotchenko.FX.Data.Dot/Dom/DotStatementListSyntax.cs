using System;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    /// <summary>
    /// Represents DOT document statement list.
    /// </summary>
    public sealed class DotStatementListSyntax : DotSyntaxNode
    {
        /// <summary>
        /// Gets or sets <c>{</c> token.
        /// </summary>
        public DotSyntaxToken? OpenBraceToken { get; set; }

        /// <summary>
        /// Gets or sets a list of statements.
        /// </summary>
        public DotSyntaxList<DotStatementSyntax>? Statements { get; set; }

        /// <summary>
        /// Gets or sets <c>}</c> token.
        /// </summary>
        public DotSyntaxToken? CloseBraceToken { get; set; }

        internal override int SlotCount => 3;

        internal override SyntaxSlot GetSlot(int i) => i switch
        {
            0 => OpenBraceToken,
            1 => new SyntaxSlot(Statements),
            2 => CloseBraceToken,
            _ => throw new ArgumentOutOfRangeException(nameof(i))
        };

        /// <inheritdoc />
        public override void Accept(DotSyntaxVisitor visitor)
        {
            visitor.VisitDotStatementListSyntax(this);
        }
    }
}