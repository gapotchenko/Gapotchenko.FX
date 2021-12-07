namespace Gapotchenko.FX.Data.Dot.Dom
{
    /// <summary>
    /// Represents a <see cref="DotNode"/> visitor that visits only the single node passed into its Visit method.
    /// </summary>
    public abstract class DotDomVisitor
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

        protected virtual void DefaultVisit(DotNode node)
        {
        }

        public virtual void VisitAttachedDotAttributesNode(AttachedDotAttributesNode node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitDotAliasNode(DotAliasNode node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitDotStatementListNode(DotStatementListNode node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitDotVertexNode(DotVertexNode node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitDotAttributeListNode(DotAttributeListNode node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitDotVertexIdentifierNode(DotVertexIdentifierNode node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitDotGraphNode(DotGraphNode node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitDotEdgeNode(DotEdgeNode node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitDotAttributeNode(DotAttributeNode node)
        {
            DefaultVisit(node);
        }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
