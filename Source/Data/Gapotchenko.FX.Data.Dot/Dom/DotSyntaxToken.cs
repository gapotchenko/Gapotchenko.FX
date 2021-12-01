using Gapotchenko.FX.Data.Dot.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    /// <summary>
    /// Represents a token in the syntax tree.
    /// </summary>
    public class DotSyntaxToken
    {
        public DotSyntaxToken(DotToken token, string value)
        {
            Token = token;
            Value = value;
        }

        public DotToken Token { get; }
        public string Value { get; }

        public List<DotSyntaxTrivia> LeadingTrivia { get; }
        public List<DotSyntaxTrivia> TrailingTrivia { get; }
    }
}
