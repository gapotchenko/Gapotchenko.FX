// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

#if NET11_0_OR_GREATER
#define TFF_STRING_INDEXOF_CHAR_COMPARISON
#endif

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
#define TFF_STRING_OPS_COMPARISON
#endif

#if NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
#define TFF_STRING_JOIN_CHAR_ENUMERABLE
#endif

using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.Text;

/// <summary>
/// Provides polyfill methods for <see cref="string"/> type.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static partial class StringPolyfills
{
    /// <summary>
    /// Provides static polyfill methods for <see cref="string"/> type.
    /// </summary>
    extension(string)
    {
        /// <summary>
        /// Concatenates the members of a collection, using the specified separator between each member.
        /// </summary>
        /// <typeparam name="T">The type of the members of <paramref name="values"/>.</typeparam>
        /// <param name="separator">
        /// The character to use as a separator.
        /// <paramref name="separator"/> is included in the returned string only if values has more than one element.
        /// </param>
        /// <param name="values">A collection that contains the objects to concatenate.</param>
        /// <returns>
        /// A string that consists of the members of <paramref name="values"/> delimited by the <paramref name="separator"/> character;
        /// or an empty string if <paramref name="values"/> has no elements.
        /// </returns>
#if TFF_STRING_JOIN_CHAR_ENUMERABLE
        [EditorBrowsable(EditorBrowsableState.Never)]
#endif
        public static string Join<T>(char separator, IEnumerable<T> values)
        {
#if TFF_STRING_JOIN_CHAR_ENUMERABLE
            return string.Join(separator, values);
#else
            return string.Join(new string(separator, 1), values);
#endif
        }
    }

    #region Contains

    /// <summary>
    /// <para>
    /// Returns a value indicating whether a specified character occurs within this string,
    /// using the specified comparison rules.
    /// </para>
    /// <para>
    /// This is a polyfill provided by Gapotchenko.FX.
    /// </para>
    /// </summary>
    /// <param name="s">The string instance.</param>
    /// <param name="value">The character to seek.</param>
    /// <param name="comparisonType">One of the enumeration values that specifies the rules to use in the comparison.</param>
    /// <returns>
    /// <see langword="true"/> if the <paramref name="value parameter"/> occurs within this string;
    /// otherwise, <see langword="false"/>.
    /// </returns>
#if TFF_STRING_CONTAINS_CHAR
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public static bool Contains(
#if !TFF_STRING_CONTAINS_CHAR
        this
#endif
        string s, char value, StringComparison comparisonType)
    {
        ArgumentNullException.ThrowIfNull(s);

#if TFF_STRING_CONTAINS_CHAR
        return s.Contains(value, comparisonType);
#else
        ReadOnlySpan<char> span = stackalloc char[] { value };
        return s.AsSpan().Contains(span, comparisonType);
#endif
    }

    /// <summary>
    /// Returns a value indicating whether a specified string occurs within this string, using the specified comparison rules.
    /// </summary>
    /// <param name="s">The string instance.</param>
    /// <param name="value">The string to seek.</param>
    /// <param name="comparisonType">One of the enumeration values that specifies the rules to use in the comparison.</param>
    /// <returns>
    /// <see langword="true"/> if the <paramref name="value"/> parameter occurs within this string,
    /// or if <paramref name="value"/> is the empty string ("");
    /// otherwise, <see langword="false"/>.
    /// </returns>
#if TFF_STRING_OPS_COMPARISON
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public static bool Contains(
#if !TFF_STRING_OPS_COMPARISON
        this
#endif
        string s, string value, StringComparison comparisonType)
    {
        ArgumentNullException.ThrowIfNull(s);

#if TFF_STRING_OPS_COMPARISON
        return s.Contains(value, comparisonType);
#else
        return s.IndexOf(value, comparisonType) != -1;
#endif
    }

    #endregion

    #region IndexOf

    /// <summary>
    /// Reports the zero-based index of the first occurrence of the specified character in this string,
    /// using the specified comparison rules.
    /// </summary>
    /// <param name="s">The string instance.</param>
    /// <param name="value">The character to seek.</param>
    /// <param name="comparisonType">One of the enumeration values that specifies the rules to use in the comparison.</param>
    /// <returns>
    /// The zero-based index position of <paramref name="value"/> if that character is found;
    /// otherwise, <c>-1</c>.
    /// </returns>
#if TFF_STRING_INDEXOF_CHAR_COMPARISON
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public static int IndexOf(
#if !TFF_STRING_INDEXOF_CHAR_COMPARISON
        this
#endif
        string s, char value, StringComparison comparisonType)
    {
        ArgumentNullException.ThrowIfNull(s);

#if TFF_STRING_INDEXOF_CHAR_COMPARISON
        return s.IndexOf(value, comparisonType);
#else
        ReadOnlySpan<char> span = stackalloc char[] { value };
        return s.AsSpan().IndexOf(span, comparisonType);
#endif
    }

    /// <summary>
    /// Reports the zero-based index of the first occurrence of the specified character in this string.
    /// Parameters specify the starting search position and the comparison rules to use.
    /// </summary>
    /// <param name="s">The string instance.</param>
    /// <param name="value">The character to seek.</param>
    /// <param name="startIndex">The search starting position.</param>
    /// <param name="comparisonType">One of the enumeration values that specifies the rules to use in the comparison.</param>
    /// <returns>
    /// The zero-based index position of <paramref name="value"/> from the start of this string if that character is found;
    /// otherwise, <c>-1</c>.
    /// </returns>
#if TFF_STRING_INDEXOF_CHAR_COMPARISON
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public static int IndexOf(
#if !TFF_STRING_INDEXOF_CHAR_COMPARISON
        this
#endif
        string s, char value, int startIndex, StringComparison comparisonType)
    {
        ArgumentNullException.ThrowIfNull(s);

#if TFF_STRING_INDEXOF_CHAR_COMPARISON
        return s.IndexOf(value, startIndex, comparisonType);
#else
        return IndexOf(s, value, startIndex, s.Length - startIndex, comparisonType);
#endif
    }

    /// <summary>
    /// Reports the zero-based index of the first occurrence of the specified character in this string.
    /// Parameters specify the starting search position, the number of characters to search,
    /// and the comparison rules to use.
    /// </summary>
    /// <param name="s">The string instance.</param>
    /// <param name="value">The character to seek.</param>
    /// <param name="startIndex">The search starting position.</param>
    /// <param name="count">The number of character positions to examine.</param>
    /// <param name="comparisonType">One of the enumeration values that specifies the rules to use in the comparison.</param>
    /// <returns>
    /// The zero-based index position of <paramref name="value"/> from the start of this string if that character is found;
    /// otherwise, <c>-1</c>.
    /// </returns>
#if TFF_STRING_INDEXOF_CHAR_COMPARISON
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public static int IndexOf(
#if !TFF_STRING_INDEXOF_CHAR_COMPARISON
        this
#endif
        string s, char value, int startIndex, int count, StringComparison comparisonType)
    {
        ArgumentNullException.ThrowIfNull(s);

#if TFF_STRING_INDEXOF_CHAR_COMPARISON
        return s.IndexOf(value, startIndex, count, comparisonType);
#else
        if (comparisonType == StringComparison.Ordinal)
        {
            return s.IndexOf(value, startIndex, count);
        }
        else
        {
            ValidateRange(s, startIndex, count);

            ReadOnlySpan<char> valueSpan = stackalloc char[] { value };
            int index = s.AsSpan(startIndex, count).IndexOf(valueSpan, comparisonType);
            return index < 0 ? index : startIndex + index;
        }
#endif
    }

    #endregion

#if !TFF_STRING_INDEXOF_CHAR_COMPARISON

    static void ValidateRange(
        string s,
        int startIndex,
        int count,
        [CallerArgumentExpression(nameof(startIndex))] string? startIndexParameterName = null,
        [CallerArgumentExpression(nameof(count))] string? countParameterName = null)
    {
        if ((uint)startIndex > (uint)s.Length)
            throw new ArgumentOutOfRangeException(startIndexParameterName);

        if ((uint)count > (uint)(s.Length - startIndex))
            throw new ArgumentOutOfRangeException(countParameterName);
    }

#endif

    /// <summary>
    /// <para>
    /// Returns the hash code for this string using the specified rules.
    /// </para>
    /// <para>
    /// This is a polyfill provided by Gapotchenko.FX.
    /// </para>
    /// </summary>
    /// <param name="s">The string instance.</param>
    /// <param name="comparisonType">One of the enumeration values that specifies the rules to use in the comparison.</param>
    /// <returns>A 32-bit signed integer hash code.</returns>
#if TFF_STRING_OPS_COMPARISON
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public static int GetHashCode(
#if !TFF_STRING_OPS_COMPARISON
        this
#endif
        string s, StringComparison comparisonType)
    {
        ArgumentNullException.ThrowIfNull(s);

#if TFF_STRING_OPS_COMPARISON
        return s.GetHashCode(comparisonType);
#else
        return GetComparer(comparisonType).GetHashCode(s);

        static StringComparer GetComparer(StringComparison comparisonType) =>
            comparisonType switch
            {
                StringComparison.CurrentCulture => StringComparer.CurrentCulture,
                StringComparison.CurrentCultureIgnoreCase => StringComparer.CurrentCultureIgnoreCase,
                StringComparison.InvariantCulture => StringComparer.InvariantCulture,
                StringComparison.InvariantCultureIgnoreCase => StringComparer.InvariantCultureIgnoreCase,
                StringComparison.Ordinal => StringComparer.Ordinal,
                StringComparison.OrdinalIgnoreCase => StringComparer.OrdinalIgnoreCase,
                _ => throw new ArgumentException("Specified string comparison is not supported.", nameof(comparisonType))
            };
#endif
    }
}
