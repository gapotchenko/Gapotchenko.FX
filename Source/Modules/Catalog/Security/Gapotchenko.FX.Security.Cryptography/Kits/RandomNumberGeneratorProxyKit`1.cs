// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.FX.Security.Cryptography.Kits;

/// <summary>
/// Provides a base implementation of <see cref="RandomNumberGenerator"/> proxy.
/// </summary>
/// <typeparam name="T">The type of the base random number generator.</typeparam>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public abstract class RandomNumberGeneratorProxyKit<T> : RandomNumberGenerator
    where T : RandomNumberGenerator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RandomNumberGeneratorProxyKit{T}"/> class with the specified base random number generator.
    /// </summary>
    /// <param name="baseRandomNumberGenerator">The base random number generator to create the proxy for.</param>
    /// <exception cref="ArgumentNullException"><paramref name="baseRandomNumberGenerator"/> is <see langword="null"/>.</exception>
    protected RandomNumberGeneratorProxyKit(T baseRandomNumberGenerator)
    {
        ArgumentNullException.ThrowIfNull(baseRandomNumberGenerator);

        BaseRandomNumberGenerator = baseRandomNumberGenerator;
    }

    /// <summary>
    /// Releases the unmanaged resources use by the <see cref="RandomNumberGenerator"/>
    /// and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing"><inheritdoc cref="RandomNumberGenerator.Dispose(bool)"/></param>
    protected sealed override void Dispose(bool disposing)
    {
        DisposeCore(disposing);

        base.Dispose(disposing);
    }

    /// <inheritdoc cref="RandomNumberGenerator.Dispose(bool)"/>
    protected virtual void DisposeCore(bool disposing)
    {
        if (disposing)
            BaseRandomNumberGenerator.Dispose();
    }

    /// <inheritdoc/>
    public override void GetBytes(byte[] data)
    {
        BaseRandomNumberGenerator.GetBytes(data);
    }

    /// <inheritdoc/>
    public override void GetBytes(byte[] data, int offset, int count)
    {
        BaseRandomNumberGenerator.GetBytes(data, offset, count);
    }

    /// <inheritdoc/>
    public override void GetNonZeroBytes(byte[] data)
    {
        BaseRandomNumberGenerator.GetNonZeroBytes(data);
    }

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER

    /// <inheritdoc/>
    public override void GetBytes(Span<byte> data)
    {
        BaseRandomNumberGenerator.GetBytes(data);
    }

    /// <inheritdoc/>
    public override void GetNonZeroBytes(Span<byte> data)
    {
        BaseRandomNumberGenerator.GetNonZeroBytes(data);
    }

#endif

    /// <summary>
    /// Gets the base random number generator.
    /// </summary>
    protected T BaseRandomNumberGenerator { get; }
}
