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
/// Data Encryption Standard (DES) algorithm
/// classified as unapproved for new security applications.
/// </summary>
/// <remarks>
/// DES is retained for compatibility with legacy systems and data formats.
/// New security applications should use approved cryptographic algorithms instead.
/// </remarks>
public static class DESUnapproved
{
    /// <summary>
    /// Creates a DES algorithm instance classified as unapproved for new applications.
    /// </summary>
    /// <returns>A DES algorithm instance.</returns>
    public static DES Create()
    {
        return new DESUnapprovedImpl();
    }
}
