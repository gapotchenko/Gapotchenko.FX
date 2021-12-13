using System;
using System.Collections.Generic;

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
        public SeparatedDotNodeList<DotNode>? Elements { get; init; }

        /// <summary>
        /// Gets or sets attributes list.
        /// </summary>
        public IList<DotAttributeListNode>? Attributes { get; set; }

        internal override int SlotCount => 3;

        internal override IDotSyntaxSlotProvider? GetSlot(int index) =>
            index switch
            {
                0 => Elements,
                1 => ListDotSyntaxSlotProvider.Create(Attributes),
                2 => SemicolonToken,
                _ => throw new ArgumentOutOfRangeException(nameof(index))
            };

        /// <inheritdoc />
        public override void Accept(DotDomVisitor visitor) => visitor.VisitDotEdgeNode(this);
    }
}
