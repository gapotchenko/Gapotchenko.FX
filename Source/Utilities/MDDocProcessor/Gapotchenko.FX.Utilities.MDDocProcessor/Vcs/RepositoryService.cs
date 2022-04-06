namespace Gapotchenko.FX.Utilities.MDDocProcessor.Vcs
{
    static class RepositoryService
    {
        const string RespositoryUriSuffix = "gapotchenko/Gapotchenko.FX";

        static string? _CachedRootPath;

        public static string RootPath => _CachedRootPath ??= _GetRootPathCore();

        static string _GetRootPathCore()
        {
            string path = typeof(RepositoryService).Assembly.Location;

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

        static string? _CachedOrigHead;

        static string OrigHead => _CachedOrigHead ??= _GetOrigHeadCore();

        static string _GetOrigHeadCore() =>
            File.ReadAllText(
                Path.Combine(RootPath, ".git", "ORIG_HEAD"))
            .Trim();

        public static Uri? TryMapUri(Uri uri)
        {
            if (uri.IsAbsoluteUri)
                return null;

            string s = uri.OriginalString;

            if (s.StartsWith("../../.."))
            {
                return null;
            }
            else if (s.StartsWith("../../"))
            {
                // GitHub services like Wiki etc.

                var baseUri = new Uri("https://github.com/" + RespositoryUriSuffix + "/tree/branch/");
                var newUri = new Uri(baseUri, uri);
                return newUri;
            }
            else
            {
                s = s.Replace("%3F", "?");

                string? query = null;
                int j = s.IndexOf('?');
                if (j != -1)
                {
                    query = s[(j + 1)..];
                    s = s[0..j];
                }

                bool raw = query?.Contains("raw=true") == true;

                var baseUri = new Uri(
                    raw ?
                        $"https://raw.githubusercontent.com/{RespositoryUriSuffix}/{OrigHead}/" :
                        $"https://github.com/{RespositoryUriSuffix}/blob/{OrigHead}/"
                    );
                var newUri = new Uri(baseUri, new Uri(s, UriKind.Relative));
                return newUri;
            }
        }
    }
}
