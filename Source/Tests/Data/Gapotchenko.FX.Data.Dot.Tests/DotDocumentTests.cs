using Gapotchenko.FX.Data.Dot.Dom;
using Gapotchenko.FX.Data.Dot.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Gapotchenko.FX.Data.Dot.Tests
{
    public sealed class DotDocumentTests
    {
        public static IEnumerable<object[]> RoundtripTestData =>
                Assets.Items.Select(x => new object[] { x.title, x.document });

        [Theory]
        [MemberData(nameof(RoundtripTestData))]
#pragma warning disable xUnit1026 // Theory methods should use all of their parameters
#pragma warning disable IDE0060 // Remove unused parameter
        public void DotDocument_RoundtripTest(string title, string inputDocument)
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore xUnit1026 // Theory methods should use all of their parameters
        {
            using var textReader = new StringReader(inputDocument);
            using var dotReader = DotReader.Create(textReader);

            using var textWriter = new StringWriter();
            using var dotWriter = DotWriter.Create(textWriter);

            var dotDocument = DotDocument.Load(dotReader);

            dotDocument.Save(dotWriter);

            var outputDocument = textWriter.ToString();
            Assert.Equal(inputDocument, outputDocument);
        }
    }
}
