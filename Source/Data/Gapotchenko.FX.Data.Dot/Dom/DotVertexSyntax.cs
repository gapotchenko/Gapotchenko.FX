using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    public sealed class DotVertexSyntax : DotStatementSyntax
    {
        public DotVertexIdentifierSyntax? Identifier { get; set; }
        public DotSyntaxList<DotAttributeListSyntax>? Attributes { get; set; }

        internal override int SlotCount => 2;

        internal override SyntaxSlot GetSlot(int i) => i switch
        {
            0 => Identifier,
            1 => new SyntaxSlot(Attributes),
            _ => throw new ArgumentOutOfRangeException(nameof(i))
        };

        public override void Accept(DotSyntaxVisitor visitor)
        {
            visitor.VisitDotVertexSyntax(this);
        }
    }
}
