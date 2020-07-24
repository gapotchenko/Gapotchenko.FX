using System.Collections.Generic;
using System.Runtime.CompilerServices;

#if !TFF_READONLY_COLLECTION

#error Missing polyfill.

#else

[assembly: TypeForwardedTo(typeof(IReadOnlyCollection<>))]

#endif
