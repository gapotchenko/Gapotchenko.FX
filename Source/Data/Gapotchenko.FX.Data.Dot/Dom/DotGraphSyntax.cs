using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    public sealed class DotGraphSyntax : DotStatementSyntax
    {
        public DotSyntaxToken StrictKeyword { get; set; }
        public DotSyntaxToken GraphKindKeyword { get; set; }
        public DotSyntaxToken Identifier { get; set; }
        public DotStatementListSyntax Statements { get; set; }

        internal override int SlotCount => 4;

        internal override SyntaxSlot GetSlot(int i) => i switch
        {
            0 => StrictKeyword,
            1 => GraphKindKeyword,
            2 => Identifier,
            3 => Statements,
            _ => throw new ArgumentOutOfRangeException(nameof(i))
        };

        public override void Accept(DotSyntaxVisitor visitor)
        {
            visitor.VisitDotGraphSyntax(this);
        }
    }
}
