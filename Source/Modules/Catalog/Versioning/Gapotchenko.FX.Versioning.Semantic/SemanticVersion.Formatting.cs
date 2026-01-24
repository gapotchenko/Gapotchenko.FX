// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using System.Diagnostics;
using System.Text;

namespace Gapotchenko.FX.Versioning;

partial record SemanticVersion
{
    /// <summary>
    /// Converts the value of the current <see cref="SemanticVersion"/> object to its equivalent <see cref="String"/> representation.
    /// </summary>
    /// <returns>The string representation of the current <see cref="SemanticVersion"/> object.</returns>
    public override string ToString() => m_CachedString ??= ToStringCore();

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [NonSerialized]
    string? m_CachedString;

    string ToStringCore()
    {
        var builder = new StringBuilder()
            .Append(m_Major)
            .Append('.').Append(m_Minor)
            .Append('.').Append(m_Patch);

        if (m_Prerelease is { } prerelease)
            builder.Append('-').Append(prerelease);

        if (m_Build is { } build)
            builder.Append('+').Append(build);

        return builder.ToString();
    }
}
