using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    public class DotVertexIdentifierSyntax : DotSyntaxNode
    {
        public DotSyntaxToken Identifier { get; }
        public DotSyntaxToken PortColonToken { get; }
        public DotSyntaxToken PortIdentifier { get; }
        public DotSyntaxToken CompassPointColonToken { get; }
        public DotSyntaxToken CompassPointToken { get; }
    }
}
