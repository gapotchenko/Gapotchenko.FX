using System;
using System.Collections.Generic;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    /// <summary>
    /// Represents a non-terminal node in the syntax tree.
    /// </summary>
    public abstract class DotNode : DotElement, ISyntaxSlotProvider
    {
        /// <inheritdoc/>
        public override IList<DotTrivia> LeadingTrivia =>
            (SyntaxNavigator.GetFirstToken(this) ?? throw new InvalidOperationException("A node contains no tokens."))
            .LeadingTrivia;

        /// <inheritdoc/>
        public override IList<DotTrivia> TrailingTrivia =>
            (SyntaxNavigator.GetLastToken(this) ?? throw new InvalidOperationException("A node contains no tokens."))
            .TrailingTrivia;

        /// <inheritdoc/>
        public override bool HasLeadingTrivia => SyntaxNavigator.GetFirstToken(this)?.HasLeadingTrivia == true;

        /// <inheritdoc/>
        public override bool HasTrailingTrivia => SyntaxNavigator.GetLastToken(this)?.HasTrailingTrivia == true;

        /// <summary>
        /// The list of child nodes and tokens of this node.
        /// </summary>
        public DotChildNodeList ChildNodesAndTokens => new(this);

        internal abstract int SlotCount { get; }
        internal abstract SyntaxSlot GetSlot(int i);

        int ISyntaxSlotProvider.SlotCount => SlotCount;
        SyntaxSlot ISyntaxSlotProvider.GetSlot(int i) => GetSlot(i);

        /// <summary>
        /// Accepts <see cref="DotDomVisitor"/> visitor.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public abstract void Accept(DotDomVisitor visitor);
    }
}
