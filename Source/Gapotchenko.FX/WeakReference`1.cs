using System.Runtime.CompilerServices;

#if !TFF_WEAKREFERENCE_1

#error Missing polyfill.

#else

[assembly: TypeForwardedTo(typeof(WeakReference<>))]

#endif
