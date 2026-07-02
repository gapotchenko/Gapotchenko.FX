// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

#pragma warning disable CA5350 // Do Not Use Weak Cryptographic Algorithms

namespace Gapotchenko.FX.Security.Cryptography;

/// <summary>
/// Provides factory methods for creating instances of the
/// Triple Data Encryption Standard (3DES) algorithm
/// classified as unapproved for new security applications.
/// </summary>
/// <remarks>
/// Triple DES is retained for compatibility with legacy systems and data formats.
/// New security applications should use approved cryptographic algorithms instead.
/// </remarks>
public static class TripleDESUnapproved
{
    /// <summary>
    /// Creates a Triple DES algorithm instance classified as unapproved for new applications.
    /// </summary>
    /// <returns>A Triple DES algorithm instance.</returns>
    public static TripleDES Create()
    {
        return new TripleDESUnapprovedImpl();
    }
}
