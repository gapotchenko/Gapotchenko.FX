// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Versioning.Semantic.Tests;

[TestClass]
public partial class SemanticVersionTests
{
    // This class is partial. Please take a look at the neighboring source files.

    static void TestRoundTrip(SemanticVersion version)
    {
        Assert.AreEqual(version, SemanticVersion.Parse(version.ToString()));
    }
}
