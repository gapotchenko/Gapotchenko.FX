using System;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    /// <summary>
    /// Represents DOT document vertex identifier.
    /// </summary>
    public sealed class DotVertexIdentifierNode : DotNode
    {
        /// <summary>
        /// Gets or sets a vertex identifier token.
        /// </summary>
        public DotStringLiteral? Identifier { get; set; }

        /// <summary>
        /// Gets or sets a port <c>:</c> token.
        /// </summary>
        public DotPunctuationToken? PortColonToken { get; set; }

        /// <summary>
        /// Gets or sets a port identifier token.
        /// </summary>
        public DotStringLiteral? PortIdentifier { get; set; }

        /// <summary>
        /// Gets or sets a compass point <c>:</c> token.
        /// </summary>
        public DotPunctuationToken? CompassPointColonToken { get; set; }

        /// <summary>
        /// Gets or sets a compass point token.
        /// </summary>
        public DotStringLiteral? CompassPointToken { get; set; }

        internal override int SlotCount => 5;

        internal override IDotSyntaxSlotProvider? GetSlot(int i) => i switch
        {
            0 => Identifier,
            1 => PortColonToken,
            2 => PortIdentifier,
            3 => CompassPointColonToken,
            4 => CompassPointToken,
            _ => throw new ArgumentOutOfRangeException(nameof(i))
        };

        /// <inheritdoc />
        public override void Accept(DotDomVisitor visitor)
        {
            if (visitor is null)
                throw new ArgumentNullException(nameof(visitor));

            visitor.VisitDotVertexIdentifierNode(this);
        }
    }
}
