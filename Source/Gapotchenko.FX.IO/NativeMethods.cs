using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.IO
{
    static class NativeMethods
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetShortPathName(
           [MarshalAs(UnmanagedType.LPTStr)] string lpszLongPath,
           [MarshalAs(UnmanagedType.LPTStr)] StringBuilder lpszShortPath,
           int cchBuffer);
    }
}
