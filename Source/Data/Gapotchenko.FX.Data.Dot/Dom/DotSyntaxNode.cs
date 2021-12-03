using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    public abstract class DotSyntaxNode : ISyntaxSlotProvider
    {
        public List<DotSyntaxTrivia> LeadingTrivia =>
            (SyntaxNavigator.GetFirstToken(this) ?? throw new InvalidOperationException("A node contains no tokens."))
            .LeadingTrivia;

        public List<DotSyntaxTrivia> TrailingTrivia =>
            (SyntaxNavigator.GetLastToken(this) ?? throw new InvalidOperationException("A node contains no tokens."))
            .TrailingTrivia;

        public bool HasLeadingTrivia =>
            SyntaxNavigator.GetFirstToken(this)?.HasLeadingTrivia == true;

        public bool HasTrailingTrivia =>
            SyntaxNavigator.GetLastToken(this)?.HasTrailingTrivia == true;

        public DotChildSyntaxList ChildNodesAndTokens()
            => new(this);

        internal abstract int SlotCount { get; }
        internal abstract SyntaxSlot GetSlot(int i);

        int ISyntaxSlotProvider.SlotCount => SlotCount;
        SyntaxSlot ISyntaxSlotProvider.GetSlot(int i) => GetSlot(i);

        public abstract void Accept(DotSyntaxVisitor visitor);
    }
}
