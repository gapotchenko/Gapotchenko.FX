using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Gapotchenko.FX;

/// <summary>
/// Represents a mutable string of name/value pairs for the query part of an URI.
/// It can be used separately or in conjunction with <see cref="UriBuilder"/> class.
/// </summary>
public sealed class UriQueryBuilder
{
    /// <summary>
    /// Provides a character used to separate a query within a URI.
    /// </summary>
    public const char QuerySeparator = '?';

    /// <summary>
    /// Provides a character used to separate parameters in a query.
    /// </summary>
    public const char ParameterSeparator = '&';

    /// <summary>
    /// Initializes a new instance of the <see cref="UriQueryBuilder"/> class.
    /// </summary>
    public UriQueryBuilder()
    {
        m_SB = new StringBuilder();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UriQueryBuilder"/> class using the specified string.
    /// </summary>
    /// <param name="query">The query.</param>
    public UriQueryBuilder(string? query)
    {
        if (string.IsNullOrEmpty(query))
        {
            m_SB = new StringBuilder();
        }
        else
        {
            if (query[0] == QuerySeparator)
            {
                // UriBuilder.Query property getter appends '?' on return result.
                // Trim it to avoid double query separators in resulting URI.
                m_SB = new StringBuilder(query, 1, query.Length - 1, 0);
            }
            else
            {
                m_SB = new StringBuilder(query);
            }
        }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    readonly StringBuilder m_SB;

    /// <summary>
    /// Appends a query parameter to this instance.
    /// </summary>
    /// <param name="name">The parameter name.</param>
    /// <param name="value">The parameter value.</param>
    /// <returns>The <see cref="UriQueryBuilder"/> instance.</returns>
    public UriQueryBuilder AppendParameter(string name, string? value)
    {
        if (name == null)
            throw new ArgumentNullException(nameof(name));
        if (name.Length == 0)
            throw new ArgumentException("A query parameter name cannot be empty.", nameof(name));

        var sb = m_SB;

        if (sb.Length != 0)
            sb.Append(ParameterSeparator);

        sb.Append(Uri.EscapeDataString(name)).Append('=');
        if (value != null)
            sb.Append(Uri.EscapeDataString(value));

        return this;
    }

    /// <summary>
    /// Appends a query parameter to the given URI.
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <param name="name">The parameter name.</param>
    /// <param name="value">The parameter value.</param>
    /// <returns>The URI with an appended query parameter.</returns>
    public static string AppendParameter(string? uri, string name, string? value) => new UriQueryBuilder().AppendParameter(name, value).CombineWithUri(uri);

    /// <summary>
    /// Appends specified query parameters to the given URI.
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <param name="name1">The name of the first parameter.</param>
    /// <param name="value1">The value of the first parameter.</param>
    /// <param name="name2">The name of the second parameter.</param>
    /// <param name="value2">The value of the second parameter.</param>
    /// <returns>The URI with an appended query parameter.</returns>
    public static string AppendParameter(string? uri, string name1, string? value1, string name2, string? value2) =>
        new UriQueryBuilder()
            .AppendParameter(name1, value1)
            .AppendParameter(name2, value2)
            .CombineWithUri(uri);

    /// <summary>
    /// Appends specified query parameters to the given URI.
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <param name="name1">The name of the first parameter.</param>
    /// <param name="value1">The value of the first parameter.</param>
    /// <param name="name2">The name of the second parameter.</param>
    /// <param name="value2">The value of the second parameter.</param>
    /// <param name="name3">The name of the third parameter.</param>
    /// <param name="value3">The value of the third parameter.</param>
    /// <returns>The URI with an appended query parameter.</returns>
    public static string AppendParameter(string? uri, string name1, string? value1, string name2, string? value2, string name3, string? value3) =>
        new UriQueryBuilder()
            .AppendParameter(name1, value1)
            .AppendParameter(name2, value2)
            .AppendParameter(name3, value3)
            .CombineWithUri(uri);

    /// <summary>
    /// Appends a query parameter to the given <see cref="Uri"/>.
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <param name="name">The parameter name.</param>
    /// <param name="value">The parameter value.</param>
    /// <returns>The <see cref="Uri"/> with an appended query parameter.</returns>
    public static Uri AppendParameter(Uri uri, string name, string? value)
    {
        if (uri == null)
            throw new ArgumentNullException(nameof(uri));

        return new UriQueryBuilder().AppendParameter(name, value).CombineWithUri(uri);
    }

    /// <summary>
    /// Appends specified query parameters to the given URI.
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <param name="name1">The name of the first parameter.</param>
    /// <param name="value1">The value of the first parameter.</param>
    /// <param name="name2">The name of the second parameter.</param>
    /// <param name="value2">The value of the second parameter.</param>
    /// <returns>The URI with an appended query parameter.</returns>
    public static Uri AppendParameter(Uri uri, string name1, string? value1, string name2, string? value2)
    {
        if (uri == null)
            throw new ArgumentNullException(nameof(uri));

        return new UriQueryBuilder()
            .AppendParameter(name1, value1)
            .AppendParameter(name2, value2)
            .CombineWithUri(uri);
    }

    /// <summary>
    /// Appends specified query parameters to the given URI.
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <param name="name1">The name of the first parameter.</param>
    /// <param name="value1">The value of the first parameter.</param>
    /// <param name="name2">The name of the second parameter.</param>
    /// <param name="value2">The value of the second parameter.</param>
    /// <param name="name3">The name of the third parameter.</param>
    /// <param name="value3">The value of the third parameter.</param>
    /// <returns>The URI with an appended query parameter.</returns>
    public static Uri AppendParameter(Uri uri, string name1, string? value1, string name2, string? value2, string name3, string? value3)
    {
        if (uri == null)
            throw new ArgumentNullException(nameof(uri));

        return new UriQueryBuilder()
            .AppendParameter(name1, value1)
            .AppendParameter(name2, value2)
            .AppendParameter(name3, value3)
            .CombineWithUri(uri);
    }

    /// <summary>
    /// Checks whether a query in this instance has a parameter with the given name.
    /// </summary>
    /// <param name="name">The parameter name.</param>
    /// <returns><see langword="true"/> if a query has a parameter with the given name; otherwise, <see langword="false"/>.</returns>
    public bool HasParameter(string name)
    {
        if (name == null)
            throw new ArgumentNullException(nameof(name));

        string token = name + '=';
        string query = ToString();

        return
            query.StartsWith(token, StringComparison.Ordinal) ||
            query.IndexOf(ParameterSeparator + token, StringComparison.Ordinal) != -1;
    }

    /// <summary>
    /// Removes all the characters from the query string in this instance.
    /// </summary>
    /// <returns>The <see cref="UriQueryBuilder"/> instance.</returns>
    public UriQueryBuilder Clear()
    {
        m_SB.Clear();
        return this;
    }

    /// <summary>
    /// Gets the length of a query string in this instance.
    /// </summary>
    public int Length => m_SB.Length;

    /// <summary>
    /// Converts the value of this instance to a <see cref="String"/>.
    /// </summary>
    /// <returns>A string whose value is the same as this instance.</returns>
    public override string ToString() => m_SB.ToString();

    /// <summary>
    /// Converts the value of this instance to a <see cref="String"/> and combines it with the given URI.
    /// </summary>
    /// <param name="uri">The URI to combine with.</param>
    /// <returns>The combined URI.</returns>
    public string CombineWithUri(string? uri) => CombineWithUri(uri, ToString());

    /// <summary>
    /// Converts the value of this instance to a <see cref="String"/> and combines it with the given <see cref="Uri"/>.
    /// </summary>
    /// <param name="uri">The URI to combine with.</param>
    /// <returns>The combined URI.</returns>
    public Uri CombineWithUri(Uri uri) =>
        CombineWithUri(
            uri ?? throw new ArgumentNullException(nameof(uri)),
            ToString());

    /// <summary>
    /// Combines a query with a URI.
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <param name="query">The query.</param>
    /// <returns>The combined URI.</returns>
    [return: NotNullIfNotNull("uri")]
    [return: NotNullIfNotNull("query")]
    public static string? CombineWithUri(string? uri, string? query)
    {
        if (string.IsNullOrEmpty(query))
            return uri;
        if (uri == null)
            return query;

        int ql = uri.Length;
        if (ql == 0)
            return query ?? string.Empty;

        int fsi = uri.IndexOf('#');
        if (fsi != -1)
            ql = fsi;

        int qsi = uri.IndexOf(QuerySeparator, 0, ql);

        if (qsi == ql - 1)
        {
            return InsertOrConcat(uri, fsi, query);
        }
        else
        {
            char delimeter = qsi == -1 ? QuerySeparator : ParameterSeparator;
            return InsertOrConcat(uri, fsi, delimeter + query);
        }
    }

    static string InsertOrConcat(string s, int startIndex, string value)
    {
        if (startIndex == -1)
            return s + value;
        else
            return s.Insert(startIndex, value);
    }

    /// <summary>
    /// Combines a query with a URI.
    /// </summary>
    /// <param name="uri">The URI.</param>
    /// <param name="query">The query.</param>
    /// <returns>The combined URI.</returns>
    public static Uri CombineWithUri(Uri uri, string? query)
    {
        if (uri == null)
            throw new ArgumentNullException(nameof(uri));

        if (string.IsNullOrEmpty(query))
            return uri;
        else
            return new Uri(CombineWithUri(uri.ToString(), query));
    }
}
