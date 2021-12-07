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
    public sealed class DotGraphNode : DotStatementNode
    {
        /// <summary>
        /// Gets or sets <c>strict</c> keyword token.
        /// </summary>
        public DotToken? StrictKeyword { get; set; }

        /// <summary>
        /// Gets or sets a graph kind token.
        /// </summary>
        public DotToken? GraphKindKeyword { get; set; }

        /// <summary>
        /// Gets or sets a graph identifier token.
        /// </summary>
        public DotToken? Identifier { get; set; }

        /// <summary>
        /// Gets or sets a graph statements list.
        /// </summary>
        public DotStatementListNode? Statements { get; set; }

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
        public override void Accept(DotDomVisitor visitor)
        {
            visitor.VisitDotGraphNode(this);
        }
    }
}
