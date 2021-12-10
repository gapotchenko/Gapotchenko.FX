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
    public class DotIdentifierTests
    {
        [TestMethod]
        public void DotIdentifier_001()
        {
            TestIdentifier(@"1", "1");
            TestIdentifier(@"""2""", "2");
            TestIdentifier(@"""3\""""", "3\"");
            TestIdentifier(@"""\4""", "4");
            TestIdentifier(@"""\\""", "\\");
            TestIdentifier(@"""a\ b""", "a b");
            TestIdentifier(@"""a\bc""", "abc");
            TestIdentifier(@"""a\nb""", "a\nb");
            TestIdentifier("\"a\\\nb\"", "ab");
            TestIdentifier("\"a\\\r\nb\"", "ab");
            TestIdentifier("\"a\nb\"", "a\nb");
            TestIdentifier("\"a\r\nb\"", "a\r\nb");
            TestIdentifier(@"<""\\""a\a>", "\"\\\"aa");
            TestIdentifier(@"<<tag>>", "<tag>");
        }

        [TestMethod]
        public void DotIdentifier_002()
        {
            var identifierToken = GetIdentifierToken("\"\\G\"");
            Assert.ThrowsException<FormatException>(() => identifierToken?.Value);
        }

        static void TestIdentifier(string text, string expectedValue)
        {
            var identifierToken = GetIdentifierToken(text);

            var expectedText = RemoveEnclosingChars(text);

            Assert.AreEqual(expectedText, identifierToken?.Text);
            Assert.AreEqual(expectedValue, identifierToken?.Value);
        }

        static DotToken? GetIdentifierToken(string text)
        {
            var dot = $"digraph {{ {text} }}";
            var document = ParseDot(dot);
            var vertexStatement = (DotVertexNode?)document?.Root?.Statements?.Statements?.Single();
            return vertexStatement?.Identifier?.Identifier;
        }

        static string RemoveEnclosingChars(string text)
        {
            if (text.Length >= 2)
            {
                if (text.StartsWith("\"") && text.EndsWith("\""))
                    text = text.Substring(1, text.Length - 2);
                else if (text.StartsWith("<") && text.EndsWith(">"))
                    text = text.Substring(1, text.Length - 2);
            }

            return text;
        }

        static DotDocument ParseDot(string sourceText)
        {
            using var stringReader = new StringReader(sourceText);
            using var dotReader = DotReader.Create(stringReader);
            return DotDocument.Load(dotReader);
        }
    }
}
