using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    public class DotAliasSyntax : DotStatementSyntax
    {
        public DotSyntaxToken LHS { get; set; }
        public DotSyntaxToken EqualToken { get; set; }
        public DotSyntaxToken RHS { get; set; }
    }
}
