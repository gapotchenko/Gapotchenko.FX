// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.IO.Vfs.Tests;

partial class VfsSearchExpressionTests
{
    [TestMethod]
    public void VfsSearchExpression_Simple_Empty()
    {
        var vse = new VfsSearchExpression("", matchType: MatchType.Simple);
        AssertThatVseMatchesNoInputs(vse);
    }

    [TestMethod]
    [DataRow("foobar", false)]
    [DataRow("foo bar", true)]
    [DataRow("foo_bar", true)]
    [DataRow("FOO BAR", false)]
    [DataRow("FoO~BaR", true, VfsSearchExpressionOptions.IgnoreCase)]
    [DataRow("bar foo", false)]
    public void VfsSearchExpression_Simple_Smoke(
        string input,
        bool expected,
        VfsSearchExpressionOptions options = VfsSearchExpressionOptions.None)
    {
        var vse = new VfsSearchExpression("*foo?bar*", matchType: MatchType.Simple, options: options);

        Assert.AreEqual(expected, vse.IsMatch(input.AsSpan()), GetInputAssertionMessage(input));

        string[] modifiers = ["$", "$$$"];
        foreach (string modifier in modifiers)
        {
            string modifiedInput = modifier + input;
            Assert.AreEqual(expected, vse.IsMatch(modifiedInput.AsSpan()), GetInputAssertionMessage(modifiedInput));

            modifiedInput = input + modifier;
            Assert.AreEqual(expected, vse.IsMatch(modifiedInput.AsSpan()), GetInputAssertionMessage(modifiedInput));

            modifiedInput = modifier + input + modifier;
            Assert.AreEqual(expected, vse.IsMatch(modifiedInput.AsSpan()), GetInputAssertionMessage(modifiedInput));
        }
    }
}
