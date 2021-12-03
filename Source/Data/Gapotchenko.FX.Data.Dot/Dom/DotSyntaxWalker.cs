using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    /// <summary>
    /// Represents a <see cref="DotSyntaxVisitor"/> that descends an entire <see cref="DotSyntaxNode"/> graph
    /// visiting each node and its child nodes and tokens in depth-first order.
    /// </summary>
    public abstract class DotSyntaxWalker : DotSyntaxVisitor
    {
        protected SyntaxWalkerDepth Depth { get; }

        protected DotSyntaxWalker(SyntaxWalkerDepth depth = SyntaxWalkerDepth.Node)
        {
            Depth = depth;
        }

        public override void Visit(DotSyntaxNode? node)
        {
            node?.Accept(this);
        }

        public override void DefaultVisit(DotSyntaxNode node)
        {
            var childCnt = node.ChildNodesAndTokens().Count;
            int i = 0;

            do
            {
                var child = DotChildSyntaxList.ItemInternal(node, i);
                i++;

                var asNode = child.AsNode();
                if (asNode is not null)
                {
                    if (Depth >= SyntaxWalkerDepth.Node)
                    {
                        Visit(asNode);
                    }
                }
                else
                {
                    if (Depth >= SyntaxWalkerDepth.Token)
                    {
                        var token = child.AsToken();
                        if (token is not null)
                        {
                            VisitToken(token);
                        }
                    }
                }
            } while (i < childCnt);
        }

        public virtual void VisitToken(DotSyntaxToken token)
        {
            if (Depth >= SyntaxWalkerDepth.Trivia)
            {
                VisitLeadingTrivia(token);
                VisitTrailingTrivia(token);
            }
        }

        public virtual void VisitLeadingTrivia(DotSyntaxToken token)
        {
            if (token.HasLeadingTrivia)
            {
                foreach (var tr in token.LeadingTrivia)
                {
                    VisitTrivia(tr);
                }
            }
        }

        public virtual void VisitTrailingTrivia(DotSyntaxToken token)
        {
            if (token.HasTrailingTrivia)
            {
                foreach (var tr in token.TrailingTrivia)
                {
                    VisitTrivia(tr);
                }
            }
        }

        public virtual void VisitTrivia(DotSyntaxTrivia trivia)
        {
        }
    }
}
