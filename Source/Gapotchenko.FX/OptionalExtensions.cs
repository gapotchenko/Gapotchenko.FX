namespace Gapotchenko.FX;

/// <summary>
/// <see cref="Optional{T}"/> extensions.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class OptionalExtensions
{
    /// <summary>
    /// Returns a <see cref="Nullable{T}"/> value that corresponds to the given optional value.
    /// </summary>
    /// <typeparam name="T">The underlying type of the <see cref="Optional{T}"/> value.</typeparam>
    /// <param name="optional">The optional value.</param>
    /// <returns>A <see cref="Nullable{T}"/> value.</returns>
    public static T? ToNullable<T>(this Optional<T> optional) where T : struct => optional.HasValue ? optional.Value : null;

    /// <inheritdoc cref="ToNullable{T}(Optional{T})"/>
    [Obsolete("Use ToNullable method instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static T? AsNullable<T>(this Optional<T> optional) where T : struct => ToNullable(optional);
}
