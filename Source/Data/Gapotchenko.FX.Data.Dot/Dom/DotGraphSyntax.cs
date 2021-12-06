using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    /// <summary>
    /// Represents DOT document graph.
    /// </summary>
    public sealed class DotGraphSyntax : DotStatementSyntax
    {
        /// <summary>
        /// Gets or sets <c>strict</c> keyword token.
        /// </summary>
        public DotSyntaxToken? StrictKeyword { get; set; }

        /// <summary>
        /// Gets or sets a graph kind token.
        /// </summary>
        public DotSyntaxToken? GraphKindKeyword { get; set; }

        /// <summary>
        /// Gets or sets a graph identifier token.
        /// </summary>
        public DotSyntaxToken? Identifier { get; set; }

        /// <summary>
        /// Gets or sets a graph statements list.
        /// </summary>
        public DotStatementListSyntax? Statements { get; set; }

        internal override int SlotCount => 5;

        internal override SyntaxSlot GetSlot(int i) => i switch
        {
            0 => StrictKeyword,
            1 => GraphKindKeyword,
            2 => Identifier,
            3 => Statements,
            4 => SemicolonToken,
            _ => throw new ArgumentOutOfRangeException(nameof(i))
        };

        /// <inheritdoc />
        public override void Accept(DotSyntaxVisitor visitor)
        {
            visitor.VisitDotGraphSyntax(this);
        }
    }
}
