using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    public class DotVertexIdentifierSyntax : DotSyntaxNode
    {
        public DotSyntaxToken Identifier { get; set; }
        public DotSyntaxToken PortColonToken { get; set; }
        public DotSyntaxToken PortIdentifier { get; set; }
        public DotSyntaxToken CompassPointColonToken { get; set; }
        public DotSyntaxToken CompassPointToken { get; set; }
    }
}
