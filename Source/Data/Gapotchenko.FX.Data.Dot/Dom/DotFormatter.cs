using Gapotchenko.FX.Data.Dot.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    /// <summary>
    /// Formats whitespace in syntax trees.
    /// </summary>
    public static class DotFormatter
    {
        const string DefaultIndentation = "    ";
        const string DefaultEOL = "\r\n";

        /// <summary>
        /// Replaces whitespaces and end of line trivia with regularly formatted trivia.
        /// </summary>
        public static void NormalizeWhitespace<TNode>(
            this TNode node,
            string indentation = DefaultIndentation,
            string eol = DefaultEOL)
            where TNode : DotSyntaxNode
        {
            new WhitespaceEraser().Visit(node);
            new SyntaxNormalizer(indentation, eol).Visit(node);
        }

        sealed class WhitespaceEraser : DotSyntaxWalker
        {
            public WhitespaceEraser() : base(SyntaxWalkerDepth.Token)
            { }

            public override void VisitToken(DotSyntaxToken token)
            {
                if (token.HasLeadingTrivia)
                {
                    token.LeadingTrivia.RemoveAll(t => t.Kind is DotToken.Whitespace);
                }

                if (token.HasTrailingTrivia)
                {
                    token.TrailingTrivia.RemoveAll(t => t.Kind is DotToken.Whitespace);
                }
            }
        }
    }
}
