using System.Diagnostics;

namespace Gapotchenko.FX.Diagnostics;

static class Log
{
    public static readonly TraceSource TraceSource = new("Gapotchenko.FX.Diagnostics.WebBrowser");
}
