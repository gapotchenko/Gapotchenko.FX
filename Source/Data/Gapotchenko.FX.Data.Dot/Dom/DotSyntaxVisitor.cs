using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    /// <summary>
    /// Represents a <see cref="DotSyntaxNode"/> visitor that visits only the single node passed into its Visit method.
    /// </summary>
    public abstract class DotSyntaxVisitor
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

        public virtual void Visit(DotSyntaxNode? node)
        {
            node?.Accept(this);
        }

        public virtual void DefaultVisit(DotSyntaxNode node)
        {
        }

        public virtual void VisitAttachedDotAttributesSyntax(AttachedDotAttributesSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitDotAliasSyntax(DotAliasSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitDotStatementListSyntax(DotStatementListSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitDotVertexSyntax(DotVertexSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitDotAttributeListSyntax(DotAttributeListSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitDotVertexIdentifierSyntax(DotVertexIdentifierSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitDotGraphSyntax(DotGraphSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitDotEdgeSyntax(DotEdgeSyntax node)
        {
            DefaultVisit(node);
        }

        public virtual void VisitDotAttributeSyntax(DotAttributeSyntax node)
        {
            DefaultVisit(node);
        }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
