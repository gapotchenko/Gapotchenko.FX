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

    PathModel(IEnumerable<string>? parts)
    {
        if (parts != null)
            m_Parts = new(parts);
    }

    static Queue<string>? Normalize(IEnumerable<string> parts)
    {
        var stack = new Queue<string>();
        foreach (string part in parts)
        {
            switch (part.AsSpan())
            {
                case "" or ".":
                    continue;

                case "..":
                    if (stack.Count == 0)
                    {
                        // The path points to a directory outside of the archive file system hierarchy.
                        return null;
                    }
                    stack.Dequeue();
                    break;

                case var x when x.TrimStart("/\\".AsSpan()).Length is 0:
                    break;

                default:
                    stack.Enqueue(part);
                    break;
            }
        }

        return stack;
    }

    public readonly bool IsNil => m_Parts is null;

    public readonly bool IsRoot => m_Parts?.Count == 0;

    public readonly int HierarchyLevel => m_Parts?.Count ?? 0;

    public string? TryPeek()
    {
        var parts = m_Parts;
        if (parts is null)
            return null;
        else if (parts.Count == 0)
            return null;
        else
            return parts.Peek();
    }

    public string? TryUp()
    {
        var parts = m_Parts;
        if (parts is null)
            return null;
        else if (parts.Count == 0)
            return null;
        else
            return parts.Dequeue();
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

    public PathModel Clone() => new(m_Parts);

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

    readonly Queue<string>? m_Parts;
}
