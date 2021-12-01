using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    public class DotAttributeSyntax : DotSyntaxNode
    {
        public DotSyntaxToken LHS { get; set; }
        public DotSyntaxToken EqualToken { get; set; }
        public DotSyntaxToken RHS { get; set; }
        public DotSyntaxToken SemicolonOrCommaToken { get; set; }
    }
}
