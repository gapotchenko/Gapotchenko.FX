using System;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    /// <summary>
    /// Represents DOT document alias.
    /// </summary>
    public sealed class DotAliasNode : DotStatementNode
    {
        /// <summary>
        /// Gets or sets a left-hand side of a statement.
        /// </summary>
        public DotStringLiteral? LHS { get; set; }

        /// <summary>
        /// Gets or sets a <c>=</c> token.
        /// </summary>
        public DotPunctuationToken? EqualToken { get; set; }

        /// <summary>
        /// Gets or sets a right-hand side of a statement.
        /// </summary>
        public DotStringLiteral? RHS { get; set; }

        internal override int SlotCount => 4;

        internal override IDotSyntaxSlotProvider? GetSlot(int i) => i switch
        {
            0 => LHS,
            1 => EqualToken,
            2 => RHS,
            3 => SemicolonToken,
            _ => throw new ArgumentOutOfRangeException(nameof(i))
        };

        /// <inheritdoc />
        public override void Accept(DotDomVisitor visitor)
        {
            visitor.VisitDotAliasNode(this);
        }
    }
}
