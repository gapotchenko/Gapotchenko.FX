using System.Diagnostics;

namespace Gapotchenko.FX.Diagnostics
{
    static class Log
    {
        public static readonly TraceSource TraceSource = new TraceSource("Gapotchenko.FX.Diagnostics.WebBrowser");
    }
}
