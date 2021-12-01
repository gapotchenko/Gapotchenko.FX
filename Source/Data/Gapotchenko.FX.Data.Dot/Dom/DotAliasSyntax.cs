using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    public class DotAliasSyntax : DotStatementSyntax
    {
        public DotSyntaxToken LHS { get; }
        public DotSyntaxToken EqualToken { get; }
        public DotSyntaxToken RHS { get; }
    }
}
