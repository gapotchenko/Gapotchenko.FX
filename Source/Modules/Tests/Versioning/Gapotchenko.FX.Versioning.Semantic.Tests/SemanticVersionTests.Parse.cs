// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Versioning.Semantic.Tests;

partial class SemanticVersionTests
{
    static IEnumerable<object[]> SemanticVersion_Parse_TestData_ValidInputs
    {
        get
        {
            yield return ["1.0.0"];
            yield return ["1.0.0-alpha"];
            yield return ["1.0.0-alpha.1"];
            yield return ["1.0.0-alpha.beta"];
        }
    }

    static IEnumerable<object[]> SemanticVersion_Parse_TestData_InvalidInputs
    {
        get
        {
            yield return [""];
            yield return ["."];
            yield return [","];
            yield return ["1.abc.3"];
            yield return ["123!"];
            yield return ["1a:2b:3c"];
            yield return ["123,abc,123"];
        }
    }

    #region Parse

    [TestMethod]
    [DynamicData(nameof(SemanticVersion_Parse_TestData_ValidInputs))]
    public void SemanticVersion_Parse_ValidInput(string input)
    {
        var tfm = SemanticVersion_Parse_ProcessesInput(input);
        Assert.AreEqual(new SemanticVersion(input), tfm);
    }

    [TestMethod]
    public void SemanticVersion_Parse_NullInput()
    {
        Assert.IsNull(SemanticVersion_Parse_ProcessesInput(null));
    }

    [return: NotNullIfNotNull(nameof(input))]
    static SemanticVersion? SemanticVersion_Parse_ProcessesInput(string? input)
    {
        var version = SemanticVersion.Parse(input);

#if TODO
        if (input is null)
        {
            Assert.ThrowsExactly<FormatException>(() => SemanticVersion.Parse(input.AsSpan()));
        }
        else
        {
            var anotherVersion = SemanticVersion.Parse(input.AsSpan());
            Assert.AreEqual(version, anotherVersion);
        }
#endif

        return version;
    }

    [TestMethod]
    [DynamicData(nameof(SemanticVersion_Parse_TestData_InvalidInputs))]
    public void SemanticVersion_Parse_InvalidInput(string input)
    {
        Assert.ThrowsExactly<FormatException>(() => SemanticVersion.Parse(input));
#if TODO
        Assert.ThrowsExactly<FormatException>(() => SemanticVersion.Parse(input.AsSpan()));
#endif
    }

    #endregion

    #region TryParse

    [TestMethod]
    [DynamicData(nameof(SemanticVersion_Parse_TestData_ValidInputs))]
    public void SemanticVersion_TryParse_ValidInput(string input)
    {
        var version = SemanticVersion_TryParse_ProcessesInput(input);
        Assert.AreEqual(new SemanticVersion(input), version);
    }

    static SemanticVersion SemanticVersion_TryParse_ProcessesInput(string input)
    {
        var version = SemanticVersion.TryParse(input);
        Assert.IsNotNull(version);

        Assert.IsTrue(SemanticVersion.TryParse(input, out var anotherVersion));
        Assert.IsNotNull(anotherVersion);
        Assert.AreEqual(version, anotherVersion);

        return version;
    }

    [TestMethod]
    [DynamicData(nameof(SemanticVersion_Parse_TestData_InvalidInputs))]
    public void SemanticVersion_TryParse_InvalidInput(string input) => SemanticVersion_TryParse_FailsOnInput(input);

    [TestMethod]
    public void SemanticVersion_TryParse_NullInput() => SemanticVersion_TryParse_FailsOnInput(null);

    static void SemanticVersion_TryParse_FailsOnInput(string? input)
    {
        var version = SemanticVersion.TryParse(input);
        Assert.IsNull(version);

        Assert.IsFalse(SemanticVersion.TryParse(input, out version));
        Assert.IsNull(version);
    }

    #endregion
}
