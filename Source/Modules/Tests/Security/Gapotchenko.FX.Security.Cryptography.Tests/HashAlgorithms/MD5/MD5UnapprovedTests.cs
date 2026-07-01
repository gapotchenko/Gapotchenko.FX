// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.FX.Security.Cryptography.Tests.HashAlgorithms.MD5;

using MD5 = System.Security.Cryptography.MD5;

[TestClass]
public sealed class MD5UnapprovedTests : MD5Test
{
    protected override MD5 CreateAlgorithm()
    {
        return MD5Unapproved.Create();
    }
}
