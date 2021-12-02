using System;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    public sealed class DotAttributeListSyntax : DotSyntaxNode
    {
        public DotSyntaxToken OpenBraceToken { get; set; }

        public DotSyntaxList<DotAttributeSyntax> Attributes { get; set; }

        public DotSyntaxToken CloseBraceToken { get; set; }

        internal override int SlotCount => 3;

        internal override SyntaxSlot GetSlot(int i) => i switch
        {
            0 => OpenBraceToken,
            1 => new SyntaxSlot(Attributes),
            2 => CloseBraceToken,
            _ => throw new ArgumentOutOfRangeException(nameof(i))
        };

        public override void Accept(DotSyntaxVisitor visitor)
        {
            visitor.VisitDotAttributeListSyntax(this);
        }
    }
}