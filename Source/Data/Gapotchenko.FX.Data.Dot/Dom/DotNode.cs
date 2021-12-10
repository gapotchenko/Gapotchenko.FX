using Gapotchenko.FX.Data.Dot.Serialization;
using System;
using System.Collections.Generic;
using System.IO;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    /// <summary>
    /// Represents a non-terminal node in the syntax tree.
    /// </summary>
    public abstract class DotNode : DotElement, IDotSyntaxSlotProvider
    {
        /// <inheritdoc/>
        public override IList<DotTrivia> LeadingTrivia =>
            (DotDomNavigator.TryGetFirstToken(this) ?? throw new InvalidOperationException("A node contains no tokens."))
            .LeadingTrivia;

        /// <inheritdoc/>
        public override IList<DotTrivia> TrailingTrivia =>
            (DotDomNavigator.TryGetLastToken(this) ?? throw new InvalidOperationException("A node contains no tokens."))
            .TrailingTrivia;

        /// <inheritdoc/>
        public override bool HasLeadingTrivia => DotDomNavigator.TryGetFirstToken(this)?.HasLeadingTrivia == true;

        /// <inheritdoc/>
        public override bool HasTrailingTrivia => DotDomNavigator.TryGetLastToken(this)?.HasTrailingTrivia == true;

        /// <summary>
        /// The list of child nodes and tokens of this node.
        /// </summary>
        public DotChildNodeList ChildNodesAndTokens => new(this);

        internal abstract int SlotCount { get; }
        internal abstract DotSyntaxSlot GetSlot(int i);

        int IDotSyntaxSlotProvider.SlotCount => SlotCount;
        DotSyntaxSlot IDotSyntaxSlotProvider.GetSlot(int i) => GetSlot(i);

        /// <summary>
        /// Accepts <see cref="DotDomVisitor"/> visitor.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public abstract void Accept(DotDomVisitor visitor);

        /// <summary>
        /// Returns the string representation of this node.
        /// </summary>
        public override string ToString()
        {
            using var stringWriter = new StringWriter();
            using var dotWriter = DotWriter.Create(stringWriter);
            using var domWriter = new DotDomWriter(dotWriter);
            Accept(domWriter);
            return stringWriter.ToString();
        }
    }
}
