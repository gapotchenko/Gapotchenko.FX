// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Gapotchenko.FX;

/// <summary>
/// Provides polyfill methods for <see cref="Uri"/> class.
/// </summary>
public static class UriEx
{
    /// <summary>
    /// Combines two specified strings into a URI.
    /// </summary>
    /// <param name="uri1">The first URI to combine.</param>
    /// <param name="uri2">The second URI to combine.</param>
    /// <returns>
    /// The combined URIs.
    /// If one of the specified URIs is a zero-length string, this method returns the other URI.
    /// If <paramref name="uri2"/> contains an absolute URI, this method returns <paramref name="uri2"/>.
    /// </returns>
    /// <exception cref="ArgumentException"><paramref name="uri1"/> is an invalid URI.</exception>
    /// <exception cref="ArgumentException"><paramref name="uri2"/> is an invalid URI.</exception>
    [return: NotNullIfNotNull(nameof(uri1))]
    [return: NotNullIfNotNull(nameof(uri2))]
    public static string? Combine(string? uri1, string? uri2)
    {
        if (uri1 is null)
            return uri2;
        if (uri2 is null)
            return uri1;

        if (!Uri.TryCreate(uri1, UriKind.RelativeOrAbsolute, out var baseUri))
            throw new ArgumentException("Invalid URI.", nameof(uri1));
        if (!Uri.TryCreate(uri2, UriKind.RelativeOrAbsolute, out var relativeUri))
            throw new ArgumentException("Invalid URI.", nameof(uri2));

        return DoCombine(baseUri, relativeUri).ToString();
    }

    /// <summary>
    /// Combines two specified URIs into a combined URI.
    /// </summary>
    /// <param name="uri1">The first URI to combine.</param>
    /// <param name="uri2">The second URI to combine.</param>
    /// <returns>
    /// The combined URI.
    /// If one of the specified URIs is empty, this method returns the other URI.
    /// If <paramref name="uri2"/> contains an absolute URI, this method returns <paramref name="uri2"/>.
    /// </returns>
    [return: NotNullIfNotNull(nameof(uri1))]
    [return: NotNullIfNotNull(nameof(uri2))]
    public static Uri? Combine(Uri? uri1, Uri? uri2)
    {
        if (uri1 is null)
            return uri2;
        if (uri2 is null)
            return uri1;

        return DoCombine(uri1, uri2);
    }

    static Uri DoCombine(Uri uri1, Uri uri2)
    {
        if (uri2.IsAbsoluteUri)
        {
            return uri2;
        }
        else if (uri1.OriginalString is var uos1 && uos1.Length == 0)
        {
            return uri2;
        }
        else if (uri2.OriginalString is var uos2 && uos2.Length == 0)
        {
            return uri1;
        }
        else if (uri1.IsAbsoluteUri)
        {
            return CombineCore(uri1, uri2);
        }
        else if (IsRootedRelativeUri(uos2))
        {
            return uri2;
        }
        else
        {
            var dummyUri = GetDummyAbsoluteUri();
            var uri = dummyUri.MakeRelativeUri(
                CombineCore(
                    new Uri(dummyUri, uri1),
                    uri2));
            if (IsRootedRelativeUri(uos1))
                uri = new Uri("/" + uri.ToString(), UriKind.Relative);
            return uri;
        }
    }

    static Uri CombineCore(Uri baseUri, Uri relativeUri)
    {
        Debug.Assert(baseUri.IsAbsoluteUri);
        Debug.Assert(!relativeUri.IsAbsoluteUri);

        var nru = new Uri(GetDummyAbsoluteUri(), relativeUri);

        var ub = new UriBuilder(baseUri);
        var rub = new UriBuilder(nru);

        bool absoluteAuthority = false;
        var ruos = relativeUri.OriginalString;

        if (IsRootedRelativeUri(ruos))
        {
            if (ruos.StartsWith("//", StringComparison.Ordinal))
            {
                ub.Host = rub.Host;
                if (!nru.IsDefaultPort)
                    ub.Port = rub.Port;
                absoluteAuthority = true;
            }

            ub.Path = rub.Path;
        }
        else if (ub.Path is var ubPath && (ubPath.EndsWith("/", StringComparison.Ordinal) || ubPath.EndsWith(@"\", StringComparison.Ordinal)))
        {
            ub.Path += rub.Path[1..];
        }
        else
        {
            ub.Path += rub.Path;
        }

        ub.Fragment = rub.Fragment
#if !(NETCOREAPP || NETSTANDARD2_1_OR_GREATER)
            .TrimStart('#')
#endif
            ;

        if (absoluteAuthority)
        {
            ub.Query = rub.Query
#if !(NETCOREAPP || NETSTANDARD2_1_OR_GREATER)
                .TrimStart(UriQueryBuilder.QuerySeparator)
#endif
                ;

            return ub.Uri;
        }
        else
        {
            return UriQueryBuilder.CombineWithUri(ub.Uri, rub.Query);
        }
    }

    static Uri GetDummyAbsoluteUri() => new("http://x/");

    static bool IsRootedRelativeUri(string uri) =>
        uri.Length > 0 &&
        uri[0] is '/' or '\\';
}
