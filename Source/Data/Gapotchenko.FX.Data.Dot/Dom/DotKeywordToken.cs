using Gapotchenko.FX.Data.Dot.Serialization;
using System;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    /// <summary>
    /// Represents a keyword token in DOT document.
    /// </summary>
    public sealed class DotKeywordToken : DotSignificantToken
    {
        /// <summary>
        /// Initializes a new <see cref="DotKeywordToken"/> instance.
        /// </summary>
        /// <param name="kind">Token kind.</param>
        /// <param name="text">Token text.</param>
        public DotKeywordToken(DotTokenKind kind, string? text = default)
            : base(kind, text ?? kind.GetDefaultValue())
        {
            switch (kind)
            {
                case DotTokenKind.Digraph:
                case DotTokenKind.Graph:
                case DotTokenKind.Subgraph:
                case DotTokenKind.Node:
                case DotTokenKind.Edge:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(kind));
            }
        }
    }
}
