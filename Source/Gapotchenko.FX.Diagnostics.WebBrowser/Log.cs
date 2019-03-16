using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Gapotchenko.FX.Diagnostics
{
    static class Log
    {
        public static TraceSource TraceSource { get; } = new TraceSource("Gapotchenko.FX.Diagnostics.WebBrowser");
    }
}
