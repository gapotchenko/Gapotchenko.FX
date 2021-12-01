using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    public abstract class DotStatementSyntax : DotSyntaxNode
    {
        public DotSyntaxToken SemicolonToken { get; set; }
    }
}
