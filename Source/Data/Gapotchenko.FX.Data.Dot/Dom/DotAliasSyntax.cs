using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    /// <summary>
    /// Represents DOT document alias.
    /// </summary>
    public sealed class DotAliasSyntax : DotStatementSyntax
    {
        /// <summary>
        /// Gets or sets a left-hand side of a statement.
        /// </summary>
        public DotSyntaxToken? LHS { get; set; }

        /// <summary>
        /// Gets or sets a <c>=</c> token.
        /// </summary>
        public DotSyntaxToken? EqualToken { get; set; }

        /// <summary>
        /// Gets or sets a right-hand side of a statement.
        /// </summary>
        public DotSyntaxToken? RHS { get; set; }

        internal override int SlotCount => 4;

        internal override SyntaxSlot GetSlot(int i) => i switch
        {
            0 => LHS,
            1 => EqualToken,
            2 => RHS,
            3 => SemicolonToken,
            _ => throw new ArgumentOutOfRangeException(nameof(i))
        };

        /// <inheritdoc />
        public override void Accept(DotSyntaxVisitor visitor)
        {
            visitor.VisitDotAliasSyntax(this);
        }
    }
}
