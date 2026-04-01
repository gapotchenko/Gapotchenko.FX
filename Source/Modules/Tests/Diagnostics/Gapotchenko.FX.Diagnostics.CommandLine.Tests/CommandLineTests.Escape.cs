extern alias testable;

namespace Gapotchenko.FX.Diagnostics.CommandLine.Tests;

using CommandLine = testable::Gapotchenko.FX.Diagnostics.CommandLine;

partial class CommandLineTests
{
    [TestMethod]
    public void CommandLine_EscapeArgument_PassThrough()
    {
        string argument = CommandLine.EscapeArgument("a");
        Assert.AreEqual("a", argument);
    }

    [TestMethod]
    public void CommandLine_EscapeArgument_Null()
    {
        string? argument = CommandLine.EscapeArgument(null);
        Assert.IsNull(argument);
    }

    [TestMethod]
    public void CommandLine_EscapeArgument_Empty()
    {
        string argument = CommandLine.EscapeArgument("");
        Assert.AreEqual("", argument);
    }

    [TestMethod]
    public void CommandLine_EscapeArgument_Quote()
    {
        string argument = CommandLine.EscapeArgument("a x");
        Assert.AreEqual("\"a x\"", argument);
    }

    [TestMethod]
    public void CommandLine_EscapeArgument_QuoteAndEscape()
    {
        string argument = CommandLine.EscapeArgument("a x \"y\"");
        Assert.AreEqual("\"a x \\\"y\\\"\"", argument);
    }

    [TestMethod]
    public void CommandLine_EscapeArgument_NoFileNameEncode()
    {
        string argument = CommandLine.EscapeArgument("-");
        Assert.AreEqual("-", argument);
    }

    [TestMethod]
    public void CommandLine_EncodeFileName_PassThrough()
    {
        string argument = CommandLine.EncodeFileName("a");
        Assert.AreEqual("a", argument);
    }

    [TestMethod]
    public void CommandLine_EncodeFileName_Null()
    {
        string? argument = CommandLine.EncodeFileName(null);
        Assert.IsNull(argument);
    }

    [TestMethod]
    public void CommandLine_EncodeFileName_Empty()
    {
        string argument = CommandLine.EncodeFileName("");
        Assert.AreEqual("", argument);
    }

    [TestMethod]
    public void CommandLine_EncodeFileName_Encode()
    {
        string argument = CommandLine.EncodeFileName("-");
        Assert.AreEqual("." + Path.DirectorySeparatorChar + "-", argument);
    }

    [TestMethod]
    public void CommandLine_EncodeFileName_NoQuote()
    {
        string argument = CommandLine.EncodeFileName("a x");
        Assert.AreEqual("a x", argument);
    }
}
