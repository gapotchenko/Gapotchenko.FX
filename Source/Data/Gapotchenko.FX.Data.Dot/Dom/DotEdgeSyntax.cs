using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    public class DotEdgeSyntax : DotStatementSyntax
    {
        public SeparatedDotSyntaxList<DotSyntaxNode> Elements { get; set; }
        public DotSyntaxList<DotAttributeListSyntax> Attributes { get; set; }
    }
}
