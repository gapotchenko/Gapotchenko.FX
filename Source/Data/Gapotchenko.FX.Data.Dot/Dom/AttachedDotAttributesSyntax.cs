using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    public class AttachedDotAttributesSyntax : DotStatementSyntax
    {
        public DotSyntaxToken TargetKeyword { get; set; }
        public DotSyntaxList<DotAttributeListSyntax> Attributes { get; set; }
    }
}
