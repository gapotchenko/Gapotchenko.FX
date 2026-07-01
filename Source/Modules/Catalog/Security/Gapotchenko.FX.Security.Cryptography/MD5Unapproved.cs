// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

#pragma warning disable CA5351 // Do Not Use Broken Cryptographic Algorithms

namespace Gapotchenko.FX.Security.Cryptography;

/// <summary>
/// Provides factory methods for creating instances of the
/// MD5 hash algorithm
/// classified as unapproved for new security applications.
/// </summary>
/// <remarks>
/// MD5 is retained for compatibility with legacy systems and data formats.
/// New security applications should use approved cryptographic algorithms instead.
/// </remarks>
public static class MD5Unapproved
{
    /// <summary>
    /// Creates a MD5 hash algorithm instance classified as unapproved for new security applications.
    /// </summary>
    /// <returns>A MD5 hash algorithm instance.</returns>
    public static MD5 Create()
    {
        return new MD5UnapprovedImpl();
    }
}
