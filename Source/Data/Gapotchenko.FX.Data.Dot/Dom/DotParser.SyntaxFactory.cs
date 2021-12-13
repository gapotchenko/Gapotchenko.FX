using Gapotchenko.FX.Data.Dot.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    partial class DotParser
    {
        static DotStringLiteral? CreateStringLiteral(DotToken? token)
        {
            if (token?.IsDefault == false)
                return CreateStringLiteral(token.Value);
            return default;
        }

        static DotStringLiteral CreateStringLiteral(DotToken token)
        {
            var result = DotStringLiteral.Parse(token.Text);
            ApplyTrivia(result, token);
            return result;
        }

        static DotPunctuationToken? CreatePunctuationToken(DotToken? token)
        {
            if (token?.IsDefault == false)
                return CreatePunctuationToken(token.Value);
            return default;
        }

        static DotPunctuationToken CreatePunctuationToken(DotToken token)
        {
            var result = new DotPunctuationToken(token.Kind.ToDotPunctuationTokenKind(), token.Text);
            ApplyTrivia(result, token);
            return result;
        }

        static DotKeywordToken? CreateKeywordToken(DotToken? token)
        {
            if (token?.IsDefault == false)
                return CreateKeywordToken(token.Value);
            return default;
        }

        static DotKeywordToken CreateKeywordToken(DotToken token)
        {
            var result = new DotKeywordToken(token.Kind.ToDotKeywordTokenKind(), token.Text);
            ApplyTrivia(result, token);
            return result;
        }

        static DotArrowToken CreateArrowToken(DotToken token)
        {
            Debug.Assert(token.Kind is DotTokenKind.Arrow);
            var result = new DotArrowToken(token.Text switch
            {
                "->" => DotArrowTokenKind.LeftToRight,
                "--" => DotArrowTokenKind.Bidirectional,
                _ => throw new ArgumentOutOfRangeException($"Unknown arrow kind: {token.Text}.")
            }, token.Text);
            ApplyTrivia(result, token);
            return result;
        }

        static void ApplyTrivia(DotSignificantToken destination, DotToken source)
        {
            foreach (var trivia in source.LeadingTrivia)
            {
                destination.LeadingTrivia.Add(trivia);
            }

            foreach (var trivia in source.TrailingTrivia)
            {
                destination.TrailingTrivia.Add(trivia);
            }
        }

        static DotInsignificantToken CreateTrivia(
            DotTokenKind tokenType,
            string value)
        {
            return new DotInsignificantToken(tokenType.ToDotTriviaKind(), value);
        }

        static DotInsignificantToken CreateTrivia(
            DotToken token)
        {
            return new DotInsignificantToken(token.Kind.ToDotTriviaKind(), token.Text);
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
                Identifier = CreateStringLiteral(identifier),
                PortColonToken = CreatePunctuationToken(colon1),
                PortIdentifier = CreateStringLiteral(portIdentifier),
                CompassPointColonToken = CreatePunctuationToken(colon2),
                CompassPointToken = CreateStringLiteral(compassPoint)
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
                LHS = CreateStringLiteral(lhs),
                EqualToken = CreatePunctuationToken(equal),
                RHS = CreateStringLiteral(rhs),
                SemicolonOrCommaToken = CreatePunctuationToken(terminator)
            };
        }

        static IList<DotAttributeListNode> CreateAttributeListSyntaxList(
            DotToken openBraceToken,
            IList<DotAttributeNode>? attributes,
            DotToken closeBraceToken)
        {
            var list = new List<DotAttributeListNode>();
            var syntax = CreateAttributeListSyntax(openBraceToken, attributes, closeBraceToken);
            list.Add(syntax);
            return list;
        }

        static DotAttributeListNode CreateAttributeListSyntax(
            DotToken openBraceToken,
            IList<DotAttributeNode>? attributes,
            DotToken closeBraceToken)
        {
            return new DotAttributeListNode()
            {
                OpenBraceToken = CreatePunctuationToken(openBraceToken),
                Attributes = attributes,
                CloseBraceToken = CreatePunctuationToken(closeBraceToken),
            };
        }

        static AttachedDotAttributesNode CreateAttachedAttributesSyntax(
            DotToken targetKeyword,
            IList<DotAttributeListNode> attributes)
        {
            return new AttachedDotAttributesNode
            {
                TargetKeyword = CreateKeywordToken(targetKeyword),
                Attributes = attributes
            };
        }

        static DotEdgeNode CreateEdgeSyntax(
            SeparatedDotNodeList<DotNode> elements,
            IList<DotAttributeListNode> attributes)
        {
            return new DotEdgeNode
            {
                Elements = elements,
                Attributes = attributes,
            };
        }

        static DotVertexNode CreateVertexSyntax(
            DotVertexIdentifierNode identifier,
            IList<DotAttributeListNode> attributes)
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
                LHS = CreateStringLiteral(lhs),
                EqualToken = CreatePunctuationToken(equalToken),
                RHS = CreateStringLiteral(rhs)
            };
        }

        static DotGraphNode CreateGraphSyntax(
            DotToken? strictKeyword,
            DotToken? graphKind,
            DotToken? identifier,
            DotStatementListNode? statements)
        {
            return new DotGraphNode
            {
                StrictKeyword = CreateKeywordToken(strictKeyword),
                GraphKindKeyword = CreateKeywordToken(graphKind),
                Identifier = CreateStringLiteral(identifier),
                Statements = statements
            };
        }

        static DotStatementListNode CreateStatementListSyntax(
            DotToken openBrace,
            IList<DotStatementNode> statements,
            DotToken closeBrace)
        {
            return new DotStatementListNode
            {
                OpenBraceToken = CreatePunctuationToken(openBrace),
                Statements = statements,
                CloseBraceToken = CreatePunctuationToken(closeBrace)
            };
        }
    }
}
