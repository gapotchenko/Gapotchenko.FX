using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    public struct DotSyntaxNodeOrToken
    {
        public bool IsDefault { get; }

        public bool IsNode { get; }
        public bool IsToken { get; }

        public DotSyntaxToken AsToken()
        {
            throw new NotImplementedException();
        }

        public DotSyntaxNode AsNode()
        {
            throw new NotImplementedException();
        }

        public List<DotSyntaxTrivia> LeadingTrivia { get; }
        public List<DotSyntaxTrivia> TrailingTrivia { get; }
    }
}
