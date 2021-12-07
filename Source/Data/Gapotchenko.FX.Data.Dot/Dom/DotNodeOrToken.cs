using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    /// <summary>
    /// A wrapper for either a syntax node (<see cref="DotNode"/>) or 
    /// a syntax token (<see cref="DotToken"/>).
    /// </summary>
    public struct DotNodeOrToken
    {
        readonly DotToken? _token;
        readonly DotNode? _node;

        /// <summary>
        /// Creates a <see cref="DotNodeOrToken"/> for a syntax token.
        /// </summary>
        /// <param name="token">Syntax token.</param>
        public DotNodeOrToken(DotToken token)
        {
            _token = token;
            IsToken = true;

            _node = null;
            IsNode = false;
        }

        /// <summary>
        /// Creates a <see cref="DotNodeOrToken"/> for a syntax node.
        /// </summary>
        /// <param name="node">Syntax node.</param>
        public DotNodeOrToken(DotNode node)
        {
            _token = null;
            IsToken = false;

            _node = node;
            IsNode = true;
        }

        /// <summary>
        /// Returns a new <see cref="DotNodeOrToken"/> that wraps the supplied token.
        /// </summary>
        /// <param name="token">Syntax token.</param>
        public static implicit operator DotNodeOrToken(DotToken token) =>
            new DotNodeOrToken(token);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node">Syntax node.</param>
        public static implicit operator DotNodeOrToken(DotNode node) =>
            new DotNodeOrToken(node);

        /// <summary>
        /// Determines whether this <see cref="DotNodeOrToken"/> is empty.
        /// </summary>
        public bool IsDefault => !IsNode && !IsToken;

        /// <summary>
        /// Determines whether this <see cref="DotNodeOrToken"/> is wrapping a node.
        /// </summary>
        public bool IsNode { get; }

        /// <summary>
        /// Determines whether this <see cref="DotNodeOrToken"/> is wrapping a token.
        /// </summary>
        public bool IsToken { get; }

        /// <summary>
        /// Returns the underlying token if this <see cref="DotNodeOrToken"/> is wrapping a token.
        /// </summary>
        public DotToken? AsToken() => _token;

        /// <summary>
        /// Returns the underlying node if this <see cref="DotNodeOrToken"/> is wrapping a node.
        /// </summary>
        public DotNode? AsNode() => _node;

        /// <summary>
        /// The list of trivia that appear before the underlying node or token and are attached to a
        /// token that is a descendant of the underlying node or token.
        /// </summary>
        public IList<DotTrivia>? GetLeadingTrivia() =>
            IsToken ? _token!.LeadingTrivia :
            IsNode ? _node!.LeadingTrivia :
            default;

        /// <summary>
        /// The list of trivia that appear after the underlying node or token and are attached to a
        /// token that is a descendant of the underlying node or token.
        /// </summary>
        public IList<DotTrivia>? GetTrailingTrivia() =>
            IsToken ? _token!.TrailingTrivia :
            IsNode ? _node!.TrailingTrivia :
            default;
    }
}
