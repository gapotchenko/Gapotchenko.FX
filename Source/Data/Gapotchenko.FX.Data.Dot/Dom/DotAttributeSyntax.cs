using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    /// <summary>
    /// Represents DOT document attribute.
    /// </summary>
    public sealed class DotAttributeSyntax : DotSyntaxNode
    {
        /// <summary>
        /// Gets or sets a left-hand side of an attribute.
        /// </summary>
        public DotSyntaxToken? LHS { get; set; }

        /// <summary>
        /// Gets or sets a <c>=</c> token.
        /// </summary>
        public DotSyntaxToken? EqualToken { get; set; }

        /// <summary>
        /// Gets or sets a right-hand side of an attribute.
        /// </summary>
        public DotSyntaxToken? RHS { get; set; }

        /// <summary>
        /// Gets or sets an attribute terminator token.
        /// </summary>
        public DotSyntaxToken? SemicolonOrCommaToken { get; set; }

        internal override int SlotCount => 4;

        internal override SyntaxSlot GetSlot(int i) => i switch
        {
            0 => LHS,
            1 => EqualToken,
            2 => RHS,
            3 => SemicolonOrCommaToken,
            _ => throw new ArgumentOutOfRangeException(nameof(i))
        };

        /// <inheritdoc />
        public override void Accept(DotSyntaxVisitor visitor)
        {
            visitor.VisitDotAttributeSyntax(this);
        }
    }
}
