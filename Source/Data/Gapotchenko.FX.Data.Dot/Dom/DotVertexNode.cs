using System;
using System.Collections.Generic;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    /// <summary>
    /// Represents DOT document vertex.
    /// </summary>
    public sealed class DotVertexNode : DotStatementNode
    {
        /// <summary>
        /// Gets or sets a vertex identifier.
        /// </summary>
        public DotVertexIdentifierNode? Identifier { get; set; }

        /// <summary>
        /// Gets or sets a list of attributes.
        /// </summary>
        public IList<DotAttributeListNode>? Attributes { get; set; }

        internal override int SlotCount => 3;

        internal override IDotSyntaxSlotProvider? GetSlot(int i) => i switch
        {
            0 => Identifier,
            1 => ListDotSyntaxSlotProvider.Create(Attributes),
            2 => SemicolonToken,
            _ => throw new ArgumentOutOfRangeException(nameof(i))
        };

        /// <inheritdoc />
        public override void Accept(DotDomVisitor visitor)
        {
            visitor.VisitDotVertexNode(this);
        }
    }
}
