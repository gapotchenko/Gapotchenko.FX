using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Gapotchenko.FX.Runtime.CompilerServices
{
    static class Log
    {
        public static readonly TraceSource TraceSource = new TraceSource("Gapotchenko.FX.Runtime.CompilerServices.Intrinsics", SourceLevels.Error);
    }
}
