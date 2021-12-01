using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    public struct DotSyntaxNodeOrToken
    {
        readonly DotSyntaxToken? _token;
        readonly DotSyntaxNode? _node;

        public DotSyntaxNodeOrToken(DotSyntaxToken token)
        {
            _token = token;
            IsToken = true;

            _node = null;
            IsNode = false;
        }

        public DotSyntaxNodeOrToken(DotSyntaxNode node)
        {
            _token = null;
            IsToken = false;

            _node = node;
            IsNode = true;
        }

        public static implicit operator DotSyntaxNodeOrToken(DotSyntaxToken token) =>
            new DotSyntaxNodeOrToken(token);
        public static implicit operator DotSyntaxNodeOrToken(DotSyntaxNode node) =>
            new DotSyntaxNodeOrToken(node);

        public bool IsDefault => !IsNode && !IsToken;

        public bool IsNode { get; }
        public bool IsToken { get; }

        public DotSyntaxToken? AsToken() => _token;

        public DotSyntaxNode? AsNode() => _node;

        public List<DotSyntaxTrivia>? LeadingTrivia =>
            IsToken ? _token!.LeadingTrivia :
            IsNode ? _node!.LeadingTrivia :
            default;
        public List<DotSyntaxTrivia>? TrailingTrivia =>
            IsToken ? _token!.TrailingTrivia :
            IsNode ? _node!.TrailingTrivia :
            default;
    }
}
