// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2019

using Gapotchenko.FX.Reflection.Loader.Polyfills;
using System.Globalization;
using System.Reflection;

namespace Gapotchenko.FX.Reflection.Loader.Utils;

sealed class AssemblyNameEqualityComparer : IEqualityComparer<AssemblyName>
{
    AssemblyNameEqualityComparer()
    {
    }

    public static AssemblyNameEqualityComparer Instance { get; } = new();

    public bool Equals(AssemblyName? x, AssemblyName? y)
    {
        if (ReferenceEquals(x, y))
            return true;
        if (x is null || y is null)
            return false;

        return
            x.Version == y.Version &&
            string.Equals(x.Name, y.Name, StringComparison.OrdinalIgnoreCase) &&
            ArrayEqualityComparer.Equals(x.GetPublicKeyToken(), y.GetPublicKeyToken()) &&
            // CultureInfo '==' operator compares references, not values.
            // Using the default equality comparer allows to workaround that.
            EqualityComparer<CultureInfo>.Default.Equals(x.CultureInfo, y.CultureInfo);
    }

    public int GetHashCode(AssemblyName obj)
    {
        int h = obj.Version?.GetHashCode() ?? 0;
        if (obj.Name is not null and var name)
            h ^= StringComparer.OrdinalIgnoreCase.GetHashCode(name);
        if (obj.GetPublicKeyToken() is not null and var publicKeyToken)
            h ^= ArrayEqualityComparer.GetHashCode(publicKeyToken);
        if (obj.CultureInfo is not null and var cultureInfo)
            h ^= cultureInfo.GetHashCode();
        return h;
    }
}
