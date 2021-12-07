using Gapotchenko.FX.Collections.Generic;
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
        static DotTrivia CreateTrivia(
            DotTokenKind tokenType,
            string value)
        {
            return new DotTrivia(tokenType, value);
        }

        static DotTrivia CreateWhitespaceTrivia(
            string value)
        {
            return new DotTrivia(DotTokenKind.Whitespace, value);
        }

        static DotTrivia CreateTrivia(
            DotValueType value)
        {
            var token = value.token;
            return new DotTrivia(token.Kind, token.Value);
        }

        static DotTrivia CreateTrivia(
            DotToken token)
        {
            return new DotTrivia(token.Kind, token.Value);
        }

        static DotVertexIdentifierNode CreateVertexIdentifierSyntax(
            DotToken identifier,
            DotToken? colon1,
            DotToken? portIdentifier,
            DotToken? colon2,
            DotToken? compassPoint)
        {
            return new DotVertexIdentifierNode
            {
                Identifier = identifier,
                PortColonToken = colon1,
                PortIdentifier = portIdentifier,
                CompassPointColonToken = colon2,
                CompassPointToken = compassPoint
            };
        }

        static DotAttributeNode CreateAttributeSyntax(
            DotToken lhs,
            DotToken? equal,
            DotToken? rhs,
            DotToken? terminator)
        {
            return new DotAttributeNode
            {
                LHS = lhs,
                EqualToken = equal,
                RHS = rhs,
                SemicolonOrCommaToken = terminator
            };
        }

        static DotNodeList<DotAttributeListNode> CreateAttributeListSyntaxList(
            DotToken openBraceToken,
            DotNodeList<DotAttributeNode>? attributes,
            DotToken closeBraceToken)
        {
            var list = new DotNodeList<DotAttributeListNode>();
            var syntax = CreateAttributeListSyntax(openBraceToken, attributes, closeBraceToken);
            list.Add(syntax);
            return list;
        }

        static DotAttributeListNode CreateAttributeListSyntax(
            DotToken openBraceToken,
            DotNodeList<DotAttributeNode>? attributes,
            DotToken closeBraceToken)
        {
            return new DotAttributeListNode()
            {
                OpenBraceToken = openBraceToken,
                Attributes = attributes,
                CloseBraceToken = closeBraceToken,
            };
        }

        static AttachedDotAttributesNode CreateAttachedAttributesSyntax(
            DotToken targetKeyword,
            DotNodeList<DotAttributeListNode> attributes)
        {
            return new AttachedDotAttributesNode
            {
                TargetKeyword = targetKeyword,
                Attributes = attributes
            };
        }

        static DotEdgeNode CreateEdgeSyntax(
            SeparatedDotNodeList<DotNode> elements,
            DotNodeList<DotAttributeListNode> attributes)
        {
            return new DotEdgeNode
            {
                Elements = elements,
                Attributes = attributes,
            };
        }

        static DotVertexNode CreateVertexSyntax(
            DotVertexIdentifierNode identifier,
            DotNodeList<DotAttributeListNode> attributes)
        {
            return new DotVertexNode
            {
                Identifier = identifier,
                Attributes = attributes
            };
        }

        static DotAliasNode CreateAliasSyntax(
            DotToken lhs,
            DotToken equalToken,
            DotToken rhs)
        {
            return new DotAliasNode
            {
                LHS = lhs,
                EqualToken = equalToken,
                RHS = rhs
            };
        }

        static DotToken CreateToken(
            DotTokenKind tokenType,
            string value)
        {
            return new DotToken(tokenType, value);
        }

        static DotToken CreateToken(
            DotValueType value)
        {
            return value.token;
        }

        static DotGraphNode CreateGraphSyntax(
            DotToken? strictKeyword,
            DotToken? graphKind,
            DotToken? identifier,
            DotStatementListNode? statements)
        {
            return new DotGraphNode
            {
                StrictKeyword = strictKeyword,
                GraphKindKeyword = graphKind,
                Identifier = identifier,
                Statements = statements
            };
        }

        static DotStatementListNode CreateStatementListSyntax(
            DotToken openBrace,
            DotNodeList<DotStatementNode> statements,
            DotToken closeBrace)
        {
            return new DotStatementListNode
            {
                OpenBraceToken = openBrace,
                Statements = statements,
                CloseBraceToken = closeBrace
            };
        }

        static void Prepend<TNode>(
            DotNodeList<TNode> syntaxList,
            TNode node)
            where TNode : DotNode
        {
            syntaxList.AddFirst(node);
        }

        static void Prepend<TNode>(
            SeparatedDotNodeList<TNode> syntaxList,
            TNode node)
            where TNode : DotNode
        {
            syntaxList.AddFirst(node);
        }

        static IEnumerable<DotTrivia> TokenToTrivia(DotToken token)
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

        static void AppendLeadingTrivia(DotToken destination, DotValueType source)
        {
            destination.LeadingTrivia.AddRange(TokenToTrivia(source.token));
        }

        static void PrependLeadingTrivia(DotToken destination, DotValueType source)
        {
            destination.LeadingTrivia.InsertRange(0, TokenToTrivia(source.token));
        }

        static void AppendTrailingTrivia(DotToken destination, DotValueType source)
        {
            destination.TrailingTrivia.AddRange(TokenToTrivia(source.token));
        }

        static void PrependTrailingTrivia(DotToken destination, DotValueType source)
        {
            destination.TrailingTrivia.InsertRange(0, TokenToTrivia(source.token));
        }
    }
}
