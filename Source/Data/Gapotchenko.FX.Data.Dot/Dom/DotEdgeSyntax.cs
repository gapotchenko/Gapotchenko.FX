using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    public sealed class DotEdgeSyntax : DotStatementSyntax
    {
        public SeparatedDotSyntaxList<DotSyntaxNode> Elements { get; set; }
        public DotSyntaxList<DotAttributeListSyntax> Attributes { get; set; }

        internal override int SlotCount => 2;

        internal override SyntaxSlot GetSlot(int i) => i switch
        {
            0 => new SyntaxSlot(Elements),
            1 => new SyntaxSlot(Attributes),
            _ => throw new ArgumentOutOfRangeException(nameof(i))
        };
    }
}
