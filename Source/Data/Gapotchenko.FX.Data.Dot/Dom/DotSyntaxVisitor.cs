using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    /// <summary>
    /// Represents a <see cref="DotNode"/> visitor that visits only the single node passed into its Visit method.
    /// </summary>
    public abstract class DotSyntaxVisitor
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

        public virtual void Visit(DotNode? node)
        {
            node?.Accept(this);
        }

        public virtual void DefaultVisit(DotNode node)
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
