using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#if !TFF_RUNTIME_INFORMATION

#error Missing polyfill.

#else

[assembly: TypeForwardedTo(typeof(RuntimeInformation))]

#endif
