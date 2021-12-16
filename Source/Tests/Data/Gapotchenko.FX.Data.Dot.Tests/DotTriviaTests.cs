using Gapotchenko.FX.Data.Dot.Dom;
using Gapotchenko.FX.Data.Dot.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Gapotchenko.FX.Data.Dot.Tests
{
    [TestClass]
    public class DotTriviaTests
    {
        [TestMethod]
        public void DotTrivia_001()
        {
            var text = @"
// Comment 1
digraph     // Comment 2
    MyGraph // Comment 3
{
    a /* Comment 4 */ -> /* Comment 5 */ b
    /* Comment 6 */ c = d // Comment 7
    // Comment 8
}";

            using var textReader = new StringReader(text);
            using var dotReader = DotReader.Create(textReader);
            var dom = DotDocument.Load(dotReader);
            var tokens = EnumerateTokens(dom);

            /*
               Trailing trivia only goes until the end of line, all subsequent trivia is
               considered leading trivia for the following token.

               +-----------+------------------+---------+
               |  Comment  | Leading/Trailing |  Token  |
               +-----------+------------------+---------+
               | Comment 1 | Leading          | digraph |
               | Comment 2 | Trailing         | digraph |
               | Comment 3 | Trailing         | MyGraph |
               | Comment 4 | Trailing         | a       |
               | Comment 5 | Trailing         | ->      |
               | Comment 6 | Leading          | c       |
               | Comment 7 | Trailing         | d       |
               | Comment 8 | Leading          | }       |
               +-----------+------------------+---------+
             */

            var cases = new[]
            {
                (comment: "Comment 1", leading: true, token: "digraph"),
                (comment: "Comment 2", leading: false, token: "digraph"),
                (comment: "Comment 3", leading: false, token: "MyGraph"),
                (comment: "Comment 4", leading: false, token: "a"),
                (comment: "Comment 5", leading: false, token: "->"),
                (comment: "Comment 6", leading: true, token: "c"),
                (comment: "Comment 7", leading: false, token: "d"),
                (comment: "Comment 8", leading: true, token: "}"),
            };

            int matches = 0;

            foreach (var token in tokens)
            {
                foreach (var (comment, leading, _) in cases.Where(c => c.token == token.Text))
                {
                    matches++;

                    var container = leading ? token.LeadingTrivia : token.TrailingTrivia;
                    var comments = container.Where(t => t.Kind is DotTokenKind.Comment or DotTokenKind.MultilineComment);
                    Assert.IsTrue(comments.Any(c => c.Text.IndexOf(comment) != -1));
                }
            }

            Assert.AreEqual(cases.Length, matches);
        }

        static IEnumerable<DotSignificantToken> EnumerateTokens(DotDocument dom)
        {
            var extractor = new TokensExtractor();
            dom.Root.Accept(extractor);
            return extractor.Tokens;
        }

        sealed class TokensExtractor : DotDomWalker
        {
            public List<DotSignificantToken> Tokens { get; } = new();
            public TokensExtractor() : base(DotDomWalkerDepth.NodesAndTokens) { }
            public override void VisitToken(DotSignificantToken token) => Tokens.Add(token);
        }
    }
}
