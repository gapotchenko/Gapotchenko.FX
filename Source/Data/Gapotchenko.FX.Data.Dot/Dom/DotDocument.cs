using Gapotchenko.FX.Data.Dot.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    /// <summary>
    /// Represents a DOT document.
    /// </summary>
    public class DotDocument
    {
        /// <summary>
        /// Gets or sets a document root.
        /// </summary>
        public DotGraphNode? Root { get; set; }

        /// <summary>
        /// Loads a DOT document from the given <see cref="DotReader" />.
        /// </summary>
        public static DotDocument Load(DotReader reader)
        {
            var parser = new DotParser(reader);
            parser.Parse();
            var root = parser.Root;
            if (root is null)
            {
                throw new InvalidOperationException("Cannot parse DOT document.");
            }

            return new DotDocument
            {
                Root = root,
            };
        }

        /// <summary>
        /// Saves a DOT document to the given <see cref="DotWriter"/>.
        /// </summary>
        /// <param name="writer"></param>
        public void Save(DotWriter writer)
        {
            if (Root is not null)
            {
                Save(writer, Root);
            }
        }

        static void Save(DotWriter writer, DotGraphNode node)
        {
            var tokens = EnumerateTokens(node);
            foreach (var token in tokens)
            {
                Save(writer, token);
            }
        }

        static void Save(DotWriter writer, DotToken token)
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

        static void Save(DotWriter writer, DotTrivia trivia)
        {
            if (!string.IsNullOrEmpty(trivia.Value))
            {
                writer.Write(trivia.Kind, trivia.Value);
            }
        }

        static IEnumerable<DotToken> EnumerateTokens(DotNode node)
        {
            foreach (var child in node.ChildNodesAndTokens)
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
