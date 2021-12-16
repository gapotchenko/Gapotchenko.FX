using System;
using System.Collections.Generic;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    /// <summary>
    /// Represents DOT document attached attribute.
    /// </summary>
    public sealed class AttachedDotAttributesNode : DotStatementNode
    {
        /// <summary>
        /// Gets or sets an attribute target.
        /// </summary>
        public DotKeywordToken? TargetKeyword { get; set; }

        /// <summary>
        /// Gets or sets a list of attributes.
        /// </summary>
        public IList<DotAttributeListNode>? Attributes { get; set; }

        internal override int SlotCount => 3;

        internal override IDotSyntaxSlotProvider? GetSlot(int i) => i switch
        {
            0 => TargetKeyword,
            1 => ListDotSyntaxSlotProvider.Create(Attributes),
            2 => SemicolonToken,
            _ => throw new ArgumentOutOfRangeException(nameof(i))
        };

        /// <inheritdoc />
        public override void Accept(DotDomVisitor visitor)
        {
            if (visitor is null)
                throw new ArgumentNullException(nameof(visitor));

            visitor.VisitAttachedDotAttributesNode(this);
        }
    }
}
