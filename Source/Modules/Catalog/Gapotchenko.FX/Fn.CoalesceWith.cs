// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

namespace Gapotchenko.FX;

partial class Fn
{
    #region Binary

    /// <summary>
    /// Applies a binary function to two optional values if both are present; 
    /// otherwise returns whichever value is present, or <c>None</c> if neither is present.
    /// </summary>
    /// <typeparam name="T">
    /// The underlying value type contained in the <see cref="Optional{T}"/>.
    /// </typeparam>
    /// <param name="func">
    /// A function used to combine the values when both <paramref name="a"/> and <paramref name="b"/> have values.
    /// </param>
    /// <param name="a">
    /// The first optional value.
    /// </param>
    /// <param name="b">
    /// The second optional value.
    /// </param>
    /// <returns>
    /// An <see cref="Optional{T}"/> containing:
    /// <list type="bullet">
    /// <item>
    /// The result of <paramref name="func"/> if both <paramref name="a"/> and <paramref name="b"/> have values.
    /// </item>
    /// <item>
    /// The value of <paramref name="a"/> or <paramref name="b"/> if only one has a value.
    /// </item>
    /// <item>
    /// <c>None</c> if neither <paramref name="a"/> nor <paramref name="b"/> has a value.
    /// </item>
    /// </list>
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="func"/> is <see langword="null"/>.</exception>
    public static Optional<T> CoalesceWith<T>(Func<T, T, T> func, Optional<T> a, Optional<T> b)
    {
        ArgumentNullException.ThrowIfNull(func);

        if (a.HasValue && b.HasValue)
            return func(a.Value, b.Value);
        else if (a.HasValue)
            return a.Value;
        else if (b.HasValue)
            return b.Value;
        else
            return Optional.None<T>();
    }

    /// <summary>
    /// Applies a binary function to two nullable values if both are present; 
    /// otherwise returns whichever value is present, or <see langword="null"/> if neither is present.
    /// </summary>
    /// <typeparam name="T">
    /// The underlying value type of the nullable operands.
    /// </typeparam>
    /// <param name="func">
    /// A function used to combine the values when both <paramref name="a"/> and <paramref name="b"/> have values.
    /// </param>
    /// <param name="a">
    /// The first nullable value.
    /// </param>
    /// <param name="b">
    /// The second nullable value.
    /// </param>
    /// <returns>
    /// A nullable value containing:
    /// <list type="bullet">
    /// <item>
    /// The result of <paramref name="func"/> if both <paramref name="a"/> and <paramref name="b"/> have values.
    /// </item>
    /// <item>
    /// The value of <paramref name="a"/> or <paramref name="b"/> if only one has a value.
    /// </item>
    /// <item>
    /// <see langword="null"/> if neither <paramref name="a"/> nor <paramref name="b"/> has a value.
    /// </item>
    /// </list>
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="func"/> is <see langword="null"/>.</exception>
    [return: NotNullIfNotNull(nameof(a))]
    [return: NotNullIfNotNull(nameof(b))]
    public static T? CoalesceWith<T>(Func<T, T, T> func, T? a, T? b)
        where T : struct
    {
        ArgumentNullException.ThrowIfNull(func);

        if (a.HasValue && b.HasValue)
            return func(a.Value, b.Value);
        else
            return a ?? b;
    }

    /// <summary>
    /// Applies a binary function to two reference values if both are not <see langword="null"/>;
    /// otherwise returns whichever value is not <see langword="null"/>, or <see langword="null"/> if neither is present.
    /// </summary>
    /// <remarks>
    /// The return value is guaranteed to be not <see langword="null"/> if either <paramref name="a"/> or <paramref name="b"/> is not <see langword="null"/>.
    /// </remarks>
    /// <typeparam name="T">
    /// The reference type of the input values.
    /// </typeparam>
    /// <param name="func">
    /// A function used to combine the values when both <paramref name="a"/> and <paramref name="b"/> are not <see langword="null"/>.
    /// </param>
    /// <param name="a">
    /// The first value, or <see langword="null"/>.
    /// </param>
    /// <param name="b">
    /// The second value, or <see langword="null"/>.
    /// </param>
    /// <returns>
    /// A value determined as follows:
    /// <list type="bullet">
    /// <item>
    /// The result of <paramref name="func"/> if both <paramref name="a"/> and <paramref name="b"/> are not <see langword="null"/>.
    /// </item>
    /// <item>
    /// <paramref name="a"/> or <paramref name="b"/> if only one is not <see langword="null"/>.
    /// </item>
    /// <item>
    /// <see langword="null"/> if both <paramref name="a"/> and <paramref name="b"/> are <see langword="null"/>.
    /// </item>
    /// </list>
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="func"/> is <see langword="null"/>.</exception>
    [return: NotNullIfNotNull(nameof(a))]
    [return: NotNullIfNotNull(nameof(b))]
    public static T? CoalesceWith<T>(Func<T, T, T> func, T? a, T? b)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(func);

        if (a is not null && b is not null)
            return func(a, b);
        else
            return a ?? b;
    }

    #endregion

    #region Ternary

    /// <summary>
    /// Applies a ternary function to three optional values if all are present;
    /// otherwise returns the first available value, or <c>None</c> if none are present.
    /// </summary>
    /// <typeparam name="T">
    /// The underlying value type contained in the <see cref="Optional{T}"/> instances.
    /// </typeparam>
    /// <param name="func">
    /// A function used to combine the values when <paramref name="a"/>, <paramref name="b"/>, 
    /// and <paramref name="c"/> all have values.
    /// </param>
    /// <param name="a">
    /// The first optional value.
    /// </param>
    /// <param name="b">
    /// The second optional value.
    /// </param>
    /// <param name="c">
    /// The third optional value.
    /// </param>
    /// <returns>
    /// An <see cref="Optional{T}"/> containing:
    /// <list type="bullet">
    /// <item>
    /// The result of <paramref name="func"/> if all three inputs have values.
    /// </item>
    /// <item>
    /// The value of <paramref name="a"/>, <paramref name="b"/>, or <paramref name="c"/> 
    /// (in that order of precedence) if at least one has a value.
    /// </item>
    /// <item>
    /// <c>None</c> if none of the inputs have values.
    /// </item>
    /// </list>
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="func"/> is <see langword="null"/>.</exception>
    public static Optional<T> CoalesceWith<T>(Func<T, T, T, T> func, Optional<T> a, Optional<T> b, Optional<T> c)
    {
        ArgumentNullException.ThrowIfNull(func);

        if (a.HasValue && b.HasValue && c.HasValue)
            return func(a.Value, b.Value, c.Value);
        else if (a.HasValue)
            return a.Value;
        else if (b.HasValue)
            return b.Value;
        else if (c.HasValue)
            return c.Value;
        else
            return Optional.None<T>();
    }

    /// <summary>
    /// Applies a ternary function to three nullable values if all are present; 
    /// otherwise returns the first available value, or <see langword="null"/> if none are present.
    /// </summary>
    /// <typeparam name="T">
    /// The underlying value type of the nullable operands.
    /// </typeparam>
    /// <param name="func">
    /// A function used to combine the values when <paramref name="a"/>, <paramref name="b"/>,
    /// and <paramref name="c"/> are all present.
    /// </param>
    /// <param name="a">
    /// The first nullable value.
    /// </param>
    /// <param name="b">
    /// The second nullable value.
    /// </param>
    /// <param name="c">
    /// The third nullable value.
    /// </param>
    /// <returns>
    /// A nullable value containing:
    /// <list type="bullet">
    /// <item>
    /// The result of <paramref name="func"/> if all three inputs have values.
    /// </item>
    /// <item>
    /// The value of <paramref name="a"/>, <paramref name="b"/>, or <paramref name="c"/> 
    /// (in that order of precedence) if at least one is not <see langword="null"/>.
    /// </item>
    /// <item>
    /// <see langword="null"/> if all inputs are <see langword="null"/>.
    /// </item>
    /// </list>
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="func"/> is <see langword="null"/>.</exception>
    [return: NotNullIfNotNull(nameof(a))]
    [return: NotNullIfNotNull(nameof(b))]
    [return: NotNullIfNotNull(nameof(c))]
    public static T? CoalesceWith<T>(Func<T, T, T, T> func, T? a, T? b, T? c)
        where T : struct
    {
        ArgumentNullException.ThrowIfNull(func);

        if (a.HasValue && b.HasValue && c.HasValue)
            return func(a.Value, b.Value, c.Value);
        else
            return a ?? b ?? c;
    }

    /// <summary>
    /// Applies a ternary function to three reference values if all are not <see langword="null"/>;
    /// otherwise returns whichever value is not <see langword="null"/>, or <see langword="null"/> if none are present.
    /// </summary>
    /// <remarks>
    /// The return value is guaranteed to be not <see langword="null"/>
    /// if any of <paramref name="a"/>, <paramref name="b"/>, or <paramref name="c"/> is not <see langword="null"/>.
    /// </remarks>
    /// <typeparam name="T">
    /// The reference type of the input values.
    /// </typeparam>
    /// <param name="func">
    /// A function used to combine the values when <paramref name="a"/>, <paramref name="b"/>,
    /// and <paramref name="c"/> are all not <see langword="null"/>.
    /// </param>
    /// <param name="a">
    /// The first value, or <see langword="null"/>.
    /// </param>
    /// <param name="b">
    /// The second value, or <see langword="null"/>.
    /// </param>
    /// <param name="c">
    /// The third value, or <see langword="null"/>.
    /// </param>
    /// <returns>
    /// A value determined as follows:
    /// <list type="bullet">
    /// <item>
    /// The result of <paramref name="func"/> if all inputs are not <see langword="null"/>.
    /// </item>
    /// <item>
    /// The value of <paramref name="a"/>, <paramref name="b"/>, or <paramref name="c"/> 
    /// (in that order of precedence) if at least one is not <see langword="null"/>.
    /// </item>
    /// <item>
    /// <see langword="null"/> if all inputs are <see langword="null"/>.
    /// </item>
    /// </list>
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="func"/> is <see langword="null"/>.</exception>
    [return: NotNullIfNotNull(nameof(a))]
    [return: NotNullIfNotNull(nameof(b))]
    [return: NotNullIfNotNull(nameof(c))]
    public static T? CoalesceWith<T>(Func<T, T, T, T> func, T? a, T? b, T? c)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(func);

        if (a is not null && b is not null && c is not null)
            return func(a, b, c);
        else
            return a ?? b ?? c;
    }

    #endregion
}
