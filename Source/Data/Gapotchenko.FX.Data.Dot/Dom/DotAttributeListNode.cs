using System;
using System.Collections.Generic;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    /// <summary>
    /// Represents DOT document attribute list.
    /// </summary>
    public sealed class DotAttributeListNode : DotNode
    {
        /// <summary>
        /// Gets or sets a <c>[</c> token.
        /// </summary>
        public DotPunctuationToken? OpenBraceToken { get; set; }

        /// <summary>
        /// Gets or sets an attribute list.
        /// </summary>
        public IList<DotAttributeNode>? Attributes { get; set; }

        /// <summary>
        /// Gets or sets a <c>]</c> token.
        /// </summary>
        public DotPunctuationToken? CloseBraceToken { get; set; }

        internal override int SlotCount => 3;

        internal override IDotSyntaxSlotProvider? GetSlot(int i) => i switch
        {
            0 => OpenBraceToken,
            1 => ListDotSyntaxSlotProvider.Create(Attributes),
            2 => CloseBraceToken,
            _ => throw new ArgumentOutOfRangeException(nameof(i))
        };

        /// <inheritdoc />
        public override void Accept(DotDomVisitor visitor)
        {
            visitor.VisitDotAttributeListNode(this);
        }
    }
}