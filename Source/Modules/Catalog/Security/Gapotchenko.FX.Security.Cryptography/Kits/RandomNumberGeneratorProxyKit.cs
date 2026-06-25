// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.FX.Security.Cryptography.Kits;

/// <inheritdoc/>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public abstract class RandomNumberGeneratorProxyKit : RandomNumberGeneratorProxyKit<RandomNumberGenerator>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RandomNumberGeneratorProxyKit"/> class with the specified base random number generator.
    /// </summary>
    /// <inheritdoc/>
    protected RandomNumberGeneratorProxyKit(RandomNumberGenerator baseRandomNumberGenerator) :
        base(baseRandomNumberGenerator)
    {
    }
}
