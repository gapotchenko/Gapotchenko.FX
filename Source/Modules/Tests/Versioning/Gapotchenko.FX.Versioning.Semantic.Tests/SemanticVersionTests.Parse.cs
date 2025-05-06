// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

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


}
