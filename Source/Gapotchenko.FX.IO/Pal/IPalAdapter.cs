// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

namespace Gapotchenko.FX.IO.Pal;

/// <summary>
/// Defines the interface of a platform abstraction layer (PAL) adapter.
/// </summary>
interface IPalAdapter
{
    bool IsCaseSensitive { get; }

    string GetShortPath(string path);

    string GetRealPath(string path);

    int GetRootPathLength(ReadOnlySpan<char> path);
}
