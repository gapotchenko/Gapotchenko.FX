using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Gapotchenko.FX.Diagnostics
{
    static class Log
    {
        public static readonly TraceSource TraceSource = new TraceSource("Gapotchenko.FX.Diagnostics.WebBrowser");
    }
}
