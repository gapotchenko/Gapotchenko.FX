// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

namespace Gapotchenko.FX;

/// <summary>
/// Provides polyfills for <see cref="Uri"/> class.
/// </summary>
public static class UriEx
{
    /// <summary>
    /// Combines two strings into a URI.
    /// </summary>
    /// <param name="uri1">The first URI to combine.</param>
    /// <param name="uri2">The second URI to combine.</param>
    /// <returns>
    /// The combined URIs.
    /// If one of the specified URIs is a zero-length string, this method returns the other URI.
    /// If <paramref name="uri2"/> contains an absolute URI, this method returns <paramref name="uri2"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="uri1"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="uri2"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="uri1"/> is an invalid URI.</exception>
    /// <exception cref="ArgumentException"><paramref name="uri2"/> is an invalid URI.</exception>
    public static string Combine(string uri1, string uri2)
    {
        if (uri1 is null)
            throw new ArgumentNullException(nameof(uri1));
        if (uri2 is null)
            throw new ArgumentNullException(nameof(uri2));

        if (!Uri.TryCreate(uri1, UriKind.RelativeOrAbsolute, out var baseUri))
            throw new ArgumentException("Invalid URI.", nameof(uri1));
        if (!Uri.TryCreate(uri2, UriKind.RelativeOrAbsolute, out var relativeUri))
            throw new ArgumentException("Invalid URI.", nameof(uri2));

        return DoCombine(baseUri, relativeUri).ToString();
    }

    /// <summary>
    /// Combines two relative or absolute URIs into a resulting URI.
    /// </summary>
    /// <param name="uri1">The first URI to combine.</param>
    /// <param name="uri2">The second URI to combine.</param>
    /// <returns>
    /// The combined URIs.
    /// If one of the specified URIs is empty, this method returns the other URI.
    /// If <paramref name="uri2"/> contains an absolute URI, this method returns <paramref name="uri2"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="uri1"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="uri2"/> is <see langword="null"/>.</exception>
    public static Uri Combine(Uri uri1, Uri uri2)
    {
        if (uri1 is null)
            throw new ArgumentNullException(nameof(uri1));
        if (uri2 is null)
            throw new ArgumentNullException(nameof(uri2));

        return DoCombine(uri1, uri2);
    }

    static Uri DoCombine(Uri uri1, Uri uri2)
    {
        if (uri2.IsAbsoluteUri)
            return uri2;

        if (uri1.IsAbsoluteUri)
        {
            return CombineCore(uri1, uri2);
        }
        else
        {
            var dummyUri = new Uri("http://example.com/");
            return dummyUri.MakeRelativeUri(CombineCore(new Uri(dummyUri, uri1), uri2));
        }

        static Uri CombineCore(Uri baseUri, Uri relativeUri)
        {
            var ub = new UriBuilder(baseUri);
            var rub = new UriBuilder(new Uri(baseUri, relativeUri));

            var ruos = relativeUri.OriginalString;
            if (ruos.StartsWith("/", StringComparison.Ordinal) || ruos.StartsWith(@"\", StringComparison.Ordinal))
                ub.Path = rub.Path;
            else if (ub.Path is var ubPath && (ubPath.EndsWith("/", StringComparison.Ordinal) || ubPath.EndsWith(@"\", StringComparison.Ordinal)))
                ub.Path += rub.Path[1..];
            else
                ub.Path += rub.Path;

            return UriQueryBuilder.CombineWithUri(ub.Uri, rub.Query);
        }
    }
}
