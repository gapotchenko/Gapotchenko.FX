// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.IO.Kits;

/// <inheritdoc/>
public abstract class StreamProxyKit : StreamProxyKit<Stream>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StreamProxyKit"/> class with the specified base stream.
    /// </summary>
    /// <inheritdoc/>
    protected StreamProxyKit(Stream baseStream) :
        base(baseStream)
    {
    }
}
