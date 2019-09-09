#if TF_NET_FRAMEWORK

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Drawing
{
    static class NativeMethods
    {
        [DllImport("dwmapi.dll")]
        public static extern int DwmIsCompositionEnabled(out bool enabled);
    }
}

#endif
