using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    /// <summary>
    /// Represents DOT document statement.
    /// </summary>
    public abstract class DotStatementSyntax : DotSyntaxNode
    {
        /// <summary>
        /// Gets or sets a statement terminator token.
        /// </summary>
        public DotSyntaxToken? SemicolonToken { get; set; }
    }
}
