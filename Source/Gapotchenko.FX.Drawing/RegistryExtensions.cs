#if TF_NET_FRAMEWORK

using Microsoft.Win32;
using System;

namespace Gapotchenko.FX.Drawing
{
    static class RegistryExtensions
    {
        public static bool TryGetInt32Value(this RegistryKey registryKey, string name, out int value)
        {
            if (registryKey == null)
                throw new ArgumentNullException(nameof(registryKey));
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            var x = registryKey.GetValue(name) as int?;

            value = x.GetValueOrDefault();
            return x.HasValue;
        }
    }
}

#endif
