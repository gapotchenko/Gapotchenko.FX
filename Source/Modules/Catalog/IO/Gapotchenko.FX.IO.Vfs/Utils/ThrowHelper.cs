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
    public static void FSDoesNotSupportReading() =>
        throw new NotSupportedException(Resources.FSDoesNotSupportReading);

    [DoesNotReturn]
    public static void FSDoesNotSupportWriting() =>
        throw new NotSupportedException(Resources.FSDoesNotSupportWriting);
}
