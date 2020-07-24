using System.Runtime.CompilerServices;
using System.Threading;

#if NET40

#error Missing polyfill.

#else

[assembly: TypeForwardedTo(typeof(Volatile))]

#endif
