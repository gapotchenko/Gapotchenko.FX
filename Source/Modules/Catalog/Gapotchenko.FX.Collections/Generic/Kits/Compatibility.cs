#if SOURCE_COMPATIBILITY || BINARY_COMPATIBILITY

using Gapotchenko.FX.Collections.Generic.Kits;

#pragma warning disable IDE0130 // Namespace does not match folder structure
#pragma warning disable IDE0161 // Convert to file-scoped namespace

namespace Gapotchenko.FX.Collections.Generic.Kit
{
    /// <inheritdoc/>
    [Obsolete("Use Gapotchenko.FX.Collections.Generic.Kits.ReadOnlySetKit<T> instead.", true)] // Y2024
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class ReadOnlySetBase<T> : ReadOnlySetKit<T>
    {
    }

    /// <inheritdoc/>
    [Obsolete("Use Gapotchenko.FX.Collections.Generic.Kits.SetKit<T> instead.", true)] // Y2024
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class SetBase<T> : SetKit<T>
    {
    }
}

#endif
