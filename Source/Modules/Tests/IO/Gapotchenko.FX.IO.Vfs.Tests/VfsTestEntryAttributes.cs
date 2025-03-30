// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.IO.Vfs.Kits;
using System.Globalization;
using System.Text;

namespace Gapotchenko.FX.IO.Vfs.Tests;

public sealed class VfsTestEntryAttributes : IEquatable<VfsTestEntryAttributes>
{
    public static VfsTestEntryAttributes Get(IReadOnlyFileSystemView view, string path) => new(view, path);

    VfsTestEntryAttributes(IReadOnlyFileSystemView view, string path)
    {
        if (!view.EntryExists(path))
            throw new FileNotFoundException(VfsResourceKit.CouldNotFindFile(path), path);

        if (view.SupportsLastWriteTime)
            LastWriteTime = view.GetLastWriteTime(path);
    }

    #region Equality

    public override bool Equals(object? obj) =>
        obj is VfsTestEntryAttributes other &&
        Equals(other);

    public bool Equals(VfsTestEntryAttributes? other)
    {
        if (other == null)
            return true;

        if (LastWriteTime.HasValue &&
            other.LastWriteTime.HasValue &&
            (LastWriteTime.Value.ToUniversalTime() - other.LastWriteTime.Value.ToUniversalTime()).Duration() > m_DurationEpsilon)
        {
            return false;
        }

        return true;
    }

    public override int GetHashCode() => 0;

    public static TimeSpan DurationEpsilon => m_DurationEpsilon;

    static readonly TimeSpan m_DurationEpsilon = TimeSpan.FromSeconds(2);

    #endregion

    public override string ToString()
    {
        var sb = new StringBuilder();
        if (LastWriteTime.HasValue)
            sb.Append("LastWriteTime=").Append(LastWriteTime.Value.ToString("u", CultureInfo.InvariantCulture));
        return sb.ToString();
    }

    public DateTime? LastWriteTime { get; }
}
