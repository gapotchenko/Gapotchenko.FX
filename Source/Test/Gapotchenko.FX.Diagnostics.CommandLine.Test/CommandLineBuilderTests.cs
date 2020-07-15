using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Diagnostics.CommandLine.Test
{
    [TestClass]
    public class CommandLineBuilderTests
    {
        [TestMethod]
        public void CommandLineBuilder_AppendTwoArguments()
        {
            var clb = new CommandLineBuilder()
                .AppendArgument("a")
                .AppendArgument("b");

            Assert.AreEqual("a b", clb.ToString());
        }

        [TestMethod]
        public void CommandLineBuilder_AppendTwoArgumentsWithDuplicateDelimeters()
        {
            var clb = new CommandLineBuilder()
                .AppendArgument("a")
                .DelimitArguments()
                .DelimitArguments()
                .AppendArgument("b");

            Assert.AreEqual("a b", clb.ToString());
        }
    }
}
