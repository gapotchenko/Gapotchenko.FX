using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    public class DotGraphSyntax : DotStatementSyntax
    {
        public DotSyntaxToken StrictKeyword { get; }
        public DotSyntaxToken GraphKindKeyword { get; }
        public DotSyntaxToken Identifier { get; }
        public DotStatementListSyntax Statements { get; }
    }
}
