using System;

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
        public DotNodeList<DotAttributeListNode>? Attributes { get; set; }

        internal override int SlotCount => 3;

        internal override DotSyntaxSlot GetSlot(int index) =>
            index switch
            {
                0 => new DotSyntaxSlot(Elements),
                1 => new DotSyntaxSlot(Attributes),
                2 => SemicolonToken,
                _ => throw new ArgumentOutOfRangeException(nameof(index))
            };

        /// <inheritdoc />
        public override void Accept(DotDomVisitor visitor) => visitor.VisitDotEdgeNode(this);
    }
}
