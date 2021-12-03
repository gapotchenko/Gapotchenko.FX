using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    public sealed class DotVertexIdentifierSyntax : DotSyntaxNode
    {
        public DotSyntaxToken Identifier { get; set; }
        public DotSyntaxToken? PortColonToken { get; set; }
        public DotSyntaxToken? PortIdentifier { get; set; }
        public DotSyntaxToken? CompassPointColonToken { get; set; }
        public DotSyntaxToken? CompassPointToken { get; set; }

        internal override int SlotCount => 5;

        internal override SyntaxSlot GetSlot(int i) => i switch
        {
            0 => Identifier,
            1 => PortColonToken,
            2 => PortIdentifier,
            3 => CompassPointColonToken,
            4 => CompassPointToken,
            _ => throw new ArgumentOutOfRangeException(nameof(i))
        };

        public override void Accept(DotSyntaxVisitor visitor)
        {
            visitor.VisitDotVertexIdentifierSyntax(this);
        }
    }
}
