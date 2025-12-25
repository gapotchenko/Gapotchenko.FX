// --------------------------------------------------------------------------
// API Compatibility Layer
// --------------------------------------------------------------------------

namespace Gapotchenko.FX.Linq;

partial class EnumeratorEx
{
#if SOURCE_COMPATIBILITY || BINARY_COMPATIBILITY

    /// <inheritdoc cref="EnumeratorExtensions.Rest{T}(IEnumerator{T})"/>
    [Obsolete("Use extension method of IEnumerator<T> interface instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IEnumerable<T> Rest<T>(IEnumerator<T> enumerator) => EnumeratorExtensions.Rest(enumerator);

#endif
}
