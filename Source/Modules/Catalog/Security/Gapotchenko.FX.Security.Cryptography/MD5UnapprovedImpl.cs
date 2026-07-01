// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
// Portions © The Mono Project
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

#pragma warning disable CA5351 // Do Not Use Broken Cryptographic Algorithms

using System.Buffers.Binary;

namespace Gapotchenko.FX.Security.Cryptography;

sealed class MD5UnapprovedImpl : MD5
{
    public MD5UnapprovedImpl()
    {
        m_H = new uint[4];
        m_ProcessingBuffer = new byte[BLOCK_SIZE_BYTES];

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

            CryptographicOperations.ZeroMemory(m_H);
        }

        base.Dispose(disposing);
    }

    public override void Initialize()
    {
        m_Count = 0;
        m_ProcessingBufferCount = 0;

        m_H[0] = 0x67452301;
        m_H[1] = 0xefcdab89;
        m_H[2] = 0x98badcfe;
        m_H[3] = 0x10325476;
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
            int j = BLOCK_SIZE_BYTES - m_ProcessingBufferCount;
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
        int blockRem = cbSize % BLOCK_SIZE_BYTES;
        int blockEnd = cbSize - blockRem;

        for (int i = 0; i < blockEnd; i += BLOCK_SIZE_BYTES)
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
        for (int i = 0; i < 4; i++)
            BinaryPrimitives.WriteUInt32LittleEndian(hash.AsSpan(i * 4), m_H[i]);

        return hash;
    }

    void ProcessFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
    {
        ulong total = m_Count + (ulong)inputCount;
        int paddingSize = (int)(56 - total % BLOCK_SIZE_BYTES);

        if (paddingSize < 1)
            paddingSize += BLOCK_SIZE_BYTES;

        byte[] fooBuffer = new byte[inputCount + paddingSize + 8];

        for (int i = 0; i < inputCount; i++)
            fooBuffer[i] = inputBuffer[i + inputOffset];

        fooBuffer[inputCount] = 0x80;
        for (int i = inputCount + 1; i < inputCount + paddingSize; i++)
            fooBuffer[i] = 0x00;

        // The algorithm uses bits, not bytes.
        ulong size = total * 8;
        BinaryPrimitives.WriteUInt64LittleEndian(fooBuffer.AsSpan(inputCount + paddingSize), size);
        ProcessBlock(fooBuffer);

        if (inputCount + paddingSize + 8 == 128)
            ProcessBlock(fooBuffer.AsSpan(64));
    }

    void ProcessBlock(ReadOnlySpan<byte> inputBuffer)
    {
        m_Count += BLOCK_SIZE_BYTES;

        Span<uint> x = stackalloc uint[16];
        for (int i = 0; i < 16; i++)
            x[i] = BinaryPrimitives.ReadUInt32LittleEndian(inputBuffer[(4 * i)..]);

        uint a = m_H[0];
        uint b = m_H[1];
        uint c = m_H[2];
        uint d = m_H[3];

        // This function was unrolled because it seems to be doubling our performance with current compiler/VM.
        // Possibly roll up if this changes.

        // ---- Round 1 --------

        a += (((c ^ d) & b) ^ d) + (uint)m_K[0] + x[0];
        a = (a << 7) | (a >> 25);
        a += b;

        d += (((b ^ c) & a) ^ c) + (uint)m_K[1] + x[1];
        d = (d << 12) | (d >> 20);
        d += a;

        c += (((a ^ b) & d) ^ b) + (uint)m_K[2] + x[2];
        c = (c << 17) | (c >> 15);
        c += d;

        b += (((d ^ a) & c) ^ a) + (uint)m_K[3] + x[3];
        b = (b << 22) | (b >> 10);
        b += c;

        a += (((c ^ d) & b) ^ d) + (uint)m_K[4] + x[4];
        a = (a << 7) | (a >> 25);
        a += b;

        d += (((b ^ c) & a) ^ c) + (uint)m_K[5] + x[5];
        d = (d << 12) | (d >> 20);
        d += a;

        c += (((a ^ b) & d) ^ b) + (uint)m_K[6] + x[6];
        c = (c << 17) | (c >> 15);
        c += d;

        b += (((d ^ a) & c) ^ a) + (uint)m_K[7] + x[7];
        b = (b << 22) | (b >> 10);
        b += c;

        a += (((c ^ d) & b) ^ d) + (uint)m_K[8] + x[8];
        a = (a << 7) | (a >> 25);
        a += b;

        d += (((b ^ c) & a) ^ c) + (uint)m_K[9] + x[9];
        d = (d << 12) | (d >> 20);
        d += a;

        c += (((a ^ b) & d) ^ b) + (uint)m_K[10] + x[10];
        c = (c << 17) | (c >> 15);
        c += d;

        b += (((d ^ a) & c) ^ a) + (uint)m_K[11] + x[11];
        b = (b << 22) | (b >> 10);
        b += c;

        a += (((c ^ d) & b) ^ d) + (uint)m_K[12] + x[12];
        a = (a << 7) | (a >> 25);
        a += b;

        d += (((b ^ c) & a) ^ c) + (uint)m_K[13] + x[13];
        d = (d << 12) | (d >> 20);
        d += a;

        c += (((a ^ b) & d) ^ b) + (uint)m_K[14] + x[14];
        c = (c << 17) | (c >> 15);
        c += d;

        b += (((d ^ a) & c) ^ a) + (uint)m_K[15] + x[15];
        b = (b << 22) | (b >> 10);
        b += c;


        // ---- Round 2 --------

        a += (((b ^ c) & d) ^ c) + (uint)m_K[16] + x[1];
        a = (a << 5) | (a >> 27);
        a += b;

        d += (((a ^ b) & c) ^ b) + (uint)m_K[17] + x[6];
        d = (d << 9) | (d >> 23);
        d += a;

        c += (((d ^ a) & b) ^ a) + (uint)m_K[18] + x[11];
        c = (c << 14) | (c >> 18);
        c += d;

        b += (((c ^ d) & a) ^ d) + (uint)m_K[19] + x[0];
        b = (b << 20) | (b >> 12);
        b += c;

        a += (((b ^ c) & d) ^ c) + (uint)m_K[20] + x[5];
        a = (a << 5) | (a >> 27);
        a += b;

        d += (((a ^ b) & c) ^ b) + (uint)m_K[21] + x[10];
        d = (d << 9) | (d >> 23);
        d += a;

        c += (((d ^ a) & b) ^ a) + (uint)m_K[22] + x[15];
        c = (c << 14) | (c >> 18);
        c += d;

        b += (((c ^ d) & a) ^ d) + (uint)m_K[23] + x[4];
        b = (b << 20) | (b >> 12);
        b += c;

        a += (((b ^ c) & d) ^ c) + (uint)m_K[24] + x[9];
        a = (a << 5) | (a >> 27);
        a += b;

        d += (((a ^ b) & c) ^ b) + (uint)m_K[25] + x[14];
        d = (d << 9) | (d >> 23);
        d += a;

        c += (((d ^ a) & b) ^ a) + (uint)m_K[26] + x[3];
        c = (c << 14) | (c >> 18);
        c += d;

        b += (((c ^ d) & a) ^ d) + (uint)m_K[27] + x[8];
        b = (b << 20) | (b >> 12);
        b += c;

        a += (((b ^ c) & d) ^ c) + (uint)m_K[28] + x[13];
        a = (a << 5) | (a >> 27);
        a += b;

        d += (((a ^ b) & c) ^ b) + (uint)m_K[29] + x[2];
        d = (d << 9) | (d >> 23);
        d += a;

        c += (((d ^ a) & b) ^ a) + (uint)m_K[30] + x[7];
        c = (c << 14) | (c >> 18);
        c += d;

        b += (((c ^ d) & a) ^ d) + (uint)m_K[31] + x[12];
        b = (b << 20) | (b >> 12);
        b += c;


        // ---- Round 3 --------

        a += (b ^ c ^ d) + (uint)m_K[32] + x[5];
        a = (a << 4) | (a >> 28);
        a += b;

        d += (a ^ b ^ c) + (uint)m_K[33] + x[8];
        d = (d << 11) | (d >> 21);
        d += a;

        c += (d ^ a ^ b) + (uint)m_K[34] + x[11];
        c = (c << 16) | (c >> 16);
        c += d;

        b += (c ^ d ^ a) + (uint)m_K[35] + x[14];
        b = (b << 23) | (b >> 9);
        b += c;

        a += (b ^ c ^ d) + (uint)m_K[36] + x[1];
        a = (a << 4) | (a >> 28);
        a += b;

        d += (a ^ b ^ c) + (uint)m_K[37] + x[4];
        d = (d << 11) | (d >> 21);
        d += a;

        c += (d ^ a ^ b) + (uint)m_K[38] + x[7];
        c = (c << 16) | (c >> 16);
        c += d;

        b += (c ^ d ^ a) + (uint)m_K[39] + x[10];
        b = (b << 23) | (b >> 9);
        b += c;

        a += (b ^ c ^ d) + (uint)m_K[40] + x[13];
        a = (a << 4) | (a >> 28);
        a += b;

        d += (a ^ b ^ c) + (uint)m_K[41] + x[0];
        d = (d << 11) | (d >> 21);
        d += a;

        c += (d ^ a ^ b) + (uint)m_K[42] + x[3];
        c = (c << 16) | (c >> 16);
        c += d;

        b += (c ^ d ^ a) + (uint)m_K[43] + x[6];
        b = (b << 23) | (b >> 9);
        b += c;

        a += (b ^ c ^ d) + (uint)m_K[44] + x[9];
        a = (a << 4) | (a >> 28);
        a += b;

        d += (a ^ b ^ c) + (uint)m_K[45] + x[12];
        d = (d << 11) | (d >> 21);
        d += a;

        c += (d ^ a ^ b) + (uint)m_K[46] + x[15];
        c = (c << 16) | (c >> 16);
        c += d;

        b += (c ^ d ^ a) + (uint)m_K[47] + x[2];
        b = (b << 23) | (b >> 9);
        b += c;


        // ---- Round 4 --------

        a += (((~d) | b) ^ c) + (uint)m_K[48] + x[0];
        a = (a << 6) | (a >> 26);
        a += b;

        d += (((~c) | a) ^ b) + (uint)m_K[49] + x[7];
        d = (d << 10) | (d >> 22);
        d += a;

        c += (((~b) | d) ^ a) + (uint)m_K[50] + x[14];
        c = (c << 15) | (c >> 17);
        c += d;

        b += (((~a) | c) ^ d) + (uint)m_K[51] + x[5];
        b = (b << 21) | (b >> 11);
        b += c;

        a += (((~d) | b) ^ c) + (uint)m_K[52] + x[12];
        a = (a << 6) | (a >> 26);
        a += b;

        d += (((~c) | a) ^ b) + (uint)m_K[53] + x[3];
        d = (d << 10) | (d >> 22);
        d += a;

        c += (((~b) | d) ^ a) + (uint)m_K[54] + x[10];
        c = (c << 15) | (c >> 17);
        c += d;

        b += (((~a) | c) ^ d) + (uint)m_K[55] + x[1];
        b = (b << 21) | (b >> 11);
        b += c;

        a += (((~d) | b) ^ c) + (uint)m_K[56] + x[8];
        a = (a << 6) | (a >> 26);
        a += b;

        d += (((~c) | a) ^ b) + (uint)m_K[57] + x[15];
        d = (d << 10) | (d >> 22);
        d += a;

        c += (((~b) | d) ^ a) + (uint)m_K[58] + x[6];
        c = (c << 15) | (c >> 17);
        c += d;

        b += (((~a) | c) ^ d) + (uint)m_K[59] + x[13];
        b = (b << 21) | (b >> 11);
        b += c;

        a += (((~d) | b) ^ c) + (uint)m_K[60] + x[4];
        a = (a << 6) | (a >> 26);
        a += b;

        d += (((~c) | a) ^ b) + (uint)m_K[61] + x[11];
        d = (d << 10) | (d >> 22);
        d += a;

        c += (((~b) | d) ^ a) + (uint)m_K[62] + x[2];
        c = (c << 15) | (c >> 17);
        c += d;

        b += (((~a) | c) ^ d) + (uint)m_K[63] + x[9];
        b = (b << 21) | (b >> 11);
        b += c;

        m_H[0] += a;
        m_H[1] += b;
        m_H[2] += c;
        m_H[3] += d;
    }

    static readonly uint[] m_K = {
        0xd76aa478, 0xe8c7b756, 0x242070db, 0xc1bdceee,
        0xf57c0faf, 0x4787c62a, 0xa8304613, 0xfd469501,
        0x698098d8, 0x8b44f7af, 0xffff5bb1, 0x895cd7be,
        0x6b901122, 0xfd987193, 0xa679438e, 0x49b40821,
        0xf61e2562, 0xc040b340, 0x265e5a51, 0xe9b6c7aa,
        0xd62f105d, 0x02441453, 0xd8a1e681, 0xe7d3fbc8,
        0x21e1cde6, 0xc33707d6, 0xf4d50d87, 0x455a14ed,
        0xa9e3e905, 0xfcefa3f8, 0x676f02d9, 0x8d2a4c8a,
        0xfffa3942, 0x8771f681, 0x6d9d6122, 0xfde5380c,
        0xa4beea44, 0x4bdecfa9, 0xf6bb4b60, 0xbebfbc70,
        0x289b7ec6, 0xeaa127fa, 0xd4ef3085, 0x04881d05,
        0xd9d4d039, 0xe6db99e5, 0x1fa27cf8, 0xc4ac5665,
        0xf4292244, 0x432aff97, 0xab9423a7, 0xfc93a039,
        0x655b59c3, 0x8f0ccc92, 0xffeff47d, 0x85845dd1,
        0x6fa87e4f, 0xfe2ce6e0, 0xa3014314, 0x4e0811a1,
        0xf7537e82, 0xbd3af235, 0x2ad7d2bb, 0xeb86d391
    };

    const int BLOCK_SIZE_BYTES = 64;

    readonly uint[] m_H;
    ulong m_Count;

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
}
