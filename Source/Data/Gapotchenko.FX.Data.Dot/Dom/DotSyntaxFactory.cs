using Gapotchenko.FX.Data.Dot.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    /// <summary>
    /// DOT syntax factory.
    /// </summary>
    public static class DotSyntaxFactory
    {
        /// <summary>
        /// Creates a token.
        /// </summary>
        public static DotToken Token(
            DotTokenKind kind,
            string? text = default)
        {
            if (text is null)
            {
                text = kind switch
                {
                    DotTokenKind.Digraph => "digraph",
                    DotTokenKind.Graph => "graph",
                    DotTokenKind.Arrow => "->",
                    _ => throw new ArgumentException("Value cannot deducted from the kind.", nameof(text))
                };
            }

            return new DotToken(kind, text);
        }

        /// <summary>
        /// Creates a whitespace trivia.
        /// </summary>
        public static DotTrivia Whitespace(string whitespace)
        {
            return new DotTrivia(
                DotTokenKind.Whitespace,
                whitespace);
        }

        /// <summary>
        /// Creates a graph syntax node.
        /// </summary>
        public static DotGraphNode Graph(
            DotToken? strict,
            DotToken? kind,
            DotToken? identifier,
            DotStatementListNode? statements)
        {
            return new DotGraphNode
            {
                StrictKeyword = strict,
                GraphKindKeyword = kind,
                Identifier = identifier,
                Statements = statements,
            };
        }

        /// <summary>
        /// Creates a statement list syntax node.
        /// </summary>
        public static DotStatementListNode StatementList(
            DotNodeList<DotStatementNode> statements)
        {
            return new DotStatementListNode
            {
                OpenBraceToken = Separator('{'),
                Statements = statements,
                CloseBraceToken = Separator('}'),
            };
        }

        /// <summary>
        /// Creates a vertes statement syntax node.
        /// </summary>
        public static DotVertexNode VertexStatement(
            DotVertexIdentifierNode identifier,
            DotNodeList<DotAttributeListNode>? attributes)
        {
            return new DotVertexNode
            {
                Identifier = identifier,
                Attributes = attributes,
            };
        }

        /// <summary>
        /// Creates a vertex identifier syntax node.
        /// </summary>
        public static DotVertexIdentifierNode Identifier(
            string identifier,
            string? port = null,
            string? compassPoint = null)
        {
            return new DotVertexIdentifierNode
            {
                Identifier = Id(identifier),
                PortColonToken = port is not null ? Separator(':') : default,
                PortIdentifier = port is not null ? Id(port) : default,
                CompassPointColonToken = compassPoint is not null ? Separator(':') : default,
                CompassPointToken = compassPoint is not null ? Id(compassPoint) : default,
            };
        }

        /// <summary>
        /// Creates an attribute syntax node.
        /// </summary>
        public static DotAttributeNode Attribute(
            string name,
            string value)
        {
            return new DotAttributeNode
            {
                LHS = Id(name),
                EqualToken = Separator('='),
                RHS = Id(value),
            };
        }

        /// <summary>
        /// Creates an attribute list syntax node.
        /// </summary>
        public static DotAttributeListNode AttributeList(
            DotNodeList<DotAttributeNode> attributes)
        {
            return new DotAttributeListNode
            {
                OpenBraceToken = Separator('['),
                Attributes = attributes,
                CloseBraceToken = Separator(']'),
            };
        }

        /// <summary>
        /// Creates a list of syntax nodes.
        /// </summary>
        public static DotNodeList<TNode> List<TNode>(
            IEnumerable<TNode> items)
            where TNode : DotNode
        {
            var list = new DotNodeList<TNode>();

            foreach (var item in items)
            {
                list.Append(item);
            }

            return list;
        }

        /// <summary>
        /// Creates a separated list of syntax nodes.
        /// </summary>
        public static SeparatedDotNodeList<TNode> SeparatedList<TNode>(
            IEnumerable<TNode> items,
            DotToken separator)
            where TNode : DotNode
        {
            var list = new SeparatedDotNodeList<TNode>();

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

        /// <summary>
        /// Creates an edge syntax node.
        /// </summary>
        public static DotEdgeNode Edge(
            SeparatedDotNodeList<DotNode> elements,
            DotNodeList<DotAttributeListNode>? attributes)
        {
            return new DotEdgeNode
            {
                Elements = elements,
                Attributes = attributes,
            };
        }

        static DotToken Separator(char separator)
        {
            return new DotToken(
                (DotTokenKind)separator,
                separator.ToString());
        }

        static DotTrivia Trivia(char trivia)
        {
            return new DotTrivia(
                (DotTokenKind)trivia,
                trivia.ToString());
        }

        static readonly Regex ValidIdentifierPattern = new(
            @"^(([a-zA-Z\200-\377_][0-9a-zA-Z\200-\377_]*)|(-?(\.[0-9]+|[0-9]+(\.[0-9]*)?)))$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant);

        static DotToken Id(string? identifier)
        {
            identifier ??= string.Empty;
            identifier = EscapeIdentifier(identifier);

            var token = new DotToken(DotTokenKind.Id, identifier);

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
