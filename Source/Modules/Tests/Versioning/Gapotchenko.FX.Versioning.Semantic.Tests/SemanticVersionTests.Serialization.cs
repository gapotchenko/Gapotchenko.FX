// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

#if TFF_JSON
using System.Text.Json;
#endif

namespace Gapotchenko.FX.Versioning.Semantic.Tests;

partial class SemanticVersionTests
{
#if TFF_JSON

    [TestMethod]
    [DataRow("\"1.2.3\"", "1.2.3")]
    [DataRow("\"1.2.3-label\"", "1.2.3-label")]
    [DataRow("null", null)]
    public void SemanticVersion_Serialization_Json(string json, string? version)
    {
        var obj = JsonSerializer.Deserialize<SemanticVersion>(json);
        Assert.AreEqual(SemanticVersion.Parse(version), obj);
    }

#endif
}
