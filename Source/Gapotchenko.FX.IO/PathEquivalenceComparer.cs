using System.Diagnostics.CodeAnalysis;

namespace Gapotchenko.FX.IO;

sealed class PathEquivalenceComparer : StringComparer
{
    public static PathEquivalenceComparer Instance { get; } = new();

    public override int Compare(string? x, string? y) => FileSystem.PathComparer.Compare(MapPath(x), MapPath(y));

    public override bool Equals(string? x, string? y) => FileSystem.PathsAreEquivalent(x, y);

    public override int GetHashCode(string obj) => FileSystem.PathComparer.GetHashCode(MapPath(obj));

    [return: NotNullIfNotNull("path")]
    static string? MapPath(string? path)
    {
        if (path == null)
            return null;

        path = FileSystem.NormalizePath(path, false);
        path = Path.GetFullPath(path);

        return path;
    }
}
