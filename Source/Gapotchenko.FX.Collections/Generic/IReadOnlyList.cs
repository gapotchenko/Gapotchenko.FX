using System.Collections.Generic;
using System.Runtime.CompilerServices;

#if !TFF_READONLY_LIST

#error Missing polyfill.

#else

[assembly: TypeForwardedTo(typeof(IReadOnlyList<>))]

#endif
