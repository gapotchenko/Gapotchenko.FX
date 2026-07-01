// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
// Portions © The Mono Project
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

#pragma warning disable CA5351 // Do Not Use Broken Cryptographic Algorithms

using System.Buffers.Binary;
using System.Numerics;

namespace Gapotchenko.FX.Security.Cryptography;

sealed class MD5UnapprovedImpl : MD5
{
    public MD5UnapprovedImpl()
    {
        m_ProcessingBuffer = new byte[BlockSizeInBytes];

        Initialize();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (m_ProcessingBuffer is { } processingBuffer)
            {
                m_ProcessingBuffer = null;
                CryptographicOperations.ZeroMemory(processingBuffer);
            }

            m_H1 = m_H2 = m_H3 = m_H4 = 0;
        }

        base.Dispose(disposing);
    }

    public override void Initialize()
    {
        m_Count = 0;
        m_ProcessingBufferCount = 0;

        m_H1 = 0x67452301;
        m_H2 = 0xefcdab89;
        m_H3 = 0x98badcfe;
        m_H4 = 0x10325476;
    }

    protected override void HashCore(byte[] rgb, int ibStart, int cbSize)
    {
        DoHashCore(rgb.AsSpan(ibStart, cbSize));
    }

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER

    protected override void HashCore(ReadOnlySpan<byte> source)
    {
        DoHashCore(source);
    }

#endif

    void DoHashCore(ReadOnlySpan<byte> rgb)
    {
        byte[] processingBuffer = GetProcessingBuffer();

        if (m_ProcessingBufferCount != 0)
        {
            int j = BlockSizeInBytes - m_ProcessingBufferCount;
            if (rgb.Length < j)
            {
                rgb.CopyTo(processingBuffer.AsSpan(m_ProcessingBufferCount));
                m_ProcessingBufferCount += rgb.Length;
                return;
            }
            else
            {
                rgb[..j].CopyTo(processingBuffer.AsSpan(m_ProcessingBufferCount));
                ProcessBlock(processingBuffer);
                m_ProcessingBufferCount = 0;
                rgb = rgb[j..];
            }
        }

        int cbSize = rgb.Length;
        int blockRem = cbSize % BlockSizeInBytes;
        int blockEnd = cbSize - blockRem;

        for (int i = 0; i < blockEnd; i += BlockSizeInBytes)
            ProcessBlock(rgb[i..]);

        if (blockRem != 0)
        {
            rgb.Slice(blockEnd, blockRem).CopyTo(processingBuffer);
            m_ProcessingBufferCount = blockRem;
        }
    }

    protected override byte[] HashFinal()
    {
        byte[] processingBuffer = GetProcessingBuffer();

        ProcessFinalBlock(processingBuffer, 0, m_ProcessingBufferCount);

        byte[] hash = new byte[16];
        BinaryPrimitives.WriteUInt32LittleEndian(hash, m_H1);
        BinaryPrimitives.WriteUInt32LittleEndian(hash.AsSpan(4), m_H2);
        BinaryPrimitives.WriteUInt32LittleEndian(hash.AsSpan(8), m_H3);
        BinaryPrimitives.WriteUInt32LittleEndian(hash.AsSpan(12), m_H4);

        return hash;
    }

    void ProcessFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
    {
        ulong total = m_Count + (ulong)inputCount;
        int paddingSize = (int)(56 - total % BlockSizeInBytes);

        if (paddingSize < 1)
            paddingSize += BlockSizeInBytes;

        int n = inputCount + paddingSize;
        Span<byte> fooBuffer = stackalloc byte[n + 8];

        for (int i = 0; i < inputCount; i++)
            fooBuffer[i] = inputBuffer[i + inputOffset];

        fooBuffer[inputCount] = 0x80;
        for (int i = inputCount + 1; i < n; ++i)
            fooBuffer[i] = 0x00;

        // The algorithm uses bits, not bytes.
        ulong size = total * 8;
        BinaryPrimitives.WriteUInt64LittleEndian(fooBuffer.Slice(n, sizeof(ulong)), size);
        ProcessBlock(fooBuffer);

        if (n + 8 == 128)
            ProcessBlock(fooBuffer[64..]);
    }

    void ProcessBlock(ReadOnlySpan<byte> inputBuffer)
    {
        m_Count += BlockSizeInBytes;

        Span<uint> x = stackalloc uint[16];
        for (int i = 0; i < x.Length; i++)
            x[i] = BinaryPrimitives.ReadUInt32LittleEndian(inputBuffer.Slice(i * sizeof(uint), sizeof(uint)));

        uint a = m_H1;
        uint b = m_H2;
        uint c = m_H3;
        uint d = m_H4;

        #region Round 1: F cycle × 16

        const int S11 = 7;
        const int S12 = 12;
        const int S13 = 17;
        const int S14 = 22;

        a = BitOperations.RotateLeft(a + F(b, c, d) + x[0] + 0xd76aa478, S11) + b;
        d = BitOperations.RotateLeft(d + F(a, b, c) + x[1] + 0xe8c7b756, S12) + a;
        c = BitOperations.RotateLeft(c + F(d, a, b) + x[2] + 0x242070db, S13) + d;
        b = BitOperations.RotateLeft(b + F(c, d, a) + x[3] + 0xc1bdceee, S14) + c;

        a = BitOperations.RotateLeft(a + F(b, c, d) + x[4] + 0xf57c0faf, S11) + b;
        d = BitOperations.RotateLeft(d + F(a, b, c) + x[5] + 0x4787c62a, S12) + a;
        c = BitOperations.RotateLeft(c + F(d, a, b) + x[6] + 0xa8304613, S13) + d;
        b = BitOperations.RotateLeft(b + F(c, d, a) + x[7] + 0xfd469501, S14) + c;

        a = BitOperations.RotateLeft(a + F(b, c, d) + x[8] + 0x698098d8, S11) + b;
        d = BitOperations.RotateLeft(d + F(a, b, c) + x[9] + 0x8b44f7af, S12) + a;
        c = BitOperations.RotateLeft(c + F(d, a, b) + x[10] + 0xffff5bb1, S13) + d;
        b = BitOperations.RotateLeft(b + F(c, d, a) + x[11] + 0x895cd7be, S14) + c;

        a = BitOperations.RotateLeft(a + F(b, c, d) + x[12] + 0x6b901122, S11) + b;
        d = BitOperations.RotateLeft(d + F(a, b, c) + x[13] + 0xfd987193, S12) + a;
        c = BitOperations.RotateLeft(c + F(d, a, b) + x[14] + 0xa679438e, S13) + d;
        b = BitOperations.RotateLeft(b + F(c, d, a) + x[15] + 0x49b40821, S14) + c;

        #endregion

        #region Round 2: G cycle × 16

        const int S21 = 5;
        const int S22 = 9;
        const int S23 = 14;
        const int S24 = 20;

        a = BitOperations.RotateLeft(a + G(b, c, d) + x[1] + 0xf61e2562, S21) + b;
        d = BitOperations.RotateLeft(d + G(a, b, c) + x[6] + 0xc040b340, S22) + a;
        c = BitOperations.RotateLeft(c + G(d, a, b) + x[11] + 0x265e5a51, S23) + d;
        b = BitOperations.RotateLeft(b + G(c, d, a) + x[0] + 0xe9b6c7aa, S24) + c;

        a = BitOperations.RotateLeft(a + G(b, c, d) + x[5] + 0xd62f105d, S21) + b;
        d = BitOperations.RotateLeft(d + G(a, b, c) + x[10] + 0x02441453, S22) + a;
        c = BitOperations.RotateLeft(c + G(d, a, b) + x[15] + 0xd8a1e681, S23) + d;
        b = BitOperations.RotateLeft(b + G(c, d, a) + x[4] + 0xe7d3fbc8, S24) + c;

        a = BitOperations.RotateLeft(a + G(b, c, d) + x[9] + 0x21e1cde6, S21) + b;
        d = BitOperations.RotateLeft(d + G(a, b, c) + x[14] + 0xc33707d6, S22) + a;
        c = BitOperations.RotateLeft(c + G(d, a, b) + x[3] + 0xf4d50d87, S23) + d;
        b = BitOperations.RotateLeft(b + G(c, d, a) + x[8] + 0x455a14ed, S24) + c;

        a = BitOperations.RotateLeft(a + G(b, c, d) + x[13] + 0xa9e3e905, S21) + b;
        d = BitOperations.RotateLeft(d + G(a, b, c) + x[2] + 0xfcefa3f8, S22) + a;
        c = BitOperations.RotateLeft(c + G(d, a, b) + x[7] + 0x676f02d9, S23) + d;
        b = BitOperations.RotateLeft(b + G(c, d, a) + x[12] + 0x8d2a4c8a, S24) + c;

        #endregion

        #region Round 3: H cycle × 16

        const int S31 = 4;
        const int S32 = 11;
        const int S33 = 16;
        const int S34 = 23;

        a = BitOperations.RotateLeft(a + H(b, c, d) + x[5] + 0xfffa3942, S31) + b;
        d = BitOperations.RotateLeft(d + H(a, b, c) + x[8] + 0x8771f681, S32) + a;
        c = BitOperations.RotateLeft(c + H(d, a, b) + x[11] + 0x6d9d6122, S33) + d;
        b = BitOperations.RotateLeft(b + H(c, d, a) + x[14] + 0xfde5380c, S34) + c;

        a = BitOperations.RotateLeft(a + H(b, c, d) + x[1] + 0xa4beea44, S31) + b;
        d = BitOperations.RotateLeft(d + H(a, b, c) + x[4] + 0x4bdecfa9, S32) + a;
        c = BitOperations.RotateLeft(c + H(d, a, b) + x[7] + 0xf6bb4b60, S33) + d;
        b = BitOperations.RotateLeft(b + H(c, d, a) + x[10] + 0xbebfbc70, S34) + c;

        a = BitOperations.RotateLeft(a + H(b, c, d) + x[13] + 0x289b7ec6, S31) + b;
        d = BitOperations.RotateLeft(d + H(a, b, c) + x[0] + 0xeaa127fa, S32) + a;
        c = BitOperations.RotateLeft(c + H(d, a, b) + x[3] + 0xd4ef3085, S33) + d;
        b = BitOperations.RotateLeft(b + H(c, d, a) + x[6] + 0x04881d05, S34) + c;

        a = BitOperations.RotateLeft(a + H(b, c, d) + x[9] + 0xd9d4d039, S31) + b;
        d = BitOperations.RotateLeft(d + H(a, b, c) + x[12] + 0xe6db99e5, S32) + a;
        c = BitOperations.RotateLeft(c + H(d, a, b) + x[15] + 0x1fa27cf8, S33) + d;
        b = BitOperations.RotateLeft(b + H(c, d, a) + x[2] + 0xc4ac5665, S34) + c;

        #endregion

        #region Round 4: K cycle × 16

        const int S41 = 6;
        const int S42 = 10;
        const int S43 = 15;
        const int S44 = 21;

        a = BitOperations.RotateLeft(a + K(b, c, d) + x[0] + 0xf4292244, S41) + b;
        d = BitOperations.RotateLeft(d + K(a, b, c) + x[7] + 0x432aff97, S42) + a;
        c = BitOperations.RotateLeft(c + K(d, a, b) + x[14] + 0xab9423a7, S43) + d;
        b = BitOperations.RotateLeft(b + K(c, d, a) + x[5] + 0xfc93a039, S44) + c;

        a = BitOperations.RotateLeft(a + K(b, c, d) + x[12] + 0x655b59c3, S41) + b;
        d = BitOperations.RotateLeft(d + K(a, b, c) + x[3] + 0x8f0ccc92, S42) + a;
        c = BitOperations.RotateLeft(c + K(d, a, b) + x[10] + 0xffeff47d, S43) + d;
        b = BitOperations.RotateLeft(b + K(c, d, a) + x[1] + 0x85845dd1, S44) + c;

        a = BitOperations.RotateLeft(a + K(b, c, d) + x[8] + 0x6fa87e4f, S41) + b;
        d = BitOperations.RotateLeft(d + K(a, b, c) + x[15] + 0xfe2ce6e0, S42) + a;
        c = BitOperations.RotateLeft(c + K(d, a, b) + x[6] + 0xa3014314, S43) + d;
        b = BitOperations.RotateLeft(b + K(c, d, a) + x[13] + 0x4e0811a1, S44) + c;

        a = BitOperations.RotateLeft(a + K(b, c, d) + x[4] + 0xf7537e82, S41) + b;
        d = BitOperations.RotateLeft(d + K(a, b, c) + x[11] + 0xbd3af235, S42) + a;
        c = BitOperations.RotateLeft(c + K(d, a, b) + x[2] + 0x2ad7d2bb, S43) + d;
        b = BitOperations.RotateLeft(b + K(c, d, a) + x[9] + 0xeb86d391, S44) + c;

        #endregion

        m_H1 += a;
        m_H2 += b;
        m_H3 += c;
        m_H4 += d;
    }

    static uint F(uint u, uint v, uint w) => (u & v) | (~u & w);

    static uint G(uint u, uint v, uint w) => (u & w) | (v & ~w);

    static uint H(uint u, uint v, uint w) => u ^ v ^ w;

    static uint K(uint u, uint v, uint w) => v ^ (u | ~w);

    /// <summary>
    /// Counts how much data we have stored that still needs processed.
    /// </summary>
    int m_ProcessingBufferCount;

    byte[] GetProcessingBuffer()
    {
        byte[]? buffer = m_ProcessingBuffer;
        ObjectDisposedException.ThrowIf(buffer is null, this);
        return buffer;
    }

    /// <summary>
    /// Used to start data when passed less than a block worth.
    /// </summary>
    byte[]? m_ProcessingBuffer;

    ulong m_Count;
    uint m_H1, m_H2, m_H3, m_H4;

    const int BlockSizeInBytes = 64;
}
