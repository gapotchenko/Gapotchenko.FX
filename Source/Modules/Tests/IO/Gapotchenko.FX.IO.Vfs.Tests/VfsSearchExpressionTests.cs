// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.IO.Vfs.Tests;

[TestClass]
public sealed partial class VfsSearchExpressionTests
{
    [TestMethod]
    public void VfsSearchExpression_Default()
    {
        var vse = new VfsSearchExpression();
        AssertThatVseMatchesAllInputs(vse);
    }

    [TestMethod]
    [DataRow(MatchType.Simple)]
    [DataRow(MatchType.Win32)]
    public void VfsSearchExpression_Null(MatchType matchType)
    {
        var vse = new VfsSearchExpression(null, matchType: matchType);
        AssertThatVseMatchesAllInputs(vse);
    }

    [TestMethod]
    [DataRow(MatchType.Simple)]
    [DataRow(MatchType.Win32)]
    public void VfsSearchExpression_Star(MatchType matchType)
    {
        var vse = new VfsSearchExpression("*", matchType: matchType);
        AssertThatVseMatchesNonEmptyInputs(vse);
    }

    [TestMethod]
    [DataRow(MatchType.Simple)]
    [DataRow(MatchType.Win32)]
    public void VfsSearchExpression_AnyPlus(MatchType matchType)
    {
        var vse = new VfsSearchExpression("?*", matchType: matchType);
        AssertThatVseMatchesNonEmptyInputs(vse);
    }

    [TestMethod]
    [DataRow(MatchType.Simple)]
    [DataRow(MatchType.Win32)]
    public void VfsSearchExpression_EndsWith(MatchType matchType)
    {
        const string suffix = "_suffix";

        Verify(new("*" + suffix, matchType: matchType), true);
        Verify(new("*" + suffix.ToUpperInvariant(), matchType: matchType), false);
        Verify(new("*" + suffix.ToUpperInvariant(), matchType: matchType, options: VfsSearchExpressionOptions.IgnoreCase), true);

        static void Verify(in VfsSearchExpression vse, bool expected)
        {
            foreach (string input in m_AllInputs.Select(x => x + suffix))
                Assert.AreEqual(expected, vse.IsMatch(input.AsSpan()), GetInputAssertionMessage(input));
        }
    }

    static void AssertThatVseMatchesAllInputs(in VfsSearchExpression vse)
    {
        foreach (string? input in m_AllInputs)
            Assert.IsTrue(vse.IsMatch(input.AsSpan()), GetInputAssertionMessage(input));
    }

    static void AssertThatVseMatchesNoInputs(in VfsSearchExpression vse)
    {
        foreach (string? input in m_AllInputs)
            Assert.IsFalse(vse.IsMatch(input.AsSpan()), GetInputAssertionMessage(input));
    }

    static void AssertThatVseMatchesNonEmptyInputs(in VfsSearchExpression vse)
    {
        foreach (string input in m_NonEmptyInputs)
            Assert.IsTrue(vse.IsMatch(input.AsSpan()), GetInputAssertionMessage(input));

        foreach (string? input in m_EmptyInputs)
            Assert.IsFalse(vse.IsMatch(input.AsSpan()), GetInputAssertionMessage(input));
    }

    static string GetInputAssertionMessage(string? input) => string.Format(
        "Mismatch for '{0}' input.",
        input ?? "<null>");

    static readonly IReadOnlyList<string?> m_EmptyInputs = [null, ""];

    static readonly IReadOnlyList<string> m_NonEmptyInputs = ["1", "ABC"];

    static readonly IReadOnlyList<string?> m_AllInputs = [.. m_EmptyInputs, .. m_NonEmptyInputs];
}
