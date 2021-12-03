using Gapotchenko.FX.Data.Dot.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    public static class DotSyntaxFactory
    {
        public static DotSyntaxToken Token(
            DotToken kind,
            string? text = default)
        {
            if (text is null)
            {
                text = kind switch
                {
                    DotToken.Digraph => "digraph",
                    DotToken.Graph => "graph",
                    DotToken.Arrow => "->",
                    _ => throw new ArgumentException("Text expected.", nameof(text))
                };
            }

            return new DotSyntaxToken(kind, text);
        }

        public static DotSyntaxTrivia Whitespace(string whitespace)
        {
            return new DotSyntaxTrivia(
                DotToken.Whitespace,
                whitespace);
        }

        public static DotGraphSyntax Graph(
            DotSyntaxToken? strict,
            DotSyntaxToken? kind,
            DotSyntaxToken? identifier,
            DotStatementListSyntax? statements)
        {
            return new DotGraphSyntax
            {
                StrictKeyword = strict,
                GraphKindKeyword = kind,
                Identifier = identifier,
                Statements = statements,
            };
        }

        public static DotStatementListSyntax StatementList(
            DotSyntaxList<DotStatementSyntax> statements)
        {
            return new DotStatementListSyntax
            {
                OpenBraceToken = Separator('{'),
                Statements = statements,
                CloseBraceToken = Separator('}'),
            };
        }

        public static DotVertexSyntax VertexStatement(
            DotVertexIdentifierSyntax identifier,
            DotSyntaxList<DotAttributeListSyntax>? attributes)
        {
            return new DotVertexSyntax
            {
                Identifier = identifier,
                Attributes = attributes,
            };
        }

        public static DotVertexIdentifierSyntax Identifier(
            string identifier,
            string? port = null,
            string? compassPoint = null)
        {
            return new DotVertexIdentifierSyntax
            {
                Identifier = Id(identifier),
                PortColonToken = port is not null ? Separator(':') : default,
                PortIdentifier = port is not null ? Id(port) : default,
                CompassPointColonToken = compassPoint is not null ? Separator(':') : default,
                CompassPointToken = compassPoint is not null ? Id(compassPoint) : default,
            };
        }

        public static DotAttributeSyntax Attribute(
            string name,
            string value)
        {
            return new DotAttributeSyntax
            {
                LHS = Id(name),
                EqualToken = Separator('='),
                RHS = Id(value),
            };
        }

        public static DotAttributeListSyntax AttributeList(
            DotSyntaxList<DotAttributeSyntax> attributes)
        {
            return new DotAttributeListSyntax
            {
                OpenBraceToken = Separator('['),
                Attributes = attributes,
                CloseBraceToken = Separator(']'),
            };
        }

        public static DotSyntaxList<TNode> List<TNode>(
            IEnumerable<TNode> items)
        {
            var list = new DotSyntaxList<TNode>();

            foreach (var item in items)
            {
                list.Append(item);
            }

            return list;
        }

        public static SeparatedDotSyntaxList<TNode> SeparatedList<TNode>(
            IEnumerable<TNode> items,
            DotSyntaxToken separator)
            where TNode : DotSyntaxNode
        {
            var list = new SeparatedDotSyntaxList<TNode>();

            bool isFirst = true;
            foreach (var item in items)
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    list.Append(separator);
                }

                list.Append(item);
            }

            return list;
        }

        public static DotEdgeSyntax Edge(
            SeparatedDotSyntaxList<DotSyntaxNode> elements,
            DotSyntaxList<DotAttributeListSyntax>? attributes)
        {
            return new DotEdgeSyntax
            {
                Elements = elements,
                Attributes = attributes,
            };
        }

        static DotSyntaxToken Separator(char separator)
        {
            return new DotSyntaxToken(
                (DotToken)separator,
                separator.ToString());
        }

        static DotSyntaxTrivia Trivia(char trivia)
        {
            return new DotSyntaxTrivia(
                (DotToken)trivia,
                trivia.ToString());
        }

        static readonly Regex ValidIdentifierPattern = new(
            @"^(([a-zA-Z\200-\377_][0-9a-zA-Z\200-\377_]*)|(-?(\.[0-9]+|[0-9]+(\.[0-9]*)?)))$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant);

        static DotSyntaxToken Id(string? identifier)
        {
            identifier ??= string.Empty;
            identifier = EscapeIdentifier(identifier);

            var token = new DotSyntaxToken(DotToken.Id, identifier);

            if (string.IsNullOrEmpty(identifier) ||
                !ValidIdentifierPattern.IsMatch(identifier))
            {
                token.LeadingTrivia.Add(Trivia('"'));
                token.TrailingTrivia.Add(Trivia('"'));
            }

            return token;
        }

        static string EscapeIdentifier(string identifier)
        {
            return identifier
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\r\n", "\\r\\n")
                .Replace("\n", "\\n");
        }
    }
}
