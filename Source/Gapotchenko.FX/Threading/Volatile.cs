using System.Runtime.CompilerServices;

#if NET40

#error Missing polyfill.

#else

[assembly: TypeForwardedTo(typeof(Volatile))]

#endif
