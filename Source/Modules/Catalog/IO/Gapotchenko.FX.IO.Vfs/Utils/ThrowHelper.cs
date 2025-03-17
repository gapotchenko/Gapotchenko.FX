// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using Gapotchenko.FX.IO.Vfs.Properties;
using System.Diagnostics;

namespace Gapotchenko.FX.IO.Vfs.Utils;

[StackTraceHidden]
static class ThrowHelper
{
    [DoesNotReturn]
    public static void CannotReadFS() => throw new NotSupportedException(Resources.CannotReadFS);

    [DoesNotReturn]
    public static void CannotWriteFS() => throw new NotSupportedException(Resources.CannotWriteFS);
}
