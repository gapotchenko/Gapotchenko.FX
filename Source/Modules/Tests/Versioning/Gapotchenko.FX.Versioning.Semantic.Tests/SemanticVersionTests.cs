// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.Versioning.Semantic.Tests;

[TestClass]
public partial class SemanticVersionTests
{
    // This type is partial.
    // Please take a look at the neighboring source files for the rest of the implementation.

    static void TestRoundTrip(SemanticVersion version)
    {
        Assert.AreEqual(version, SemanticVersion.Parse(version.ToString()));
    }
}
