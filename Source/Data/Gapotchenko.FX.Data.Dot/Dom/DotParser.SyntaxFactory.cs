using Gapotchenko.FX.Data.Dot.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    partial class DotParser
    {
        static DotSyntaxTrivia CreateTrivia(
            DotToken tokenType,
            string value)
        {
            return new DotSyntaxTrivia(tokenType, value);
        }

        static DotSyntaxTrivia CreateWhitespaceTrivia(
            string value)
        {
            return new DotSyntaxTrivia(DotToken.Whitespace, value);
        }

        static DotSyntaxTrivia CreateTrivia(
            DotValueType value)
        {
            var token = value.token;
            return new DotSyntaxTrivia(token.Kind, token.Value);
        }

        static DotSyntaxTrivia CreateTrivia(
            DotSyntaxToken token)
        {
            return new DotSyntaxTrivia(token.Kind, token.Value);
        }

        static DotVertexIdentifierSyntax CreateVertexIdentifierSyntax(
            DotSyntaxToken identifier,
            DotSyntaxToken? colon1,
            DotSyntaxToken? portIdentifier,
            DotSyntaxToken? colon2,
            DotSyntaxToken? compassPoint)
        {
            return new DotVertexIdentifierSyntax
            {
                Identifier = identifier,
                PortColonToken = colon1,
                PortIdentifier = portIdentifier,
                CompassPointColonToken = colon2,
                CompassPointToken = compassPoint
            };
        }

        static DotAttributeSyntax CreateAttributeSyntax(
            DotSyntaxToken lhs,
            DotSyntaxToken? equal,
            DotSyntaxToken? rhs,
            DotSyntaxToken? terminator)
        {
            return new DotAttributeSyntax
            {
                LHS = lhs,
                EqualToken = equal,
                RHS = rhs,
                SemicolonOrCommaToken = terminator
            };
        }

        static DotSyntaxList<DotAttributeListSyntax> CreateAttributeListSyntaxList(
            DotSyntaxToken openBraceToken,
            DotSyntaxList<DotAttributeSyntax>? attributes,
            DotSyntaxToken closeBraceToken)
        {
            var list = new DotSyntaxList<DotAttributeListSyntax>();
            var syntax = CreateAttributeListSyntax(openBraceToken, attributes, closeBraceToken);
            list.Append(syntax);
            return list;
        }

        static DotAttributeListSyntax CreateAttributeListSyntax(
            DotSyntaxToken openBraceToken,
            DotSyntaxList<DotAttributeSyntax> attributes,
            DotSyntaxToken closeBraceToken)
        {
            return new DotAttributeListSyntax()
            {
                OpenBraceToken = openBraceToken,
                Attributes = attributes,
                CloseBraceToken = closeBraceToken,
            };
        }

        static AttachedDotAttributesSyntax CreateAttachedAttributesSyntax(
            DotSyntaxToken targetKeyword,
            DotSyntaxList<DotAttributeListSyntax> attributes)
        {
            return new AttachedDotAttributesSyntax
            {
                TargetKeyword = targetKeyword,
                Attributes = attributes
            };
        }

        static DotEdgeSyntax CreateEdgeSyntax(
            SeparatedDotSyntaxList<DotSyntaxNode> elements,
            DotSyntaxList<DotAttributeListSyntax> attributes)
        {
            return new DotEdgeSyntax
            {
                Elements = elements,
                Attributes = attributes,
            };
        }

        static DotVertexSyntax CreateVertexSyntax(
            DotVertexIdentifierSyntax identifier,
            DotSyntaxList<DotAttributeListSyntax> attributes)
        {
            return new DotVertexSyntax
            {
                Identifier = identifier,
                Attributes = attributes
            };
        }

        static DotAliasSyntax CreateAliasSyntax(
            DotSyntaxToken lhs,
            DotSyntaxToken equalToken,
            DotSyntaxToken rhs)
        {
            return new DotAliasSyntax
            {
                LHS = lhs,
                EqualToken = equalToken,
                RHS = rhs
            };
        }

        static DotSyntaxToken CreateToken(
            DotToken tokenType,
            string value)
        {
            return new DotSyntaxToken(tokenType, value);
        }

        static DotSyntaxToken CreateToken(
            DotValueType value)
        {
            return value.token;
        }

        static DotGraphSyntax CreateGraphSyntax(
            DotSyntaxToken? strictKeyword,
            DotSyntaxToken? graphKind,
            DotSyntaxToken? identifier,
            DotStatementListSyntax? statements)
        {
            return new DotGraphSyntax
            {
                StrictKeyword = strictKeyword,
                GraphKindKeyword = graphKind,
                Identifier = identifier,
                Statements = statements
            };
        }

        static DotStatementListSyntax CreateStatementListSyntax(
            DotSyntaxToken openBrace,
            DotSyntaxList<DotStatementSyntax> statements,
            DotSyntaxToken closeBrace)
        {
            return new DotStatementListSyntax
            {
                OpenBraceToken = openBrace,
                Statements = statements,
                CloseBraceToken = closeBrace
            };
        }

        static void Prepend<T>(
            DotSyntaxList<T> syntaxList,
            T node)
        {
            syntaxList.Prepend(node);
        }

        static void Prepend<TNode>(
            SeparatedDotSyntaxList<TNode> syntaxList,
            TNode node)
            where TNode : DotSyntaxNode
        {
            syntaxList.Prepend(node);
        }

        IEnumerable<DotSyntaxTrivia> TokenToTrivia(DotSyntaxToken token)
        {
            if (token.HasLeadingTrivia)
            {
                foreach (var trivia in token.LeadingTrivia)
                {
                    yield return trivia;
                }
            }

            yield return CreateTrivia(token);

            if (token.HasTrailingTrivia)
            {
                foreach (var trivia in token.TrailingTrivia)
                {
                    yield return trivia;
                }
            }
        }

        void AppendLeadingTrivia(DotSyntaxToken destination, DotValueType source)
        {
            destination.LeadingTrivia.AddRange(TokenToTrivia(source.token));
        }

        void PrependLeadingTrivia(DotSyntaxToken destination, DotValueType source)
        {
            destination.LeadingTrivia.InsertRange(0, TokenToTrivia(source.token));
        }

        void AppendTrailingTrivia(DotSyntaxToken destination, DotValueType source)
        {
            destination.TrailingTrivia.AddRange(TokenToTrivia(source.token));
        }

        void PrependTrailingTrivia(DotSyntaxToken destination, DotValueType source)
        {
            destination.TrailingTrivia.InsertRange(0, TokenToTrivia(source.token));
        }
    }
}
