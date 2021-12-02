using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    /// <summary>
    /// Syntax the <see cref="DotSyntaxWalker"/> should descend into.
    /// </summary>
    public enum SyntaxWalkerDepth : int
    {
        /// <summary>
        /// Descend into only nodes.
        /// </summary>
        Node = 0,

        /// <summary>
        /// Descend into nodes and tokens.
        /// </summary>
        Token = 1,

        /// <summary>
        /// Descend into nodes, tokens and trivia.
        /// </summary>
        Trivia = 2
    }
}
