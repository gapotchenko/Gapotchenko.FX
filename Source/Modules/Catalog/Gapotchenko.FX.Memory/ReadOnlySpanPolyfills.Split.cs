// Portions © .NET Foundation and its licensors

// --------------------------------------------------------------------------

#if NET8_0_OR_GREATER
#define TFF_READONLYSPAN_CHAR_SPLIT
#endif

#if NET5_0_OR_GREATER
#define TFF_STRINGSPLITOPTIONS_TRIMENTRIES
#endif

// --------------------------------------------------------------------------

using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;

#if !TFF_STRINGSPLITOPTIONS_TRIMENTRIES
#pragma warning disable CS1574 // XML comment has cref attribute that could not be resolved
#endif

namespace Gapotchenko.FX.Memory;

partial class ReadOnlySpanPolyfills
{
    /// <summary>
    /// Parses the source <see cref="ReadOnlySpan{Char}"/> for the specified <paramref name="separator"/>, populating the <paramref name="destination"/> span
    /// with <see cref="Range"/> instances representing the regions between the separators.
    /// </summary>
    /// <param name="source">The source span to parse.</param>
    /// <param name="destination">The destination span into which the resulting ranges are written.</param>
    /// <param name="separator">A character that delimits the regions in this instance.</param>
    /// <param name="options">A bitwise combination of the enumeration values that specifies whether to trim whitespace and include empty ranges.</param>
    /// <returns>The number of ranges written into <paramref name="destination"/>.</returns>
    /// <remarks>
    /// <para>
    /// Delimiter characters are not included in the elements of the returned array.
    /// </para>
    /// <para>
    /// If the <paramref name="destination"/> span is empty, or if the <paramref name="options"/> specifies <see cref="StringSplitOptions.RemoveEmptyEntries"/> and <paramref name="source"/> is empty,
    /// or if <paramref name="options"/> specifies both <see cref="StringSplitOptions.RemoveEmptyEntries"/> and <see cref="StringSplitOptions.TrimEntries"/> and the <paramref name="source"/> is
    /// entirely whitespace, no ranges are written to the destination.
    /// </para>
    /// <para>
    /// If the span does not contain <paramref name="separator"/>, or if <paramref name="destination"/>'s length is 1, a single range will be output containing the entire <paramref name="source"/>,
    /// subject to the processing implied by <paramref name="options"/>.
    /// </para>
    /// <para>
    /// If there are more regions in <paramref name="source"/> than will fit in <paramref name="destination"/>, the first <paramref name="destination"/> length minus 1 ranges are
    /// stored in <paramref name="destination"/>, and a range for the remainder of <paramref name="source"/> is stored in <paramref name="destination"/>.
    /// </para>
    /// <para>
    /// This is a polyfill provided by Gapotchenko.FX.
    /// </para>
    /// </remarks>
#if TFF_READONLYSPAN_CHAR_SPLIT
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public static int Split(
#if !TFF_READONLYSPAN_CHAR_SPLIT
        this
#endif
        ReadOnlySpan<char> source,
        Span<Range> destination,
        char separator,
        StringSplitOptions options = StringSplitOptions.None)
    {
#if TFF_READONLYSPAN_CHAR_SPLIT
        return source.Split(destination, separator, options);
#else
        ValidateStringSplitOptions(options);

        Span<char> separatorSpan = stackalloc char[1];
        separatorSpan[0] = separator;

        return SplitCore(source, destination, separatorSpan, default, isAny: true, options);
#endif
    }

    /// <summary>
    /// Parses the source <see cref="ReadOnlySpan{Char}"/> for the specified <paramref name="separator"/>, populating the <paramref name="destination"/> span
    /// with <see cref="Range"/> instances representing the regions between the separators.
    /// </summary>
    /// <param name="source">The source span to parse.</param>
    /// <param name="destination">The destination span into which the resulting ranges are written.</param>
    /// <param name="separator">A character that delimits the regions in this instance.</param>
    /// <param name="options">A bitwise combination of the enumeration values that specifies whether to trim whitespace and include empty ranges.</param>
    /// <returns>The number of ranges written into <paramref name="destination"/>.</returns>
    /// <remarks>
    /// <para>
    /// Delimiter characters are not included in the elements of the returned array.
    /// </para>
    /// <para>
    /// If the <paramref name="destination"/> span is empty, or if the <paramref name="options"/> specifies <see cref="StringSplitOptions.RemoveEmptyEntries"/> and <paramref name="source"/> is empty,
    /// or if <paramref name="options"/> specifies both <see cref="StringSplitOptions.RemoveEmptyEntries"/> and <see cref="StringSplitOptions.TrimEntries"/> and the <paramref name="source"/> is
    /// entirely whitespace, no ranges are written to the destination.
    /// </para>
    /// <para>
    /// If the span does not contain <paramref name="separator"/>, or if <paramref name="destination"/>'s length is 1, a single range will be output containing the entire <paramref name="source"/>,
    /// subject to the processing implied by <paramref name="options"/>.
    /// </para>
    /// <para>
    /// If there are more regions in <paramref name="source"/> than will fit in <paramref name="destination"/>, the first <paramref name="destination"/> length minus 1 ranges are
    /// stored in <paramref name="destination"/>, and a range for the remainder of <paramref name="source"/> is stored in <paramref name="destination"/>.
    /// </para>
    /// <para>
    /// This is a polyfill provided by Gapotchenko.FX.
    /// </para>
    /// </remarks>
#if TFF_READONLYSPAN_CHAR_SPLIT
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public static int Split(
#if !TFF_READONLYSPAN_CHAR_SPLIT
        this
#endif
        ReadOnlySpan<char> source,
        Span<Range> destination,
        ReadOnlySpan<char> separator,
        StringSplitOptions options = StringSplitOptions.None)
    {
#if TFF_READONLYSPAN_CHAR_SPLIT
        return source.Split(destination, separator, options);
#else
        ValidateStringSplitOptions(options);

        // If the separator is an empty string, the whole input is considered the sole range.
        if (separator.IsEmpty)
        {
            if (!destination.IsEmpty)
            {
                int startInclusive = 0, endExclusive = source.Length;

#if TFF_STRINGSPLITOPTIONS_TRIMENTRIES
                if ((options & StringSplitOptions.TrimEntries) != 0)
                {
                    (startInclusive, endExclusive) = TrimSplitEntry(source, startInclusive, endExclusive);
                }
#endif

                if (startInclusive != endExclusive || (options & StringSplitOptions.RemoveEmptyEntries) == 0)
                {
                    destination[0] = startInclusive..endExclusive;
                    return 1;
                }
            }

            return 0;
        }

        return SplitCore(source, destination, separator, default, isAny: false, options);
#endif
    }

    /// <summary>
    /// Parses the source <see cref="ReadOnlySpan{Char}"/> for one of the specified <paramref name="separators"/>, populating the <paramref name="destination"/> span
    /// with <see cref="Range"/> instances representing the regions between the separators.
    /// </summary>
    /// <param name="source">The source span to parse.</param>
    /// <param name="destination">The destination span into which the resulting ranges are written.</param>
    /// <param name="separators">Any number of characters that may delimit the regions in this instance. If empty, all Unicode whitespace characters are used as the separators.</param>
    /// <param name="options">A bitwise combination of the enumeration values that specifies whether to trim whitespace and include empty ranges.</param>
    /// <returns>The number of ranges written into <paramref name="destination"/>.</returns>
    /// <remarks>
    /// <para>
    /// Delimiter characters are not included in the elements of the returned array.
    /// </para>
    /// <para>
    /// If the <paramref name="destination"/> span is empty, or if the <paramref name="options"/> specifies <see cref="StringSplitOptions.RemoveEmptyEntries"/> and <paramref name="source"/> is empty,
    /// or if <paramref name="options"/> specifies both <see cref="StringSplitOptions.RemoveEmptyEntries"/> and <see cref="StringSplitOptions.TrimEntries"/> and the <paramref name="source"/> is
    /// entirely whitespace, no ranges are written to the destination.
    /// </para>
    /// <para>
    /// If the span does not contain any of the <paramref name="separators"/>, or if <paramref name="destination"/>'s length is 1, a single range will be output containing the entire <paramref name="source"/>,
    /// subject to the processing implied by <paramref name="options"/>.
    /// </para>
    /// <para>
    /// If there are more regions in <paramref name="source"/> than will fit in <paramref name="destination"/>, the first <paramref name="destination"/> length minus 1 ranges are
    /// stored in <paramref name="destination"/>, and a range for the remainder of <paramref name="source"/> is stored in <paramref name="destination"/>.
    /// </para>
    /// <para>
    /// This is a polyfill provided by Gapotchenko.FX.
    /// </para>
    /// </remarks>
#if TFF_READONLYSPAN_CHAR_SPLIT
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public static int SplitAny(
#if !TFF_READONLYSPAN_CHAR_SPLIT
        this
#endif
        ReadOnlySpan<char> source,
        Span<Range> destination,
        ReadOnlySpan<char> separators,
        StringSplitOptions options = StringSplitOptions.None)
    {
#if TFF_READONLYSPAN_CHAR_SPLIT
        return source.SplitAny(destination, separators, options);
#else
        ValidateStringSplitOptions(options);

#if TFF_STRINGSPLITOPTIONS_TRIMENTRIES
        // If the separators list is empty, whitespace is used as separators.
        // In that case, we want to ignore TrimEntries if specified, since TrimEntries also impacts whitespace.
        // The TrimEntries flag must be left intact if we are constrained by count because we need to process last substring.
        if (separators.IsEmpty && destination.Length > source.Length)
            options &= ~StringSplitOptions.TrimEntries;
#endif

        return SplitCore(source, destination, separators, default, isAny: true, options);
#endif
    }

    /// <summary>
    /// Parses the source <see cref="ReadOnlySpan{Char}"/> for one of the specified <paramref name="separators"/>, populating the <paramref name="destination"/> span
    /// with <see cref="Range"/> instances representing the regions between the separators.
    /// </summary>
    /// <param name="source">The source span to parse.</param>
    /// <param name="destination">The destination span into which the resulting ranges are written.</param>
    /// <param name="separators">Any number of strings that may delimit the regions in this instance.  If empty, all Unicode whitespace characters are used as the separators.</param>
    /// <param name="options">A bitwise combination of the enumeration values that specifies whether to trim whitespace and include empty ranges.</param>
    /// <returns>The number of ranges written into <paramref name="destination"/>.</returns>
    /// <remarks>
    /// <para>
    /// Delimiter characters are not included in the elements of the returned array.
    /// </para>
    /// <para>
    /// If the <paramref name="destination"/> span is empty, or if the <paramref name="options"/> specifies <see cref="StringSplitOptions.RemoveEmptyEntries"/> and <paramref name="source"/> is empty,
    /// or if <paramref name="options"/> specifies both <see cref="StringSplitOptions.RemoveEmptyEntries"/> and <see cref="StringSplitOptions.TrimEntries"/> and the <paramref name="source"/> is
    /// entirely whitespace, no ranges are written to the destination.
    /// </para>
    /// <para>
    /// If the span does not contain any of the <paramref name="separators"/>, or if <paramref name="destination"/>'s length is 1, a single range will be output containing the entire <paramref name="source"/>,
    /// subject to the processing implied by <paramref name="options"/>.
    /// </para>
    /// <para>
    /// If there are more regions in <paramref name="source"/> than will fit in <paramref name="destination"/>, the first <paramref name="destination"/> length minus 1 ranges are
    /// stored in <paramref name="destination"/>, and a range for the remainder of <paramref name="source"/> is stored in <paramref name="destination"/>.
    /// </para>
    /// <para>
    /// This is a polyfill provided by Gapotchenko.FX.
    /// </para>
    /// </remarks>
#if TFF_READONLYSPAN_CHAR_SPLIT
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public static int SplitAny(
#if !TFF_READONLYSPAN_CHAR_SPLIT
        this
#endif
        ReadOnlySpan<char> source,
        Span<Range> destination,
        ReadOnlySpan<string> separators,
        StringSplitOptions options = StringSplitOptions.None)
    {
#if TFF_READONLYSPAN_CHAR_SPLIT
        return source.SplitAny(destination, separators, options);
#else
        ValidateStringSplitOptions(options);

#if TFF_STRINGSPLITOPTIONS_TRIMENTRIES
        // If the separators list is empty, whitespace is used as separators.
        // In that case, we want to ignore TrimEntries if specified, since TrimEntries also impacts whitespace.
        // The TrimEntries flag must be left intact if we are constrained by count because we need to process last substring.
        if (separators.IsEmpty && destination.Length > source.Length)
            options &= ~StringSplitOptions.TrimEntries;
#endif

        return SplitCore(source, destination, default, separators!, isAny: true, options);
#endif
    }

#if !TFF_READONLYSPAN_CHAR_SPLIT
    static void ValidateStringSplitOptions(StringSplitOptions options)
    {
        const StringSplitOptions AllValidFlags =
            StringSplitOptions.RemoveEmptyEntries
#if NET
            | StringSplitOptions.TrimEntries
#endif
            ;

        if ((options & ~AllValidFlags) != 0)
            throw new ArgumentException("String split options contains an invalid flag.", nameof(options));
    }

    static int SplitCore(
        ReadOnlySpan<char> source, Span<Range> destination,
        ReadOnlySpan<char> separatorOrSeparators, ReadOnlySpan<string?> stringSeparators, bool isAny,
        StringSplitOptions options)
    {
        // If the destination is empty, there's nothing to do.
        if (destination.IsEmpty)
        {
            return 0;
        }

        bool keepEmptyEntries = (options & StringSplitOptions.RemoveEmptyEntries) == 0;
        bool trimEntries =
#if TFF_STRINGSPLITOPTIONS_TRIMENTRIES
            (options & StringSplitOptions.TrimEntries) != 0;
#else
            false;
#endif

        // If the input is empty, then we either return an empty range as the sole range, or if empty entries
        // are to be removed, we return nothing.
        if (source.Length == 0)
        {
            if (keepEmptyEntries)
            {
                destination[0] = default;
                return 1;
            }

            return 0;
        }

        int startInclusive = 0, endExclusive;

        // If the destination has only one slot, then we need to return the whole input, subject to the options.
        if (destination.Length == 1)
        {
            endExclusive = source.Length;
            if (trimEntries)
            {
                (startInclusive, endExclusive) = TrimSplitEntry(source, startInclusive, endExclusive);
            }

            if (startInclusive != endExclusive || keepEmptyEntries)
            {
                destination[0] = startInclusive..endExclusive;
                return 1;
            }

            return 0;
        }

        scoped ValueListBuilder<int> separatorList = new ValueListBuilder<int>(stackalloc int[StringStackallocIntBufferSizeLimit]);
        scoped ValueListBuilder<int> lengthList = default;

        int separatorLength;
        int rangeCount = 0;
        if (!stringSeparators.IsEmpty)
        {
            lengthList = new ValueListBuilder<int>(stackalloc int[StringStackallocIntBufferSizeLimit]);
            MakeSeparatorListAny(source, stringSeparators, ref separatorList, ref lengthList);
            separatorLength = -1; // Will be set on each iteration of the loop
        }
        else if (isAny)
        {
            MakeSeparatorListAny(source, separatorOrSeparators, ref separatorList);
            separatorLength = 1;
        }
        else
        {
            MakeSeparatorList(source, separatorOrSeparators, ref separatorList);
            separatorLength = separatorOrSeparators.Length;
        }

        // Try to fill in all but the last slot in the destination.  The last slot is reserved for whatever remains
        // after the last discovered separator. If the options specify that empty entries are to be removed, then we
        // need to skip past all of those here as well, including any that occur at the beginning of the last entry,
        // which is why we enter the loop if remove empty entries is set, even if we've already added enough entries.
        int separatorIndex = 0;
        Span<Range> destinationMinusOne = destination.Slice(0, destination.Length - 1);
        while (separatorIndex < separatorList.Length && (rangeCount < destinationMinusOne.Length || !keepEmptyEntries))
        {
            endExclusive = separatorList[separatorIndex];
            if (separatorIndex < lengthList.Length)
            {
                separatorLength = lengthList[separatorIndex];
            }
            separatorIndex++;

            // Trim off whitespace from the start and end of the range.
            int untrimmedEndEclusive = endExclusive;
            if (trimEntries)
            {
                (startInclusive, endExclusive) = TrimSplitEntry(source, startInclusive, endExclusive);
            }

            // If the range is not empty or we're not ignoring empty ranges, store it.
            Debug.Assert(startInclusive <= endExclusive);
            if (startInclusive != endExclusive || keepEmptyEntries)
            {
                // If we're not keeping empty entries, we may have entered the loop even if we'd
                // already written enough ranges.  Now that we know this entry isn't empty, we
                // need to validate there's still room remaining.
                if ((uint)rangeCount >= (uint)destinationMinusOne.Length)
                {
                    break;
                }

                destinationMinusOne[rangeCount] = startInclusive..endExclusive;
                rangeCount++;
            }

            // Reset to be just past the separator, and loop around to go again.
            startInclusive = untrimmedEndEclusive + separatorLength;
        }

        separatorList.Dispose();
        lengthList.Dispose();

        // Either we found at least destination.Length - 1 ranges or we didn't find any more separators.
        // If we still have a last destination slot available and there's anything left in the source,
        // put a range for the remainder of the source into the destination.
        if ((uint)rangeCount < (uint)destination.Length)
        {
            endExclusive = source.Length;
            if (trimEntries)
            {
                (startInclusive, endExclusive) = TrimSplitEntry(source, startInclusive, endExclusive);
            }

            if (startInclusive != endExclusive || keepEmptyEntries)
            {
                destination[rangeCount] = startInclusive..endExclusive;
                rangeCount++;
            }
        }

        // Return how many ranges were written.
        return rangeCount;
    }

    /// <summary>Updates the starting and ending markers for a range to exclude whitespace.</summary>
    static (int StartInclusive, int EndExclusive) TrimSplitEntry(ReadOnlySpan<char> source, int startInclusive, int endExclusive)
    {
        while (startInclusive < endExclusive && char.IsWhiteSpace(source[startInclusive]))
        {
            startInclusive++;
        }

        while (endExclusive > startInclusive && char.IsWhiteSpace(source[endExclusive - 1]))
        {
            endExclusive--;
        }

        return (startInclusive, endExclusive);
    }

    /// <summary>
    /// Uses ValueListBuilder to create list that holds indexes of separators in string.
    /// </summary>
    /// <param name="source">The source to parse.</param>
    /// <param name="separators"><see cref="ReadOnlySpan{T}"/> of separator chars</param>
    /// <param name="sepListBuilder"><see cref="ValueListBuilder{T}"/> to store indexes</param>
    static void MakeSeparatorListAny(ReadOnlySpan<char> source, ReadOnlySpan<char> separators, ref ValueListBuilder<int> sepListBuilder)
    {
        // Special-case no separators to mean any whitespace is a separator.
        if (separators.Length == 0)
        {
            for (int i = 0; i < source.Length; i++)
            {
                if (char.IsWhiteSpace(source[i]))
                {
                    sepListBuilder.Append(i);
                }
            }
        }

        // Special-case the common cases of 1, 2, and 3 separators, with manual comparisons against each separator.
        else if (separators.Length <= 3)
        {
            char sep0, sep1, sep2;
            sep0 = separators[0];
            sep1 = separators.Length > 1 ? separators[1] : sep0;
            sep2 = separators.Length > 2 ? separators[2] : sep1;

            for (int i = 0; i < source.Length; i++)
            {
                char c = source[i];
                if (c == sep0 || c == sep1 || c == sep2)
                {
                    sepListBuilder.Append(i);
                }
            }
        }

        // Handle > 3 separators.
        else
        {
            var sepSet = new HashSet<char>();
            foreach (var sep in separators)
                sepSet.Add(sep);

            for (int i = 0; i < source.Length; i++)
            {
                if (sepSet.Contains(source[i]))
                {
                    sepListBuilder.Append(i);
                }
            }
        }
    }

    /// <summary>
    /// Uses ValueListBuilder to create list that holds indexes of separators in string.
    /// </summary>
    /// <param name="source">The source to parse.</param>
    /// <param name="separator">separator string</param>
    /// <param name="sepListBuilder"><see cref="ValueListBuilder{T}"/> to store indexes</param>
    static void MakeSeparatorList(ReadOnlySpan<char> source, ReadOnlySpan<char> separator, ref ValueListBuilder<int> sepListBuilder)
    {
        Debug.Assert(!separator.IsEmpty, "Empty separator");

        int i = 0;
        while (!source.IsEmpty)
        {
            int index = source.IndexOf(separator);
            if (index < 0)
            {
                break;
            }

            i += index;
            sepListBuilder.Append(i);

            i += separator.Length;
            source = source.Slice(index + separator.Length);
        }
    }

    /// <summary>
    /// Uses ValueListBuilder to create list that holds indexes of separators in string and list that holds length of separator strings.
    /// </summary>
    /// <param name="source">The source to parse.</param>
    /// <param name="separators">separator strngs</param>
    /// <param name="sepListBuilder"><see cref="ValueListBuilder{T}"/> for separator indexes</param>
    /// <param name="lengthListBuilder"><see cref="ValueListBuilder{T}"/> for separator length values</param>
    static void MakeSeparatorListAny(ReadOnlySpan<char> source, ReadOnlySpan<string?> separators, ref ValueListBuilder<int> sepListBuilder, ref ValueListBuilder<int> lengthListBuilder)
    {
        Debug.Assert(!separators.IsEmpty, "Zero separators");

        for (int i = 0; i < source.Length; i++)
        {
            for (int j = 0; j < separators.Length; j++)
            {
                string? separator = separators[j];
                if (string.IsNullOrEmpty(separator))
                {
                    continue;
                }
                int currentSepLength = separator.Length;
                if (source[i] == separator[0] && currentSepLength <= source.Length - i)
                {
                    if (currentSepLength == 1 || source.Slice(i, currentSepLength).SequenceEqual(separator.AsSpan()))
                    {
                        sepListBuilder.Append(i);
                        lengthListBuilder.Append(currentSepLength);
                        i += currentSepLength - 1;
                        break;
                    }
                }
            }
        }
    }

    const int StringStackallocIntBufferSizeLimit = 128;

    ref struct ValueListBuilder<T>
    {
        Span<T> _span;
        T[]? _arrayFromPool;
        int _pos;

        public ValueListBuilder(Span<T> initialSpan)
        {
            _span = initialSpan;
            _arrayFromPool = null;
            _pos = 0;
        }

        public int Length
        {
            get => _pos;
            set
            {
                Debug.Assert(value >= 0);
                Debug.Assert(value <= _span.Length);
                _pos = value;
            }
        }

        public ref T this[int index]
        {
            get
            {
                Debug.Assert(index < _pos);
                return ref _span[index];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(T item)
        {
            int pos = _pos;

            // Workaround for https://github.com/dotnet/runtime/issues/72004
            Span<T> span = _span;
            if ((uint)pos < (uint)span.Length)
            {
                span[pos] = item;
                _pos = pos + 1;
            }
            else
            {
                AddWithResize(item);
            }
        }

        // Hide uncommon path
        [MethodImpl(MethodImplOptions.NoInlining)]
        void AddWithResize(T item)
        {
            Debug.Assert(_pos == _span.Length);
            int pos = _pos;
            Grow();
            _span[pos] = item;
            _pos = pos + 1;
        }

        public ReadOnlySpan<T> AsSpan()
        {
            return _span.Slice(0, _pos);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            T[]? toReturn = _arrayFromPool;
            if (toReturn != null)
            {
                _arrayFromPool = null;
                ArrayPool<T>.Shared.Return(toReturn);
            }
        }

        void Grow()
        {
            // Double the size of the span.  If it's currently empty, default to size 4,
            // although it'll be increased in Rent to the pool's minimum bucket size.
            int nextCapacity = _span.Length != 0 ? _span.Length * 2 : 4;

            // If the computed doubled capacity exceeds the possible length of an array, then we
            // want to downgrade to either the maximum array length if that's large enough to hold
            // an additional item, or the current length + 1 if it's larger than the max length, in
            // which case it'll result in an OOM when calling Rent below.  In the exceedingly rare
            // case where _span.Length is already int.MaxValue (in which case it couldn't be a managed
            // array), just use that same value again and let it OOM in Rent as well.
            int arrayMaxLength = Array.MaxLength;
            if ((uint)nextCapacity > arrayMaxLength)
            {
                nextCapacity = Math.Max(Math.Max(_span.Length + 1, arrayMaxLength), _span.Length);
            }

            T[] array = ArrayPool<T>.Shared.Rent(nextCapacity);
            _span.CopyTo(array);

            T[]? toReturn = _arrayFromPool;
            _span = _arrayFromPool = array;
            if (toReturn != null)
            {
                ArrayPool<T>.Shared.Return(toReturn);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Pop()
        {
            _pos--;
            return _span[_pos];
        }
    }
#endif
}
