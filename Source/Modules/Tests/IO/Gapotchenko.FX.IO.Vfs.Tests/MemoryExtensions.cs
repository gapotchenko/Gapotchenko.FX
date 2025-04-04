#pragma warning disable IDE0031 // Use null propagation

namespace Gapotchenko.FX.IO.Vfs.Tests;

static class MemoryExtensions
{
    public static string? ToNullableString(this ReadOnlySpan<char> span) =>
        span == null ? null : span.ToString();
}
