// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2022

namespace Gapotchenko.FX.IO;

/// <summary>
/// Provides polyfill methods for <see cref="Path"/> class.
/// </summary>
public static partial class PathEx
{
    // This class is partial. Please take a look at the neighboring source files.

    static bool IsDirectorySeparator(char c) =>
        c == Path.DirectorySeparatorChar ||
        c == Path.AltDirectorySeparatorChar;
}
