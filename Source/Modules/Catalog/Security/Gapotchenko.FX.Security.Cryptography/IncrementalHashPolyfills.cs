// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
#define TFF_INCREMENTALHASH_APPENDDATA_SPAN
#endif

using System.Buffers;

namespace Gapotchenko.FX.Security.Cryptography;

/// <summary>
/// Provides polyfill extensions for <see cref="IncrementalHash"/> class.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class IncrementalHashPolyfills
{
    /// <summary>
    /// Appends the specified data to the hash.
    /// </summary>
    /// <param name="hash">The incremental hash instance.</param>
    /// <param name="data">The data to append.</param>
#if TFF_INCREMENTALHASH_APPENDDATA_SPAN
    [EditorBrowsable(EditorBrowsableState.Never)]
#endif
    public static void AppendData(
#if !TFF_INCREMENTALHASH_APPENDDATA_SPAN
        this
#endif
        IncrementalHash hash,
        ReadOnlySpan<byte> data)
    {
        ArgumentNullException.ThrowIfNull(hash);

#if TFF_INCREMENTALHASH_APPENDDATA_SPAN
        hash.AppendData(data);
#else
        var pool = ArrayPool<byte>.Shared;
        int bufferSize = Math.Min(data.Length, BufferSize);
        byte[] buffer = pool.Rent(bufferSize);
        try
        {
            do
            {
                int n = Math.Min(data.Length, bufferSize);
                data[..n].CopyTo(buffer);
                hash.AppendData(buffer, 0, n);
                data = data[n..];
            }
            while (!data.IsEmpty);
        }
        finally
        {
            CryptographicOperations.ZeroMemory(buffer.AsSpan(0, bufferSize));
            pool.Return(buffer);
        }
#endif
    }

    const int BufferSize = 4096;
}
