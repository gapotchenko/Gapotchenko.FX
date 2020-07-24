using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

#if NET40

#error Missing polyfill.

#else

[assembly: TypeForwardedTo(typeof(ExceptionDispatchInfo))]

#endif
