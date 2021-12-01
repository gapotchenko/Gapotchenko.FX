using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    public abstract class DotSyntaxNode
    {
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
