﻿using System.Diagnostics;

namespace Gapotchenko.FX.Runtime.CompilerServices;

static class Log
{
    public static readonly TraceSource TraceSource = new("Gapotchenko.FX.Runtime.CompilerServices.Intrinsics", SourceLevels.Error);
}
