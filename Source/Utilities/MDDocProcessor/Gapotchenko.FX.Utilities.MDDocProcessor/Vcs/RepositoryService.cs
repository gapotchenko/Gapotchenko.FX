namespace Gapotchenko.FX.Utilities.MDDocProcessor.Vcs;

sealed class RepositoryService(string basePath)
{
    public Uri? TryMapUri(Uri uri, RepositoryUriUsage usage)
    {
        string? localPath = null;

        if (uri.IsAbsoluteUri)
        {
            if (!uri.IsFile)
                return null;

            localPath = uri.LocalPath;
            int j = localPath.IndexOf('?');
            if (j != -1)
                localPath = localPath[0..j];

            j = localPath.IndexOf('#');
            if (j != -1)
                localPath = localPath[0..j];

            var rootUri = new Uri(RootPath);
            uri = rootUri.MakeRelativeUri(uri);
            uri = new Uri(Uri.UnescapeDataString(uri.OriginalString), UriKind.Relative);
        }

        string s = uri.OriginalString;

        if (s.StartsWith("../../.."))
        {
            return null;
        }
        else if (s.StartsWith("../../"))
        {
            // GitHub services like Wiki etc.

            var baseUri = new Uri("https://github.com/" + RepositoryUriSuffix + "/tree/branch/");
            var newUri = new Uri(baseUri, uri);
            return newUri;
        }
        else
        {
            string? query = null;
            int j = s.IndexOf('?');
            if (j != -1)
            {
                query = s[(j + 1)..];
                s = s[0..j];
            }

            bool raw =
                usage == RepositoryUriUsage.Resource ||
                query?.Contains("raw=true") == true;

            bool blob = Path.GetExtension(s).ToLowerInvariant() is ".png" or ".jpg" or ".jpeg" or ".gif";

            string? packageName = null;
            if (!(raw || blob) && localPath != null && Directory.Exists(localPath))
            {
                if (File.Exists(Path.Combine(localPath, "README.md")) &&
                    Directory.EnumerateFiles(localPath, "*.*proj").Any())
                {
                    packageName = Path.GetFileName(localPath);
                }
            }

            if (packageName != null)
                return new Uri($"https://www.nuget.org/packages/{Uri.EscapeDataString(packageName)}");

            var baseUri = new Uri(
                raw ?
                    $"https://raw.githubusercontent.com/{RepositoryUriSuffix}/{Uri.EscapeDataString(OrigHead)}/" :
                    $"https://github.com/{RepositoryUriSuffix}/{(blob ? "blob" : "tree")}/{Uri.EscapeDataString(OrigHead)}/"
                );
            var newUri = new Uri(baseUri, new Uri(s, UriKind.Relative));
            return newUri;
        }
    }

    const string RepositoryUriSuffix = "gapotchenko/Gapotchenko.FX";

    string OrigHead => m_CachedOrigHead ??= GetOrigHeadCore();

    string? m_CachedOrigHead;

    string GetOrigHeadCore() =>
        File.ReadAllText(
            Path.Combine(RootPath, ".git", "ORIG_HEAD"))
        .Trim();

    public string RootPath => m_CachedRootPath ??= GetRootPathCore();

    string? m_CachedRootPath;

    string GetRootPathCore()
    {
        string path = basePath;

        for (; ; )
        {
            string? newPath = Path.GetDirectoryName(path);
            if (newPath == null)
                throw new Exception("Cannot locate repository root path.");

            path = newPath;

            string probingPath = Path.Combine(path, ".gitignore");
            if (File.Exists(probingPath))
            {
                path += Path.DirectorySeparatorChar;
                break;
            }
        }

        return path;
    }
}
