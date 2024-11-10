using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Diagnostics.CommandLine.Tests;

[TestClass]
public class CommandLineBuilderTests
{
    [TestMethod]
    public void CommandLineBuilder_AppendTwoStrings()
    {
        var clb = new CommandLineBuilder()
            .AppendArgument("a")
            .AppendArgument("b");

        Assert.AreEqual("a b", clb.ToString());
    }

    [TestMethod]
    public void CommandLineBuilder_AppendTwoStringsWithDuplicateDelimeters()
    {
        var clb = new CommandLineBuilder()
            .AppendArgument("a")
            .DelimitArguments()
            .DelimitArguments()
            .AppendArgument("b");

        Assert.AreEqual("a b", clb.ToString());
    }

    [TestMethod]
    public void CommandLineBuilder_AppendVersion()
    {
        var clb = new CommandLineBuilder().AppendArgument(new Version(1, 2, 3));
        Assert.AreEqual("1.2.3", clb.ToString());
    }

    [TestMethod]
    public void CommandLineBuilder_AppendNull()
    {
        var clb = new CommandLineBuilder().AppendArgument(null);
        Assert.AreEqual("", clb.ToString());
    }

    [TestMethod]
    public void CommandLineBuilder_AppendNullString()
    {
        var clb = new CommandLineBuilder().AppendArgument((string?)null);
        Assert.AreEqual("", clb.ToString());
    }

    [TestMethod]
    public void CommandLineBuilder_AppendNullObject()
    {
        var clb = new CommandLineBuilder().AppendArgument((object?)null);
        Assert.AreEqual("", clb.ToString());
    }
}
