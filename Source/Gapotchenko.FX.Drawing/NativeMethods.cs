#if TF_NET_FRAMEWORK

using System.Runtime.InteropServices;

namespace Gapotchenko.FX.Drawing
{
    static class NativeMethods
    {
        [DllImport("dwmapi.dll")]
        public static extern int DwmIsCompositionEnabled(out bool enabled);
    }
}

#endif
