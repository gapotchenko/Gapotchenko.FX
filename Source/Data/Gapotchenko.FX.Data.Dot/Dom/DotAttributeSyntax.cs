using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    public class DotAttributeSyntax : DotSyntaxNode
    {
        public DotSyntaxToken LHS { get; }
        public DotSyntaxToken EqualToken { get; }
        public DotSyntaxToken RHS { get; }
        public DotSyntaxToken SemicolonOrCommaToken { get; set; }
    }
}
