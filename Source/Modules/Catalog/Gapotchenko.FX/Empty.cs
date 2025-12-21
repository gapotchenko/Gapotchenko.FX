using System.Collections;

namespace Gapotchenko.FX;

/// <summary>
/// Provides operations related to a functional notion of emptiness.
/// </summary>
public static class Empty
{
    /// <summary>
    /// Nullifies an empty collection.
    /// </summary>
    /// <typeparam name="TSource">The type of the input collection.</typeparam>
    /// <param name="source">The source collection.</param>
    /// <returns>The source or a <see langword="null"/> collection if the source is empty or <see langword="null"/>.</returns>
    public static TSource? Nullify<TSource>(TSource? source) where TSource : class, ICollection
    {
        if (source == null || source.Count == 0)
            return null;
        else
            return source;
    }

    /// <summary>
    /// Nullifies an empty value.
    /// </summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="reserved">The reserved parameter used for method signature resolution.</param>
    /// <returns>The value or a <see langword="null"/> if the value is zero.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static T? Nullify<T>(T? value, int reserved = default) where T : class, IEmptiable
    {
        // The reserved parameter is needed to avoid the method signature conflict with
        // existing methods that should be kept to provide backward binary compatibility
        // between the module versions.
        _ = reserved;

        if (value is null || value.IsEmpty)
            return null;
        else
            return value;
    }

    /// <summary>
    /// Nullifies an empty value.
    /// </summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="reserved">The reserved parameter used for method signature resolution.</param>
    /// <returns>The value or a <see langword="null"/> if the value is zero.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static T? Nullify<T>(T value, short reserved = default) where T : struct, IEmptiable
    {
        // The reserved parameter is needed to avoid the method signature conflict with
        // existing methods that should be kept to provide backward binary compatibility
        // between the module versions.
        _ = reserved;

        if (value.IsEmpty)
            return null;
        else
            return value;
    }

    /// <summary>
    /// Nullifies an empty sequence.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of the input sequence.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <returns>The source or a <see langword="null"/> sequence if the source is empty or <see langword="null"/>.</returns>
    public static IEnumerable<TSource>? Nullify<TSource>(IEnumerable<TSource>? source)
    {
        if (source == null || !source.Any())
            return null;
        else
            return source;
    }

    /// <summary>
    /// Nullifies an empty array.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of the input array.</typeparam>
    /// <param name="source">The source array.</param>
    /// <returns>The source or a <see langword="null"/> array if the source is empty or <see langword="null"/>.</returns>
    public static TSource[]? Nullify<TSource>(TSource[]? source)
    {
        if (source == null || source.Length == 0)
            return null;
        else
            return source;
    }

    /// <summary>
    /// Nullifies an empty string.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The string value or a <see langword="null"/> if the string is empty or <see langword="null"/>.</returns>
    public static string? Nullify(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return null;
        else
            return value;
    }

    /// <summary>
    /// Nullifies an empty string.
    /// </summary>
    /// <param name="value">The value to nullify.</param>
    /// <param name="empty">The value to treat as empty.</param>
    /// <returns>The string value or a <see langword="null"/> if the string equals to <paramref name="empty"/> or is <see langword="null"/>.</returns>
    public static string? Nullify(string? value, string? empty)
    {
        if (value == null || value == empty)
            return null;
        else
            return value;
    }

    /// <summary>
    /// Nullifies an empty string.
    /// </summary>
    /// <param name="value">The value to nullify.</param>
    /// <param name="empty">The value to treat as empty.</param>
    /// <param name="comparison">The comparison.</param>
    /// <returns>
    /// The string value,
    /// or a <see langword="null"/> if the string equals to <paramref name="empty"/> using <paramref name="comparison"/> or the string is <see langword="null"/>.
    /// </returns>
    public static string? Nullify(string? value, string? empty, StringComparison comparison)
    {
        if (value == null || value.Equals(empty, comparison))
            return null;
        else
            return value;
    }

    /// <summary>
    /// Nullifies a string when it is empty, or consists only of white-space characters.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The string value or a <see langword="null"/> if the string is empty, or consists only of white-space characters.</returns>
    public static string? NullifyWhiteSpace(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;
        else
            return value;
    }

    /// <summary>
    /// Nullifies an empty type.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The type value or a <see langword="null"/> if the type is empty or <see langword="null"/>.
    /// The type is considered empty when it equals to the value returned by <see cref="Type"/> property of <see cref="Empty"/> class.
    /// </returns>
    public static Type? Nullify(Type? value)
    {
        if (value == Empty.Type)
            return null;
        else
            return value;
    }

    /// <summary>
    /// Nullifies a zero <see cref="SByte"/> value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The value or a <see langword="null"/> if the value is zero.</returns>
    [CLSCompliant(false)]
    public static sbyte? Nullify(sbyte value)
    {
        if (value == 0)
            return null;
        else
            return value;
    }

    /// <summary>
    /// Nullifies a zero <see cref="Byte"/> value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The value or a <see langword="null"/> if the value is zero.</returns>
    public static byte? Nullify(byte value)
    {
        if (value == 0)
            return null;
        else
            return value;
    }

    /// <summary>
    /// Nullifies a zero <see cref="Int16"/> value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The value or a <see langword="null"/> if the value is zero.</returns>
    public static short? Nullify(short value)
    {
        if (value == 0)
            return null;
        else
            return value;
    }

    /// <summary>
    /// Nullifies a zero <see cref="UInt16"/> value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The value or a <see langword="null"/> if the value is zero.</returns>
    [CLSCompliant(false)]
    public static ushort? Nullify(ushort value)
    {
        if (value == 0)
            return null;
        else
            return value;
    }

    /// <summary>
    /// Nullifies a zero <see cref="Int32"/> value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The value or a <see langword="null"/> if the value is zero.</returns>
    public static int? Nullify(int value)
    {
        if (value == 0)
            return null;
        else
            return value;
    }

    /// <summary>
    /// Nullifies a zero <see cref="UInt32"/> value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The value or a <see langword="null"/> if the value is zero.</returns>
    [CLSCompliant(false)]
    public static uint? Nullify(uint value)
    {
        if (value == 0)
            return null;
        else
            return value;
    }

    /// <summary>
    /// Nullifies a zero <see cref="Int64"/> value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The value or a <see langword="null"/> if the value is zero.</returns>
    public static long? Nullify(long value)
    {
        if (value == 0)
            return null;
        else
            return value;
    }

    /// <summary>
    /// Nullifies a zero <see cref="UInt64"/> value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The value or a <see langword="null"/> if the value is zero.</returns>
    [CLSCompliant(false)]
    public static ulong? Nullify(ulong value)
    {
        if (value == 0)
            return null;
        else
            return value;
    }

    /// <summary>
    /// Nullifies an empty <see cref="SByte"/> value.
    /// </summary>
    /// <param name="value">The value to nullify.</param>
    /// <param name="empty">The value to treat as empty.</param>
    /// <returns>The value or a <see langword="null"/> if the value is empty.</returns>
    [CLSCompliant(false)]
    public static sbyte? Nullify(sbyte value, sbyte empty)
    {
        if (value == empty)
            return null;
        else
            return value;
    }

    /// <summary>
    /// Nullifies an empty <see cref="Byte"/> value.
    /// </summary>
    /// <param name="value">The value to nullify.</param>
    /// <param name="empty">The value to treat as empty.</param>
    /// <returns>The value or a <see langword="null"/> if the value is empty.</returns>
    public static byte? Nullify(byte value, byte empty)
    {
        if (value == empty)
            return null;
        else
            return value;
    }

    /// <summary>
    /// Nullifies an empty <see cref="Int16"/> value.
    /// </summary>
    /// <param name="value">The value to nullify.</param>
    /// <param name="empty">The value to treat as empty.</param>
    /// <returns>The value or a <see langword="null"/> if the value is empty.</returns>
    public static short? Nullify(short value, short empty)
    {
        if (value == empty)
            return null;
        else
            return value;
    }

    /// <summary>
    /// Nullifies an empty <see cref="UInt16"/> value.
    /// </summary>
    /// <param name="value">The value to nullify.</param>
    /// <param name="empty">The value to treat as empty.</param>
    /// <returns>The value or a <see langword="null"/> if the value is empty.</returns>
    [CLSCompliant(false)]
    public static ushort? Nullify(ushort value, ushort empty)
    {
        if (value == empty)
            return null;
        else
            return value;
    }

    /// <summary>
    /// Nullifies an empty <see cref="Int32"/> value.
    /// </summary>
    /// <param name="value">The value to nullify.</param>
    /// <param name="empty">The value to treat as empty.</param>
    /// <returns>The value or a <see langword="null"/> if the value is empty.</returns>
    public static int? Nullify(int value, int empty)
    {
        if (value == empty)
            return null;
        else
            return value;
    }

    /// <summary>
    /// Nullifies an empty <see cref="UInt32"/> value.
    /// </summary>
    /// <param name="value">The value to nullify.</param>
    /// <param name="empty">The value to treat as empty.</param>
    /// <returns>The value or a <see langword="null"/> if the value is empty.</returns>
    [CLSCompliant(false)]
    public static uint? Nullify(uint value, uint empty)
    {
        if (value == empty)
            return null;
        else
            return value;
    }

    /// <summary>
    /// Nullifies an empty <see cref="Int64"/> value.
    /// </summary>
    /// <param name="value">The value to nullify.</param>
    /// <param name="empty">The value to treat as empty.</param>
    /// <returns>The value or a <see langword="null"/> if the value is empty.</returns>
    public static long? Nullify(long value, long empty)
    {
        if (value == empty)
            return null;
        else
            return value;
    }

    /// <summary>
    /// Nullifies an empty <see cref="UInt64"/> value.
    /// </summary>
    /// <param name="value">The value to nullify.</param>
    /// <param name="empty">The value to treat as empty.</param>
    /// <returns>The value or a <see langword="null"/> if the value is empty.</returns>
    [CLSCompliant(false)]
    public static ulong? Nullify(ulong value, ulong empty)
    {
        if (value == empty)
            return null;
        else
            return value;
    }

    /// <summary>
    /// Nullifies an empty <see cref="Decimal"/> value.
    /// </summary>
    /// <param name="value">The value to nullify.</param>
    /// <param name="empty">The value to treat as empty.</param>
    /// <returns>The value or a <see langword="null"/> if the value is empty.</returns>
    [CLSCompliant(false)]
    public static decimal? Nullify(decimal value, decimal empty)
    {
        if (value == empty)
            return null;
        else
            return value;
    }

    /// <summary>
    /// Nullifies an empty <typeparamref name="T"/> value.
    /// </summary>
    /// <param name="value">The value to nullify.</param>
    /// <param name="empty">The value to treat as empty.</param>
    /// <returns>The value or a <see langword="null"/> if the value is empty.</returns>
    public static T? Nullify<T>(T value, T empty) where T : struct, IEquatable<T>
    {
        if (value.Equals(empty))
            return null;
        else
            return value;
    }

    /// <summary>
    /// Nullifies an empty <typeparamref name="T"/> value.
    /// </summary>
    /// <param name="value">The value to nullify.</param>
    /// <param name="empty">The value to treat as empty.</param>
    /// <param name="reserved">The reserved parameter used for method signature resolution.</param>
    /// <returns>The value or a <see langword="null"/> if the value is empty.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static T? Nullify<T>(T value, T empty, int reserved = 0) where T : struct, Enum
    {
        // The reserved parameter is needed to avoid the method signature conflict with
        // an existing method that should be kept to provide backward binary compatibility
        // between the module versions.
        _ = reserved;

        if (EqualityComparer<T>.Default.Equals(value, empty))
            return null;
        else
            return value;
    }

    /// <summary>
    /// Nullifies an empty function in terms of lambda calculus provided by <see cref="Fn"/> class.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <returns>The value of <paramref name="action"/> or <see langword="null"/> if it represents an empty function in terms of lambda calculus.</returns>
    public static Action? Nullify(Action? action)
    {
        if (action == Fn.Empty)
            return null;
        else
            return action;
    }

    /// <summary>
    /// Nullifies a default function in terms of lambda calculus provided by <see cref="Fn"/> class.
    /// </summary>
    /// <param name="func">The function.</param>
    /// <returns>The value of <paramref name="func"/> or <see langword="null"/> if it represents a default function in terms of lambda calculus.</returns>
    public static Func<T>? Nullify<T>(Func<T>? func)
    {
        if (func == Fn<T>.Default)
            return null;
        else
            return func;
    }

    /// <summary>
    /// Nullifies an identity function in terms of lambda calculus provided by <see cref="Fn"/> class.
    /// </summary>
    /// <param name="func">The function.</param>
    /// <returns>The value of <paramref name="func"/> or <see langword="null"/> if it represents an identity function in terms of lambda calculus.</returns>
    public static Func<T, T>? Nullify<T>(Func<T, T>? func)
    {
        if (func == Fn<T>.Identity)
            return null;
        else
            return func;
    }

    /// <summary>
    /// Nullifies a default equality comparer for type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of objects compared by a comparer.</typeparam>
    /// <param name="comparer">The comparer.</param>
    /// <returns>The value of <paramref name="comparer"/> or <see langword="null"/> if it represents a default equality comparer for type <typeparamref name="T"/>.</returns>
    public static IEqualityComparer<T>? Nullify<T>(IEqualityComparer<T>? comparer)
    {
        if (comparer is null)
            return null;
        else if (comparer == EqualityComparer<T>.Default)
            return null;
        else
            return comparer;
    }

    /// <summary>
    /// Nullifies a default comparer for type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of objects compared by a comparer.</typeparam>
    /// <param name="comparer">The comparer.</param>
    /// <returns>The value of <paramref name="comparer"/> or <see langword="null"/> if it represents a default comparer for type <typeparamref name="T"/>.</returns>
    public static IComparer<T>? Nullify<T>(IComparer<T>? comparer)
    {
        if (comparer is null)
            return null;
        else if (Comparer<T>.Default.Equals(comparer))
            return null;
        else
            return comparer;
    }

    /// <summary>
    /// Returns an empty <see cref="System.Threading.Tasks.Task"/> that has already completed successfully.
    /// </summary>
#if TFF_COMPLETED_TASK
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public static Task Task =>
#if TFF_COMPLETED_TASK
        Task.CompletedTask;
#else
        Empty<Unit>.Task;
#endif

    /// <summary>
    /// Returns a type that is considered empty.
    /// </summary>
    /// <value>The type of <see cref="Empty"/> class.</value>
    public static Type Type { get; } = typeof(Empty);
}
