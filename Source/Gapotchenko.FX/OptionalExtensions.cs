namespace Gapotchenko.FX;

/// <summary>
/// <see cref="Optional{T}"/> extensions.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class OptionalExtensions
{
    /// <summary>
    /// Returns <see cref="Nullable{T}"/> that corresponds to the given optional value.
    /// </summary>
    /// <typeparam name="T">The underlying type of the <see cref="Optional{T}"/> value.</typeparam>
    /// <returns>A <see cref="Nullable{T}"/> value.</returns>
    public static T? AsNullable<T>(this Optional<T> optional) where T : struct => optional.HasValue ? optional.Value : null;
}
