using System;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    /// <summary>
    /// Represents DOT document attribute.
    /// </summary>
    public sealed class DotAttributeNode : DotNode
    {
        /// <summary>
        /// Gets or sets a left-hand side of an attribute.
        /// </summary>
        public DotStringLiteral? LHS { get; set; }

        /// <summary>
        /// Gets or sets a <c>=</c> token.
        /// </summary>
        public DotPunctuationToken? EqualToken { get; set; }

        /// <summary>
        /// Gets or sets a right-hand side of an attribute.
        /// </summary>
        public DotStringLiteral? RHS { get; set; }

        /// <summary>
        /// Gets or sets an attribute terminator token.
        /// </summary>
        public DotPunctuationToken? SemicolonOrCommaToken { get; set; }

        internal override int SlotCount => 4;

        internal override DotSyntaxSlot GetSlot(int i) => i switch
        {
            0 => LHS,
            1 => EqualToken,
            2 => RHS,
            3 => SemicolonOrCommaToken,
            _ => throw new ArgumentOutOfRangeException(nameof(i))
        };

        /// <inheritdoc />
        public override void Accept(DotDomVisitor visitor)
        {
            visitor.VisitDotAttributeNode(this);
        }
    }
}
