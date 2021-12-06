using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    /// <summary>
    /// Represents a non-terminal node in the syntax tree.
    /// </summary>
    public abstract class DotNode : ISyntaxSlotProvider
    {
        /// <summary>
        /// The list of trivia that appear before this node and are attached to 
        /// a token that is a descendant of this node.
        /// </summary>
        public List<DotTrivia> GetLeadingTrivia() =>
            (SyntaxNavigator.GetFirstToken(this) ?? throw new InvalidOperationException("A node contains no tokens."))
            .LeadingTrivia;

        /// <summary>
        /// The list of trivia that appear after this node and are attached to 
        /// a token that is a descendant of this node.
        /// </summary>
        public List<DotTrivia> GetTrailingTrivia() =>
            (SyntaxNavigator.GetLastToken(this) ?? throw new InvalidOperationException("A node contains no tokens."))
            .TrailingTrivia;

        /// <summary>
        /// Determines whether this node has any leading trivia.
        /// </summary>
        public bool HasLeadingTrivia =>
            SyntaxNavigator.GetFirstToken(this)?.HasLeadingTrivia == true;

        /// <summary>
        /// Determines whether this node has any trailing trivia.
        /// </summary>
        public bool HasTrailingTrivia =>
            SyntaxNavigator.GetLastToken(this)?.HasTrailingTrivia == true;

        /// <summary>
        /// The list of child nodes and tokens of this node.
        /// </summary>
        public DotChildNodeList ChildNodesAndTokens()
            => new(this);

        internal abstract int SlotCount { get; }
        internal abstract SyntaxSlot GetSlot(int i);

        int ISyntaxSlotProvider.SlotCount => SlotCount;
        SyntaxSlot ISyntaxSlotProvider.GetSlot(int i) => GetSlot(i);

        /// <summary>
        /// Accepts <see cref="DotSyntaxVisitor"/> visitor.
        /// </summary>
        /// <param name="visitor"></param>
        public abstract void Accept(DotSyntaxVisitor visitor);
    }
}
