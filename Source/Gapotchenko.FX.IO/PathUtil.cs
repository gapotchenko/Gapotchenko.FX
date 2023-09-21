using Gapotchenko.FX.Text;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Gapotchenko.FX.IO;

static class PathUtil
{
    [return: NotNullIfNotNull(nameof(path))]
    internal static string? Normalize(string? path, bool? trailingSlash = null)
    {
        if (string.IsNullOrEmpty(path))
            return path;

        path = path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

        static string RemoveAdjacentChars(string value, char c, int startIndex)
        {
            int n = value.Length;

            ++startIndex;
            if (startIndex >= n)
                return value;

            var sb = new StringBuilder(value, 0, startIndex, n);

            char prevChar = value[startIndex - 1];
            for (int i = startIndex; i != n; ++i)
            {
                char ch = value[i];
                if (ch == c && ch == prevChar)
                    continue;
                prevChar = ch;
                sb.Append(ch);
            }

            return sb.ToString();
        }

        path = RemoveAdjacentChars(path, Path.DirectorySeparatorChar, 1);

        if (trailingSlash.HasValue)
        {
            if (trailingSlash.Value)
            {
                if (!path.EndsWith(Path.DirectorySeparatorChar))
                    path += Path.DirectorySeparatorChar;
            }
            else
            {
                if (path.EndsWith(Path.DirectorySeparatorChar))
                    path = path[..^1];
            }
        }

        return path;
    }
}
