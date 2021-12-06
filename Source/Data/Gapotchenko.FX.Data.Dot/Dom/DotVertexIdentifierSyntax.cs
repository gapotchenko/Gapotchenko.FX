using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    /// <summary>
    /// Represents DOT document vertex identifier.
    /// </summary>
    public sealed class DotVertexIdentifierSyntax : DotSyntaxNode
    {
        /// <summary>
        /// Gets or sets a vertex identifier token.
        /// </summary>
        public DotSyntaxToken? Identifier { get; set; }

        /// <summary>
        /// Gets or sets a port <c>:</c> token.
        /// </summary>
        public DotSyntaxToken? PortColonToken { get; set; }

        /// <summary>
        /// Gets or sets a port identifier token.
        /// </summary>
        public DotSyntaxToken? PortIdentifier { get; set; }

        /// <summary>
        /// Gets or sets a compass point <c>:</c> token.
        /// </summary>
        public DotSyntaxToken? CompassPointColonToken { get; set; }

        /// <summary>
        /// Gets or sets a compass point token.
        /// </summary>
        public DotSyntaxToken? CompassPointToken { get; set; }

        internal override int SlotCount => 5;

        internal override SyntaxSlot GetSlot(int i) => i switch
        {
            0 => Identifier,
            1 => PortColonToken,
            2 => PortIdentifier,
            3 => CompassPointColonToken,
            4 => CompassPointToken,
            _ => throw new ArgumentOutOfRangeException(nameof(i))
        };

        /// <inheritdoc />
        public override void Accept(DotSyntaxVisitor visitor)
        {
            visitor.VisitDotVertexIdentifierSyntax(this);
        }
    }
}
