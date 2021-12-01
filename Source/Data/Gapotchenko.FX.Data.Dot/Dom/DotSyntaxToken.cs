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

        List<DotSyntaxTrivia>? _leadingTrivia;
        List<DotSyntaxTrivia>? _trailingTrivia;

        public List<DotSyntaxTrivia> LeadingTrivia =>
            _leadingTrivia ??= new();

        public List<DotSyntaxTrivia> TrailingTrivia =>
            _trailingTrivia ??= new();

        public bool HasLeadingTrivia => _leadingTrivia?.Count > 0;

        public bool HasTrailingTrivia => _trailingTrivia?.Count > 0;
    }
}
