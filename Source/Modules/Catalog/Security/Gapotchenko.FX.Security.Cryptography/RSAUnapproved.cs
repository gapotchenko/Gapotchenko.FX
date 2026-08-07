// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.FX.Security.Cryptography;

/// <summary>
/// Provides factory methods for creating instances of the
/// RSA algorithm
/// classified as unapproved for new security applications.
/// </summary>
/// <remarks>
/// Unapproved RSA is retained for compatibility with legacy systems and data formats.
/// New security applications should use approved cryptographic algorithms instead.
/// </remarks>
public static class RSAUnapproved
{
    /// <summary>
    /// Creates an RSA algorithm instance classified as unapproved for new security applications.
    /// </summary>
    /// <returns>An RSA algorithm instance.</returns>
    public static RSA Create()
    {
        return new RSAUnapprovedImpl();
    }
}
