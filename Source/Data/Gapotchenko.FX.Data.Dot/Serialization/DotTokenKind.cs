using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Dot.Serialization
{
    /// <summary>
    /// DOT document tokens.
    /// </summary>
    public enum DotTokenKind
    {
        /// <summary>
        /// Represents <c>EOF</c> token.
        /// </summary>
        EOF = 128,

        /// <summary>
        /// Represents <c>digraph</c> token.
        /// </summary>
        Digraph,

        /// <summary>
        /// Represents <c>graph</c> token.
        /// </summary>
        Graph,

        /// <summary>
        /// Represents <c>--</c> or <c>-></c> token.
        /// </summary>
        Arrow,

        /// <summary>
        /// Represents <c>subgraph</c> token.
        /// </summary>
        Subgraph,

        /// <summary>
        /// Represents <c>node</c> token.
        /// </summary>
        Node,

        /// <summary>
        /// Represents <c>edge</c> token.
        /// </summary>
        Edge,

        /// <summary>
        /// Represents an identifier token.
        /// </summary>
        Id,

        /// <summary>
        /// Represents a comment token.
        /// </summary>
        Comment,

        /// <summary>
        /// Represents a multiline comment token.
        /// </summary>
        MultilineComment,

        /// <summary>
        /// Represents a whitespace token.
        /// </summary>
        Whitespace,
    }
}
