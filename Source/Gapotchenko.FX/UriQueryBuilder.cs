using System;
using System.Text;

namespace Gapotchenko.FX
{
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
            _SB = new StringBuilder();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UriQueryBuilder"/> class using the specified string.
        /// </summary>
        /// <param name="query">The query.</param>
        public UriQueryBuilder(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                _SB = new StringBuilder();
            }
            else
            {
                if (query[0] == QuerySeparator)
                {
                    // UriBuilder.Query property getter appends '?' on return result.
                    // Trim it to avoid double query separators in resulting URI.
                    _SB = new StringBuilder(query, 1, query.Length - 1, 0);
                }
                else
                {
                    _SB = new StringBuilder(query);
                }
            }
        }

        readonly StringBuilder _SB;

        /// <summary>
        /// Appends a query parameter to this instance.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="value">The parameter value.</param>
        /// <returns>The <see cref="UriQueryBuilder"/> instance.</returns>
        public UriQueryBuilder AppendParameter(string name, string value)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (name.Length == 0)
                throw new ArgumentException("A query parameter name cannot be empty.", nameof(name));

            var sb = _SB;

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
        public static string AppendParameter(string uri, string name, string value) => new UriQueryBuilder().AppendParameter(name, value).CombineWithUri(uri);

        /// <summary>
        /// Appends a query parameter to the given <see cref="Uri"/>.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="name">The parameter name.</param>
        /// <param name="value">The parameter value.</param>
        /// <returns>The <see cref="Uri"/> with an appended query parameter.</returns>
        public static Uri AppendParameter(Uri uri, string name, string value) => new UriQueryBuilder().AppendParameter(name, value).CombineWithUri(uri);

        /// <summary>
        /// Checks whether a query in this instance has a parameter with the given name.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <returns><c>true</c> if a query has a parameter with the given name; otherwise, <c>false</c>.</returns>
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
        /// Removes all the characters from a query string in this instance.
        /// </summary>
        /// <returns>The <see cref="UriQueryBuilder"/> instance.</returns>
        public UriQueryBuilder Clear()
        {
            _SB.Clear();
            return this;
        }

        /// <summary>
        /// Gets the length of a query string in this instance.
        /// </summary>
        public int Length => _SB.Length;

        /// <summary>
        /// Converts the value of this instance to a <see cref="String"/>.
        /// </summary>
        /// <returns>A string whose value is the same as this instance.</returns>
        public override string ToString() => _SB.ToString();

        /// <summary>
        /// Converts the value of this instance to a <see cref="String"/> and combines it with the given URI.
        /// </summary>
        /// <param name="uri">The URI to combine with.</param>
        /// <returns>The combined URI.</returns>
        public string CombineWithUri(string uri) => CombineWithUri(uri, ToString());

        /// <summary>
        /// Converts the value of this instance to a <see cref="String"/> and combines it with the given <see cref="Uri"/>.
        /// </summary>
        /// <param name="uri">The URI to combine with.</param>
        /// <returns>The combined URI.</returns>
        public Uri CombineWithUri(Uri uri) => CombineWithUri(uri, ToString());

        /// <summary>
        /// Combines a query with a URI.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="query">The query.</param>
        /// <returns>The combined URI.</returns>
        public static string CombineWithUri(string uri, string query)
        {
            if (string.IsNullOrEmpty(query))
                return uri;
            if (uri == null)
                return query;
            if (uri.Length == 0)
                return query ?? string.Empty;

            int i = uri.IndexOf(QuerySeparator);
            if (i == -1)
                return uri + QuerySeparator + query;
            if (i == uri.Length - 1)
                return uri + query;

            return uri + ParameterSeparator + query;
        }

        /// <summary>
        /// Combines a query with a URI.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="query">The query.</param>
        /// <returns>The combined URI.</returns>
        public static Uri CombineWithUri(Uri uri, string query)
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));
            if (string.IsNullOrEmpty(query))
                return uri;
            return new Uri(CombineWithUri(uri.ToString(), query));
        }
    }
}
