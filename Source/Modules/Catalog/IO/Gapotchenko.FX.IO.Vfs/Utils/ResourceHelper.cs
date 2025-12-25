// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.IO.Vfs.Properties;

namespace Gapotchenko.FX.IO.Vfs.Utils;

static class ResourceHelper
{
    public static string ObjectDoesNotImplInterface(string? name) =>
        string.Format(Resources.ObjectDoesNotImplXInterface, name);

    public static string FileStorageCannotBeOpenedInMode(FileMode mode) =>
        string.Format(Resources.FileStorageCannotBeOpenedInXMode, mode.ToString());
}
