using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    /// <summary>
    /// Represents DOT document attached attribute.
    /// </summary>
    public sealed class AttachedDotAttributesSyntax : DotStatementSyntax
    {
        /// <summary>
        /// Gets or sets an attribute target.
        /// </summary>
        public DotSyntaxToken? TargetKeyword { get; set; }

        /// <summary>
        /// Gets or sets a list of attributes.
        /// </summary>
        public DotSyntaxList<DotAttributeListSyntax>? Attributes { get; set; }

        internal override int SlotCount => 3;

        internal override SyntaxSlot GetSlot(int i) => i switch
        {
            0 => TargetKeyword,
            1 => new SyntaxSlot(Attributes),
            2 => SemicolonToken,
            _ => throw new ArgumentOutOfRangeException(nameof(i))
        };

        /// <inheritdoc />
        public override void Accept(DotSyntaxVisitor visitor)
        {
            visitor.VisitAttachedDotAttributesSyntax(this);
        }
    }
}
