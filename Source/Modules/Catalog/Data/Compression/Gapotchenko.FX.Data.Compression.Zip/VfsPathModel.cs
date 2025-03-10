// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.Collections.Generic;
using Gapotchenko.FX.IO;
using Gapotchenko.FX.Linq;

namespace Gapotchenko.FX.Data.Compression.Zip;

readonly struct VfsPathModel : IEquatable<VfsPathModel>
{
    public VfsPathModel(string? path)
    {
        if (!string.IsNullOrEmpty(path))
            m_Parts = Normalize(FileSystem.SplitPath(path));
    }

    VfsPathModel(VfsPathModel model)
    {
        if (model.m_Parts is not null and var parts)
            m_Parts = new(parts);
    }

    static Deque<string>? Normalize(IEnumerable<string> parts)
    {
        var deque = new Deque<string>();

        foreach (string part in parts)
        {
            switch (part.AsSpan())
            {
                case "" or ".":
                    continue;

                case "..":
                    if (!deque.TryPopBack(out _))
                    {
                        // The path points to a directory outside of the virtual file system hierarchy.
                        return null;
                    }
                    break;

                case var x when x.TrimStart("/\\".AsSpan()).Length is 0:
                    break;

                default:
                    deque.PushBack(part);
                    break;
            }
        }

        return deque;
    }

    [MemberNotNullWhen(false, nameof(Path))]
    public readonly bool IsNil => m_Parts is null;

    [MemberNotNullWhen(true, nameof(Path))]
    public readonly bool IsRoot => m_Parts?.Count == 0;

    public readonly int HierarchyLevel => m_Parts?.Count ?? 0;

    public string? TryPeekBack()
    {
        var parts = m_Parts;
        if (parts is null)
            return null;
        else if (parts.TryPeekBack(out string? part))
            return part;
        else
            return null;
    }

    public string? PopBack() =>
        (m_Parts ?? throw new InvalidOperationException()).PopBack();

    public string? TryPopBack()
    {
        var parts = m_Parts;
        if (parts is null)
            return null;
        else if (parts.TryPopBack(out string? part))
            return part;
        else
            return null;
    }

    public bool StartsWith(VfsPathModel other)
    {
        var a = m_Parts;
        var b = other.m_Parts;

        return
            a == b ||
            a is not null && b is not null &&
            a.StartsWith(b, PartComparer);
    }

    public VfsPathModel Clone() => new(this);

    public override bool Equals(object? obj) => obj is VfsPathModel model && Equals(model);

    public bool Equals(VfsPathModel other)
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

    public override string? ToString() => Path;

    public string? Path =>
        m_Parts is not null and var parts
            ? string.Join("/", parts)
            : null;

    readonly Deque<string>? m_Parts;
}
