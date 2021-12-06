using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    /// <summary>
    /// A wrapper for either a syntax node (<see cref="DotSyntaxNode"/>) or 
    /// a syntax token (<see cref="DotSyntaxToken"/>).
    /// </summary>
    public struct DotSyntaxNodeOrToken
    {
        readonly DotSyntaxToken? _token;
        readonly DotSyntaxNode? _node;

        /// <summary>
        /// Creates a <see cref="DotSyntaxNodeOrToken"/> for a syntax token.
        /// </summary>
        /// <param name="token">Syntax token.</param>
        public DotSyntaxNodeOrToken(DotSyntaxToken token)
        {
            _token = token;
            IsToken = true;

            _node = null;
            IsNode = false;
        }

        /// <summary>
        /// Creates a <see cref="DotSyntaxNodeOrToken"/> for a syntax node.
        /// </summary>
        /// <param name="node">Syntax node.</param>
        public DotSyntaxNodeOrToken(DotSyntaxNode node)
        {
            _token = null;
            IsToken = false;

            _node = node;
            IsNode = true;
        }

        /// <summary>
        /// Returns a new <see cref="DotSyntaxNodeOrToken"/> that wraps the supplied token.
        /// </summary>
        /// <param name="token">Syntax token.</param>
        public static implicit operator DotSyntaxNodeOrToken(DotSyntaxToken token) =>
            new DotSyntaxNodeOrToken(token);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node">Syntax node.</param>
        public static implicit operator DotSyntaxNodeOrToken(DotSyntaxNode node) =>
            new DotSyntaxNodeOrToken(node);

        /// <summary>
        /// Determines whether this <see cref="DotSyntaxNodeOrToken"/> is empty.
        /// </summary>
        public bool IsDefault => !IsNode && !IsToken;

        /// <summary>
        /// Determines whether this <see cref="DotSyntaxNodeOrToken"/> is wrapping a node.
        /// </summary>
        public bool IsNode { get; }

        /// <summary>
        /// Determines whether this <see cref="DotSyntaxNodeOrToken"/> is wrapping a token.
        /// </summary>
        public bool IsToken { get; }

        /// <summary>
        /// Returns the underlying token if this <see cref="DotSyntaxNodeOrToken"/> is wrapping a token.
        /// </summary>
        public DotSyntaxToken? AsToken() => _token;

        /// <summary>
        /// Returns the underlying node if this <see cref="DotSyntaxNodeOrToken"/> is wrapping a node.
        /// </summary>
        public DotSyntaxNode? AsNode() => _node;

        /// <summary>
        /// The list of trivia that appear before the underlying node or token and are attached to a
        /// token that is a descendant of the underlying node or token.
        /// </summary>
        public List<DotSyntaxTrivia>? GetLeadingTrivia() =>
            IsToken ? _token!.LeadingTrivia :
            IsNode ? _node!.GetLeadingTrivia() :
            default;

        /// <summary>
        /// The list of trivia that appear after the underlying node or token and are attached to a
        /// token that is a descendant of the underlying node or token.
        /// </summary>
        public List<DotSyntaxTrivia>? GetTrailingTrivia() =>
            IsToken ? _token!.TrailingTrivia :
            IsNode ? _node!.GetTrailingTrivia() :
            default;
    }
}
