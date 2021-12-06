using Gapotchenko.FX.Data.Dot.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    sealed class SyntaxNormalizer : DotSyntaxWalker
    {
        readonly string _indentation;
        readonly string _eol;

        public SyntaxNormalizer(string indentation, string eol)
            : base(SyntaxWalkerDepth.Token)
        {
            _indentation = indentation;
            _eol = eol;
        }

        int _indents = 0;

        public override void VisitToken(DotToken token)
        {
            base.VisitToken(token);

            TrimEnd(token);
            token.TrailingTrivia.Add(new DotTrivia(DotTokenKind.Whitespace, " "));
        }

        int _depth = -1;

        public override void VisitDotStatementListNode(DotStatementListNode node)
        {
            _depth++;

            if (_depth is 0)
            {
                _indents++;
            }

            base.VisitDotStatementListNode(node);

            if (_depth is 0)
            {
                _indents--;

                PlaceEndOfLine(node.OpenBraceToken);
                PlaceEndOfLine(node.CloseBraceToken);
            }

            _depth--;
        }

        bool _nestedStatement = false;

        public override void DefaultVisit(DotNode node)
        {
            var shouldIndent = !_nestedStatement && node is DotStatementNode && _indents != 0;

            if (shouldIndent)
            {
                _nestedStatement = true;
            }

            base.DefaultVisit(node);

            if (shouldIndent)
            {
                _nestedStatement = false;
                node.GetLeadingTrivia().InsertRange(0, CreateIndentation());
                PlaceEndOfLine(node);
            }
        }

        IEnumerable<DotTrivia> CreateIndentation()
        {
            for (int i = 0; i < _indents; i++)
            {
                yield return new DotTrivia(DotTokenKind.Whitespace, _indentation);
            }
        }

        void PlaceEndOfLine(DotNode? node)
        {
            if (node is not null)
            {
                var trailingTrivia = node.GetTrailingTrivia();
                trailingTrivia.RemoveAll(t => t.Kind is DotTokenKind.Whitespace);
                trailingTrivia.Add(new DotTrivia(DotTokenKind.Whitespace, _eol));
            }
        }

        void PlaceEndOfLine(DotToken? token)
        {
            if (token is not null)
            {
                TrimEnd(token);
                token.TrailingTrivia.Add(new DotTrivia(DotTokenKind.Whitespace, _eol));
            }
        }

        static void TrimStart(DotToken token)
        {
            if (token.HasLeadingTrivia)
            {
                token.LeadingTrivia.RemoveAll(t => t.Kind is DotTokenKind.Whitespace);
            }
        }

        static void TrimEnd(DotToken token)
        {
            if (token.HasTrailingTrivia)
            {
                token.TrailingTrivia.RemoveAll(t => t.Kind is DotTokenKind.Whitespace);
            }
        }

        public override void VisitDotAttributeListNode(DotAttributeListNode node)
        {
            base.VisitDotAttributeListNode(node);

            if (node.OpenBraceToken?.HasTrailingTrivia == true)
            {
                TrimEnd(node.OpenBraceToken);
            }

            var lastAttribute = node.Attributes?.LastOrDefault();
            if (lastAttribute is not null)
            {
                var lastToken = SyntaxNavigator.GetLastToken(lastAttribute);
                if (lastToken is not null)
                {
                    TrimEnd(lastToken);
                }
            }
        }
    }
}
