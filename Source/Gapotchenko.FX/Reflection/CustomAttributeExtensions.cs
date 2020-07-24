using System.Reflection;
using System.Runtime.CompilerServices;

#if !TFF_CUSTOM_ATTRIBUTE_EXTENSIONS

#error Missing polyfill.

#else

[assembly: TypeForwardedTo(typeof(CustomAttributeExtensions))]

#endif
