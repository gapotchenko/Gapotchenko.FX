using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    /// <summary>
    /// Represents DOT document edge.
    /// </summary>
    public sealed class DotEdgeNode : DotStatementNode
    {
        /// <summary>
        /// Gets or sets elements list.
        /// </summary>
        public SeparatedDotNodeList<DotNode>? Elements { get; set; }

        /// <summary>
        /// Gets or sets attributes list.
        /// </summary>
        public DotNodeList<DotAttributeListNode>? Attributes { get; set; }

        internal override int SlotCount => 3;

        internal override SyntaxSlot GetSlot(int i) => i switch
        {
            0 => new SyntaxSlot(Elements),
            1 => new SyntaxSlot(Attributes),
            2 => SemicolonToken,
            _ => throw new ArgumentOutOfRangeException(nameof(i))
        };

        /// <inheritdoc />
        public override void Accept(DotDomVisitor visitor)
        {
            visitor.VisitDotEdgeNode(this);
        }
    }
}
