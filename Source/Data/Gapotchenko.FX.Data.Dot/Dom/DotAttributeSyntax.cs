using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    public sealed class DotAttributeSyntax : DotSyntaxNode
    {
        public DotSyntaxToken LHS { get; set; }
        public DotSyntaxToken EqualToken { get; set; }
        public DotSyntaxToken RHS { get; set; }
        public DotSyntaxToken SemicolonOrCommaToken { get; set; }

        internal override int SlotCount => 4;

        internal override SyntaxSlot GetSlot(int i) => i switch
        {
            0 => LHS,
            1 => EqualToken,
            2 => RHS,
            3 => SemicolonOrCommaToken,
            _ => throw new ArgumentOutOfRangeException(nameof(i))
        };
    }
}
