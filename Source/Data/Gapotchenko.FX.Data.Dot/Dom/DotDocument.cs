using Gapotchenko.FX.Data.Dot.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    public class DotDocument
    {
        public DotDocument(DotGraphSyntax root)
        {
            Root = root;
        }

        public DotGraphSyntax Root { get; set; }

        public static DotDocument Load(DotReader reader)
        {
            var parser = new DotParser(reader);
            parser.Parse();
            var root = parser.Root;
            if (root is null)
            {
                throw new InvalidOperationException("Cannot parse DOT document.");
            }

            return new DotDocument(root);
        }

        public void Save(DotWriter writer)
        {
            Save(writer, Root);
        }

        static void Save(DotWriter writer, DotGraphSyntax node)
        {
            var tokens = EnumerateTokens(node);
            foreach (var token in tokens)
            {
                Save(writer, token);
            }
        }

        static void Save(DotWriter writer, DotSyntaxToken token)
        {
            if (token.HasLeadingTrivia)
            {
                foreach (var trivia in token.LeadingTrivia)
                {
                    Save(writer, trivia);
                }
            }

            if (!string.IsNullOrEmpty(token.Value))
            {
                writer.Write(token.Kind, token.Value);
            }

            if (token.HasTrailingTrivia)
            {
                foreach (var trivia in token.TrailingTrivia)
                {
                    Save(writer, trivia);
                }
            }
        }

        static void Save(DotWriter writer, DotSyntaxTrivia trivia)
        {
            if (!string.IsNullOrEmpty(trivia.Value))
            {
                writer.Write(trivia.Kind, trivia.Value);
            }
        }

        static IEnumerable<DotSyntaxToken> EnumerateTokens(DotSyntaxNode node)
        {
            foreach (var child in node.ChildNodesAndTokens())
            {
                if (child.IsToken)
                {
                    yield return child.AsToken()!;
                }
                else if (child.IsNode)
                {
                    foreach (var token in EnumerateTokens(child.AsNode()!))
                    {
                        yield return token;
                    }
                }
            }
        }
    }
}
