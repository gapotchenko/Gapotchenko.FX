namespace Gapotchenko.FX.Security.Cryptography;

/// <summary>
/// Provides factory methods for creating unapproved implementations of the
/// Alleged Rivest Cipher 4 (ARC4) algorithm.
/// </summary>
/// <remarks>
/// ARC4 is retained for compatibility with legacy systems and data formats.
/// New applications should use approved cryptographic algorithms instead.
/// </remarks>
public static class Arc4Unapproved
{
    /// <summary>
    /// Creates an ARC4 algorithm instance that is outside the set of algorithms
    /// approved by <see cref="CryptographyPolicy"/>.
    /// </summary>
    /// <returns>An ARC4 algorithm instance.</returns>
    public static Arc4 Create()
    {
        return new Arc4UnapprovedImpl();
    }
}
