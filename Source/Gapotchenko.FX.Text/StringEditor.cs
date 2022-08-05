using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace Gapotchenko.FX.Text;

/// <summary>
/// Provides operations for bulk string editing.
/// </summary>
public sealed class StringEditor
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StringEditor"/> class.
    /// </summary>
    /// <param name="value">The string to edit.</param>
    public StringEditor(string value)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        m_Value = value;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    string m_Value;

    sealed class Operation
    {
        public Operation(StringSpan span, string newValue)
        {
            Span = span;
            NewValue = newValue;
        }

        public readonly StringSpan Span;

        public string NewValue;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    List<Operation> m_Operations = new List<Operation>();

    /// <summary>
    /// Validates a string span.
    /// </summary>
    /// <param name="startIndex">The zero-based start index of the span.</param>
    /// <param name="length">The span length.</param>
    /// <param name="size">The string size.</param>
    static void ValidateSpan(int startIndex, int length, int size)
    {
        if (startIndex < 0)
            throw new ArgumentOutOfRangeException(nameof(startIndex), "Span start index cannot be less than zero.");
        if (length < 0)
            throw new ArgumentOutOfRangeException(nameof(length), "Span length cannot be less than zero.");
        if (length > size - startIndex)
            throw new ArgumentOutOfRangeException(nameof(length), "Span start index and length must refer to a location within the string.");
    }

    /// <summary>
    /// Validates a string span.
    /// </summary>
    /// <param name="span">The span.</param>
    /// <param name="size">The string size.</param>
    static void ValidateSpan(StringSpan span, int size)
    {
        if (span.StartIndex < 0)
            throw new ArgumentOutOfRangeException(nameof(span), "Span start index cannot be less than zero.");
        if (span.Length < 0)
            throw new ArgumentOutOfRangeException(nameof(span), "Span length cannot be less than zero.");
        if (span.Length > size - span.StartIndex)
            throw new ArgumentOutOfRangeException(nameof(span), "Span start index and length must refer to a location within the string.");
    }

    /// <summary>
    /// Validates a regular expression capture.
    /// </summary>
    /// <param name="capture">The regular expression capture.</param>
    /// <param name="size">The string size.</param>
    static void ValidateCapture(Capture capture, int size)
    {
        if (capture.Index < 0)
            throw new ArgumentOutOfRangeException(nameof(capture), "Capture index cannot be less than zero.");
        if (capture.Length < 0)
            throw new ArgumentOutOfRangeException(nameof(capture), "Capture length cannot be less than zero.");
        if (capture.Length > size - capture.Index)
            throw new ArgumentOutOfRangeException(nameof(capture), "Capture index and length must refer to a location within the string.");
    }

    /// <summary>
    /// Accumulates an insertion operation at a specified index position.
    /// </summary>
    /// <param name="startIndex">The zero-based index position of the insertion.</param>
    /// <param name="value">The string to insert.</param>
    public void Insert(int startIndex, string value)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        if (startIndex < 0 || startIndex > m_Value.Length)
            throw new ArgumentOutOfRangeException(nameof(startIndex));

        ReplaceCore(new StringSpan(startIndex, 0), value);
    }

    /// <summary>
    /// Accumulates a replacement operation for a specified string span.
    /// </summary>
    /// <param name="span">The string span to replace.</param>
    /// <param name="newValue">The string to use as a replacement.</param>
    public void Replace(StringSpan span, string newValue)
    {
        if (newValue == null)
            throw new ArgumentNullException(nameof(newValue));

        ValidateSpan(span, m_Value.Length);

        ReplaceCore(span, newValue);
    }

    void ReplaceCore(StringSpan span, string newValue)
    {
        Operation? compatibleOperation = null;
        foreach (var i in m_Operations)
        {
            if (i.Span == span)
            {
                compatibleOperation = i;
                break;
            }
        }

        if (compatibleOperation != null)
            compatibleOperation.NewValue = newValue;
        else
            m_Operations.Add(new Operation(span, newValue));
    }

    /// <summary>
    /// Accumulates a replacement operation for a specified string span.
    /// </summary>
    /// <param name="startIndex">The zero-based start index of the string span to replace.</param>
    /// <param name="length">The length of the string span to replace.</param>
    /// <param name="newValue">The string to use as a replacement.</param>
    public void Replace(int startIndex, int length, string newValue)
    {
        if (newValue == null)
            throw new ArgumentNullException(nameof(newValue));

        ValidateSpan(startIndex, length, m_Value.Length);

        ReplaceCore(new StringSpan(startIndex, length), newValue);
    }

    /// <summary>
    /// Accumulates a replacement operation for a specified regular expression capture.
    /// </summary>
    /// <param name="capture">The regular expression capture to replace.</param>
    /// <param name="newValue">The string to use as a replacement.</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Replace(Capture capture, string newValue)
    {
        if (capture == null)
            throw new ArgumentNullException(nameof(capture));
        if (newValue == null)
            throw new ArgumentNullException(nameof(newValue));

        ValidateCapture(capture, m_Value.Length);

        if (!newValue.Equals(capture.Value, StringComparison.Ordinal))
            ReplaceCore(capture, newValue);
    }

    /// <summary>
    /// Accumulates a removal operation for a specified string span.
    /// </summary>
    /// <param name="span">The string span to remove.</param>
    public void Remove(StringSpan span) => Replace(span, string.Empty);

    /// <summary>
    /// Accumulates a removal operation for a specified string span.
    /// </summary>
    /// <param name="startIndex">The zero-based start index of the string span to remove.</param>
    /// <param name="length">The length of the string span to remove.</param>
    public void Remove(int startIndex, int length) => Replace(startIndex, length, string.Empty);

    /// <summary>
    /// Replaces a specified string span with a new value.
    /// </summary>
    /// <param name="s">The string to edit.</param>
    /// <param name="span">The string span to replace.</param>
    /// <param name="newValue">The string to use as a replacement.</param>
    public static string Replace(string s, StringSpan span, string newValue)
    {
        if (s == null)
            throw new ArgumentNullException(nameof(s));
        if (newValue == null)
            throw new ArgumentNullException(nameof(newValue));

        ValidateSpan(span, s.Length);

        return _ReplaceCore(s, span, newValue);
    }

    /// <summary>
    /// Replaces a specified string span with a new value.
    /// </summary>
    /// <param name="s">The string to edit.</param>
    /// <param name="startIndex">The zero-based start index of the string span to replace.</param>
    /// <param name="length">The length of the string span to replace.</param>
    /// <param name="newValue">The string to use as a replacement.</param>
    public static string Replace(string s, int startIndex, int length, string newValue)
    {
        if (s == null)
            throw new ArgumentNullException(nameof(s));
        if (newValue == null)
            throw new ArgumentNullException(nameof(newValue));

        ValidateSpan(startIndex, length, s.Length);

        return _ReplaceCore(s, new StringSpan(startIndex, length), newValue);
    }

    static string _ReplaceCore(string s, StringSpan span, string newValue)
    {
        int index = span.StartIndex;
        int length = span.Length;

        if (length != 0)
            s = s.Remove(index, length);

        if (newValue.Length != 0)
            s = s.Insert(index, newValue);

        return s;
    }

    /// <summary>
    /// Replaces a specified regular expression capture with a new string.
    /// </summary>
    /// <param name="s">The string to edit.</param>
    /// <param name="capture">The regular expression capture to replace.</param>
    /// <param name="newValue">The string to use as a replacement.</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static string Replace(string s, Capture capture, string newValue)
    {
        if (s == null)
            throw new ArgumentNullException(nameof(s));
        if (capture == null)
            throw new ArgumentNullException(nameof(capture));
        if (newValue == null)
            throw new ArgumentNullException(nameof(newValue));

        ValidateCapture(capture, s.Length);

        if (newValue.Equals(capture.Value, StringComparison.Ordinal))
            return s;
        else
            return _ReplaceCore(s, capture, newValue);
    }

    /// <summary>
    /// Returns a new string in which a specified span has been deleted.
    /// </summary>
    /// <param name="s">The string to edit.</param>
    /// <param name="span">The string span to remove.</param>
    /// <returns>A new string that is equivalent to <paramref name="s"/> except for the removed characters.</returns>
    public static string Remove(string s, StringSpan span)
    {
        if (s == null)
            throw new ArgumentNullException(nameof(s));

        ValidateSpan(span, s.Length);

        if (span.Length == 0)
            return s;
        else
            return s.Remove(span.StartIndex, span.Length);
    }

    /// <summary>
    /// Applies accumulated editing operations to a string value.
    /// </summary>
    /// <param name="s">The string value.</param>
    /// <returns>The edited string value.</returns>
    string ApplyOperations(string s)
    {
        if (s == null)
            throw new ArgumentNullException(nameof(s));

        if (m_Operations.Count == 0)
            return s;

        var sb = new StringBuilder(s);

        int offset = 0;
        foreach (var group in m_Operations.OrderBy(x => x.Span.StartIndex))
        {
            int spanIndex = group.Span.StartIndex + offset;
            int spanLength = group.Span.Length;
            string newValue = group.NewValue;

            sb.Remove(spanIndex, spanLength).Insert(spanIndex, newValue);

            offset += newValue.Length - spanLength;
        }

        return sb.ToString();
    }

    /// <summary>
    /// Clears accumulated editing operations, and sets a new string value for editing.
    /// </summary>
    /// <param name="value">The string to edit.</param>
    public void Reset(string value)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        m_Operations.Clear();
        m_Value = value;
    }

    /// <summary>
    /// Applies accumulated editing operations and forms the resulting string.
    /// </summary>
    /// <returns>The resulting string value.</returns>
    public override string ToString() => ApplyOperations(m_Value);
}
