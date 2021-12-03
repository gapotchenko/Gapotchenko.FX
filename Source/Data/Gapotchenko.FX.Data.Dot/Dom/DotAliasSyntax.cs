using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    public sealed class DotAliasSyntax : DotStatementSyntax
    {
        public DotSyntaxToken? LHS { get; set; }
        public DotSyntaxToken? EqualToken { get; set; }
        public DotSyntaxToken? RHS { get; set; }

        internal override int SlotCount => 4;

        internal override SyntaxSlot GetSlot(int i) => i switch
        {
            0 => LHS,
            1 => EqualToken,
            2 => RHS,
            3 => SemicolonToken,
            _ => throw new ArgumentOutOfRangeException(nameof(i))
        };

        public override void Accept(DotSyntaxVisitor visitor)
        {
            visitor.VisitDotAliasSyntax(this);
        }
    }
}
