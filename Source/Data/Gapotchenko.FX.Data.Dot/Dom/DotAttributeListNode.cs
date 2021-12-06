using System;

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
        public DotToken? OpenBraceToken { get; set; }

        /// <summary>
        /// Gets or sets an attribute list.
        /// </summary>
        public DotNodeList<DotAttributeNode>? Attributes { get; set; }

        /// <summary>
        /// Gets or sets a <c>]</c> token.
        /// </summary>
        public DotToken? CloseBraceToken { get; set; }

        internal override int SlotCount => 3;

        internal override SyntaxSlot GetSlot(int i) => i switch
        {
            0 => OpenBraceToken,
            1 => new SyntaxSlot(Attributes),
            2 => CloseBraceToken,
            _ => throw new ArgumentOutOfRangeException(nameof(i))
        };

        /// <inheritdoc />
        public override void Accept(DotSyntaxVisitor visitor)
        {
            visitor.VisitDotAttributeListNode(this);
        }
    }
}