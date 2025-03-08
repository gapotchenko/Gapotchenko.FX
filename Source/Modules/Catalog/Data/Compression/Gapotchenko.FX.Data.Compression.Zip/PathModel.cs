using Gapotchenko.FX.IO;
using Gapotchenko.FX.Linq;

namespace Gapotchenko.FX.Data.Compression.Zip;

readonly struct PathModel : IEquatable<PathModel>
{
    public PathModel(string? path)
    {
        if (!string.IsNullOrEmpty(path))
            m_Parts = Normalize(FileSystem.SplitPath(path));
    }

    static Stack<string>? Normalize(IEnumerable<string> parts)
    {
        var stack = new Stack<string>();
        foreach (string part in parts)
        {
            switch (part)
            {
                case "" or ".":
                    continue;

                case "..":
                    if (stack.Count == 0)
                    {
                        // The path points to a directory outside of the archive file system hierarchy.
                        return null;
                    }
                    stack.Pop();
                    break;

                default:
                    stack.Push(part);
                    break;
            }
        }

        return stack;
    }

    public readonly bool IsNil => m_Parts is null;

    public readonly bool IsRoot => m_Parts?.Count == 0;

    public string? TryPop()
    {
        var parts = m_Parts;
        if (parts is null)
            return null;
        else if (parts.Count == 0)
            return null;
        else
            return parts.Pop();
    }

    public bool StartsWith(PathModel other)
    {
        var a = m_Parts;
        var b = other.m_Parts;

        return
            a == b ||
            a is not null && b is not null &&
            a.StartsWith(b, PartComparer);
    }


    public override bool Equals(object? obj) => obj is PathModel model && Equals(model);

    public bool Equals(PathModel other)
    {
        var a = m_Parts;
        var b = other.m_Parts;

        return
            a == b ||
            a is not null && b is not null &&
            a.SequenceEqual(b, PartComparer);
    }

    public override int GetHashCode() => HashCodeEx.SequenceCombine(m_Parts, PartComparer);

    static StringComparer PartComparer => StringComparer.InvariantCulture;

    public override string? ToString() =>
        m_Parts is not null and var parts
            ? string.Join("/", parts)
            : null;

    readonly Stack<string>? m_Parts;
}
