// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.FX.Security.Cryptography;

/// <summary>
/// Provides factory methods for creating instances of the
/// Alleged Rivest Cipher 4 (ARC4) algorithm
/// classified as unapproved for new applications.
/// </summary>
/// <remarks>
/// ARC4 is retained for compatibility with legacy systems and data formats.
/// New applications should use approved cryptographic algorithms instead.
/// </remarks>
public static class Arc4Unapproved
{
    /// <summary>
    /// Creates an ARC4 algorithm instance classified as unapproved for new applications.
    /// </summary>
    /// <returns>An ARC4 algorithm instance.</returns>
    public static Arc4 Create()
    {
        return new Arc4UnapprovedImpl();
    }
}
