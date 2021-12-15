using Gapotchenko.FX.Data.Dot.Dom;
using Gapotchenko.FX.Data.Dot.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Dot.Tests
{
    [TestClass]
    public class DotFormattingTests
    {
        [TestMethod]
        public void DotFormatting_001()
        {
            var text = @"digraph{abra;cadabra[attr1=val1;attr2=val2;];c->{a;b;};}";

            var actualText = Apply(
                text,
                dom => dom.Root!.Accept(new TerminatorCleaner()));

            var expectedText = @"digraph{abra cadabra[attr1=val1 attr2=val2]c->{a b}}";
            Assert.AreEqual(expectedText, actualText);
        }

        [TestMethod]
        public void DotFormatting_002()
        {
            var text = "// Comment\n\n\ndigraph{}";

            var actualText = Apply(
                text,
                dom => dom.Root!.Accept(new WhitespaceCleaner()));

            var expectedText = "// Comment\r\ndigraph{}";
            Assert.AreEqual(expectedText, actualText);
        }

        static string Apply(string text, Action<DotDocument> action)
        {
            using var textReader = new StringReader(text);
            using var dotReader = DotReader.Create(textReader);
            var dom = DotDocument.Load(dotReader);

            action(dom);

            var textWriter = new StringWriter();
            var dotWriter = DotWriter.Create(textWriter);
            dom.Save(dotWriter);

            return textWriter.ToString();
        }

        sealed class TerminatorCleaner : DotDomWalker
        {
            public override void VisitDotAttributeNode(DotAttributeNode node)
            {
                base.VisitDotAttributeNode(node);
                node.SemicolonOrCommaToken = null;
            }

            protected override void DefaultVisit(DotNode node)
            {
                base.DefaultVisit(node);

                if (node is DotStatementNode statement)
                {
                    statement.SemicolonToken = null;
                }
            }
        }

        sealed class WhitespaceCleaner : DotDomWalker
        {
            protected override void DefaultVisit(DotNode node)
            {
                base.DefaultVisit(node);

                if (node.HasLeadingTrivia)
                    RemoveWhitespaces(node.LeadingTrivia);
                if (node.HasTrailingTrivia)
                    RemoveWhitespaces(node.TrailingTrivia);

                static void RemoveWhitespaces(IList<DotTrivia> list)
                {
                    var toRemove = list
                        .Select((x, id) => (x, id))
                        .Where(x => x.x.Kind is DotTokenKind.Whitespace)
                        .Select(x => x.id)
                        .OrderByDescending(x => x)
                        .ToList();

                    foreach (var i in toRemove)
                    {
                        list.RemoveAt(i);
                    }
                }
            }
        }
    }
}
