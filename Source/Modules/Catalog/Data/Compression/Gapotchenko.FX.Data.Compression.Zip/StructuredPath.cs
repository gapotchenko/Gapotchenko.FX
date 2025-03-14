// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.IO.Vfs.Kits;

namespace Gapotchenko.FX.Data.Compression.Zip;

readonly struct StructuredPath
{
    public static implicit operator StructuredPath(string? path) => new(path);

    public static implicit operator StructuredPath(string[]? parts) => new(parts);

    public static implicit operator StructuredPath(in Memory<string> parts) => new(parts);

    public static implicit operator StructuredPath(in ReadOnlyMemory<string> parts) => new(parts);

    public StructuredPath(string? path)
    {
        OriginalPath = path;
        Parts = VfsPathKit.Split(path);
    }

    public StructuredPath(in ReadOnlyMemory<string> parts)
    {
        Parts = parts;
    }

    public string? OriginalPath { get; }

    public ReadOnlyMemory<string> Parts { get; }

    public override string? ToString() =>
        OriginalPath ??
        VfsPathKit.Join(Parts.Span, ZipArchive.C_DirectorySeparatorChar);
}
