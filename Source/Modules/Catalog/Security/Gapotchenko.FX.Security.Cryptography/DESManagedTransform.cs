// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
// Portions © The Legion of the Bouncy Castle Inc.
// Portions © James Gillogly
// Portions © Phil Karn
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using System.Buffers.Binary;

namespace Gapotchenko.FX.Security.Cryptography;

sealed class DESManagedTransform(byte[] key, bool encrypting) : ICryptoTransform
{
    const int BlockSize = 8;

    public int InputBlockSize => BlockSize;

    public int OutputBlockSize => BlockSize;

    public bool CanTransformMultipleBlocks => true;

    public bool CanReuseTransform => true;

    public void Dispose()
    {
        if (m_WorkingKey is { } workingKey)
        {
            // Revoke the reference as quickly as possible to indicate that the object has been disposed.
            m_WorkingKey = null;

            // Zero the state to avoid cryptographic material leaking.
            CryptographicOperations.ZeroMemory(workingKey);
        }
    }

    public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
    {
        TransformCore(
            inputBuffer.AsSpan(inputOffset, inputCount),
            outputBuffer.AsSpan(outputOffset));

        return BlockSize;
    }

    public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
    {
        if (inputCount == 0)
            return [];

        byte[] output = new byte[BlockSize];
        TransformCore(inputBuffer.AsSpan(inputOffset, inputCount), output);
        return output;
    }

    #region Transform Core

    void TransformCore(ReadOnlySpan<byte> input, Span<byte> output)
    {
        int[] workingKey = GetWorkingKey();

        uint hi32 = BinaryPrimitives.ReadUInt32BigEndian(input);
        uint lo32 = BinaryPrimitives.ReadUInt32BigEndian(input[4..]);

        DesFunc(workingKey, ref hi32, ref lo32);

        BinaryPrimitives.WriteUInt32BigEndian(output, hi32);
        BinaryPrimitives.WriteUInt32BigEndian(output[4..], lo32);
    }

    static void DesFunc(int[] wKey, ref uint hi32, ref uint lo32)
    {
        uint left = hi32;
        uint right = lo32;
        uint work;

        work = ((left >> 4) ^ right) & 0x0f0f0f0f;
        right ^= work;
        left ^= (work << 4);
        work = ((left >> 16) ^ right) & 0x0000ffff;
        right ^= work;
        left ^= (work << 16);
        work = ((right >> 2) ^ left) & 0x33333333;
        left ^= work;
        right ^= (work << 2);
        work = ((right >> 8) ^ left) & 0x00ff00ff;
        left ^= work;
        right ^= (work << 8);
        right = (right << 1) | (right >> 31);
        work = (left ^ right) & 0xaaaaaaaa;
        left ^= work;
        right ^= work;
        left = (left << 1) | (left >> 31);

        for (int round = 0; round < 8; round++)
        {
            uint fval;

            work = (right << 28) | (right >> 4);
            work ^= (uint)wKey[round * 4 + 0];
            fval = m_SP7[work & 0x3f];
            fval |= m_SP5[(work >> 8) & 0x3f];
            fval |= m_SP3[(work >> 16) & 0x3f];
            fval |= m_SP1[(work >> 24) & 0x3f];
            work = right ^ (uint)wKey[round * 4 + 1];
            fval |= m_SP8[work & 0x3f];
            fval |= m_SP6[(work >> 8) & 0x3f];
            fval |= m_SP4[(work >> 16) & 0x3f];
            fval |= m_SP2[(work >> 24) & 0x3f];
            left ^= fval;
            work = (left << 28) | (left >> 4);
            work ^= (uint)wKey[round * 4 + 2];
            fval = m_SP7[work & 0x3f];
            fval |= m_SP5[(work >> 8) & 0x3f];
            fval |= m_SP3[(work >> 16) & 0x3f];
            fval |= m_SP1[(work >> 24) & 0x3f];
            work = left ^ (uint)wKey[round * 4 + 3];
            fval |= m_SP8[work & 0x3f];
            fval |= m_SP6[(work >> 8) & 0x3f];
            fval |= m_SP4[(work >> 16) & 0x3f];
            fval |= m_SP2[(work >> 24) & 0x3f];
            right ^= fval;
        }

        right = (right << 31) | (right >> 1);
        work = (left ^ right) & 0xaaaaaaaa;
        left ^= work;
        right ^= work;
        left = (left << 31) | (left >> 1);
        work = ((left >> 8) ^ right) & 0x00ff00ff;
        right ^= work;
        left ^= (work << 8);
        work = ((left >> 2) ^ right) & 0x33333333;
        right ^= work;
        left ^= (work << 2);
        work = ((right >> 16) ^ left) & 0x0000ffff;
        left ^= work;
        right ^= (work << 16);
        work = ((right >> 4) ^ left) & 0x0f0f0f0f;
        left ^= work;
        right ^= (work << 4);

        hi32 = right;
        lo32 = left;
    }

    static readonly uint[] m_SP1 =
    [
        0x01010400, 0x00000000, 0x00010000, 0x01010404,
        0x01010004, 0x00010404, 0x00000004, 0x00010000,
        0x00000400, 0x01010400, 0x01010404, 0x00000400,
        0x01000404, 0x01010004, 0x01000000, 0x00000004,
        0x00000404, 0x01000400, 0x01000400, 0x00010400,
        0x00010400, 0x01010000, 0x01010000, 0x01000404,
        0x00010004, 0x01000004, 0x01000004, 0x00010004,
        0x00000000, 0x00000404, 0x00010404, 0x01000000,
        0x00010000, 0x01010404, 0x00000004, 0x01010000,
        0x01010400, 0x01000000, 0x01000000, 0x00000400,
        0x01010004, 0x00010000, 0x00010400, 0x01000004,
        0x00000400, 0x00000004, 0x01000404, 0x00010404,
        0x01010404, 0x00010004, 0x01010000, 0x01000404,
        0x01000004, 0x00000404, 0x00010404, 0x01010400,
        0x00000404, 0x01000400, 0x01000400, 0x00000000,
        0x00010004, 0x00010400, 0x00000000, 0x01010004
    ];

    static readonly uint[] m_SP2 =
    [
        0x80108020, 0x80008000, 0x00008000, 0x00108020,
        0x00100000, 0x00000020, 0x80100020, 0x80008020,
        0x80000020, 0x80108020, 0x80108000, 0x80000000,
        0x80008000, 0x00100000, 0x00000020, 0x80100020,
        0x00108000, 0x00100020, 0x80008020, 0x00000000,
        0x80000000, 0x00008000, 0x00108020, 0x80100000,
        0x00100020, 0x80000020, 0x00000000, 0x00108000,
        0x00008020, 0x80108000, 0x80100000, 0x00008020,
        0x00000000, 0x00108020, 0x80100020, 0x00100000,
        0x80008020, 0x80100000, 0x80108000, 0x00008000,
        0x80100000, 0x80008000, 0x00000020, 0x80108020,
        0x00108020, 0x00000020, 0x00008000, 0x80000000,
        0x00008020, 0x80108000, 0x00100000, 0x80000020,
        0x00100020, 0x80008020, 0x80000020, 0x00100020,
        0x00108000, 0x00000000, 0x80008000, 0x00008020,
        0x80000000, 0x80100020, 0x80108020, 0x00108000
    ];

    static readonly uint[] m_SP3 =
    [
        0x00000208, 0x08020200, 0x00000000, 0x08020008,
        0x08000200, 0x00000000, 0x00020208, 0x08000200,
        0x00020008, 0x08000008, 0x08000008, 0x00020000,
        0x08020208, 0x00020008, 0x08020000, 0x00000208,
        0x08000000, 0x00000008, 0x08020200, 0x00000200,
        0x00020200, 0x08020000, 0x08020008, 0x00020208,
        0x08000208, 0x00020200, 0x00020000, 0x08000208,
        0x00000008, 0x08020208, 0x00000200, 0x08000000,
        0x08020200, 0x08000000, 0x00020008, 0x00000208,
        0x00020000, 0x08020200, 0x08000200, 0x00000000,
        0x00000200, 0x00020008, 0x08020208, 0x08000200,
        0x08000008, 0x00000200, 0x00000000, 0x08020008,
        0x08000208, 0x00020000, 0x08000000, 0x08020208,
        0x00000008, 0x00020208, 0x00020200, 0x08000008,
        0x08020000, 0x08000208, 0x00000208, 0x08020000,
        0x00020208, 0x00000008, 0x08020008, 0x00020200
    ];

    static readonly uint[] m_SP4 =
    [
        0x00802001, 0x00002081, 0x00002081, 0x00000080,
        0x00802080, 0x00800081, 0x00800001, 0x00002001,
        0x00000000, 0x00802000, 0x00802000, 0x00802081,
        0x00000081, 0x00000000, 0x00800080, 0x00800001,
        0x00000001, 0x00002000, 0x00800000, 0x00802001,
        0x00000080, 0x00800000, 0x00002001, 0x00002080,
        0x00800081, 0x00000001, 0x00002080, 0x00800080,
        0x00002000, 0x00802080, 0x00802081, 0x00000081,
        0x00800080, 0x00800001, 0x00802000, 0x00802081,
        0x00000081, 0x00000000, 0x00000000, 0x00802000,
        0x00002080, 0x00800080, 0x00800081, 0x00000001,
        0x00802001, 0x00002081, 0x00002081, 0x00000080,
        0x00802081, 0x00000081, 0x00000001, 0x00002000,
        0x00800001, 0x00002001, 0x00802080, 0x00800081,
        0x00002001, 0x00002080, 0x00800000, 0x00802001,
        0x00000080, 0x00800000, 0x00002000, 0x00802080
    ];

    static readonly uint[] m_SP5 =
    [
        0x00000100, 0x02080100, 0x02080000, 0x42000100,
        0x00080000, 0x00000100, 0x40000000, 0x02080000,
        0x40080100, 0x00080000, 0x02000100, 0x40080100,
        0x42000100, 0x42080000, 0x00080100, 0x40000000,
        0x02000000, 0x40080000, 0x40080000, 0x00000000,
        0x40000100, 0x42080100, 0x42080100, 0x02000100,
        0x42080000, 0x40000100, 0x00000000, 0x42000000,
        0x02080100, 0x02000000, 0x42000000, 0x00080100,
        0x00080000, 0x42000100, 0x00000100, 0x02000000,
        0x40000000, 0x02080000, 0x42000100, 0x40080100,
        0x02000100, 0x40000000, 0x42080000, 0x02080100,
        0x40080100, 0x00000100, 0x02000000, 0x42080000,
        0x42080100, 0x00080100, 0x42000000, 0x42080100,
        0x02080000, 0x00000000, 0x40080000, 0x42000000,
        0x00080100, 0x02000100, 0x40000100, 0x00080000,
        0x00000000, 0x40080000, 0x02080100, 0x40000100
    ];

    static readonly uint[] m_SP6 =
    [
        0x20000010, 0x20400000, 0x00004000, 0x20404010,
        0x20400000, 0x00000010, 0x20404010, 0x00400000,
        0x20004000, 0x00404010, 0x00400000, 0x20000010,
        0x00400010, 0x20004000, 0x20000000, 0x00004010,
        0x00000000, 0x00400010, 0x20004010, 0x00004000,
        0x00404000, 0x20004010, 0x00000010, 0x20400010,
        0x20400010, 0x00000000, 0x00404010, 0x20404000,
        0x00004010, 0x00404000, 0x20404000, 0x20000000,
        0x20004000, 0x00000010, 0x20400010, 0x00404000,
        0x20404010, 0x00400000, 0x00004010, 0x20000010,
        0x00400000, 0x20004000, 0x20000000, 0x00004010,
        0x20000010, 0x20404010, 0x00404000, 0x20400000,
        0x00404010, 0x20404000, 0x00000000, 0x20400010,
        0x00000010, 0x00004000, 0x20400000, 0x00404010,
        0x00004000, 0x00400010, 0x20004010, 0x00000000,
        0x20404000, 0x20000000, 0x00400010, 0x20004010
    ];

    static readonly uint[] m_SP7 =
    [
        0x00200000, 0x04200002, 0x04000802, 0x00000000,
        0x00000800, 0x04000802, 0x00200802, 0x04200800,
        0x04200802, 0x00200000, 0x00000000, 0x04000002,
        0x00000002, 0x04000000, 0x04200002, 0x00000802,
        0x04000800, 0x00200802, 0x00200002, 0x04000800,
        0x04000002, 0x04200000, 0x04200800, 0x00200002,
        0x04200000, 0x00000800, 0x00000802, 0x04200802,
        0x00200800, 0x00000002, 0x04000000, 0x00200800,
        0x04000000, 0x00200800, 0x00200000, 0x04000802,
        0x04000802, 0x04200002, 0x04200002, 0x00000002,
        0x00200002, 0x04000000, 0x04000800, 0x00200000,
        0x04200800, 0x00000802, 0x00200802, 0x04200800,
        0x00000802, 0x04000002, 0x04200802, 0x04200000,
        0x00200800, 0x00000000, 0x00000002, 0x04200802,
        0x00000000, 0x00200802, 0x04200000, 0x00000800,
        0x04000002, 0x04000800, 0x00000800, 0x00200002
    ];

    static readonly uint[] m_SP8 =
    [
        0x10001040, 0x00001000, 0x00040000, 0x10041040,
        0x10000000, 0x10001040, 0x00000040, 0x10000000,
        0x00040040, 0x10040000, 0x10041040, 0x00041000,
        0x10041000, 0x00041040, 0x00001000, 0x00000040,
        0x10040000, 0x10000040, 0x10001000, 0x00001040,
        0x00041000, 0x00040040, 0x10040040, 0x10041000,
        0x00001040, 0x00000000, 0x00000000, 0x10040040,
        0x10000040, 0x10001000, 0x00041040, 0x00040000,
        0x00041040, 0x00040000, 0x10041000, 0x00001000,
        0x00000040, 0x10040040, 0x00001000, 0x00041040,
        0x10001000, 0x00000040, 0x10000040, 0x10040000,
        0x10040040, 0x10000000, 0x00040000, 0x10001040,
        0x00000000, 0x10041040, 0x00040040, 0x10000040,
        0x10040000, 0x10001000, 0x10001040, 0x00000000,
        0x10041040, 0x00041000, 0x00041000, 0x00001040,
        0x00001040, 0x00040040, 0x10000000, 0x10041000
    ];

    #endregion

    #region Working Key

    int[] GetWorkingKey()
    {
        int[]? workingKey = m_WorkingKey;
        ObjectDisposedException.ThrowIf(workingKey is null, this);
        return workingKey;
    }

    /// <summary>
    /// The expanded DES round key schedule.
    /// </summary>
    int[]? m_WorkingKey = CreateWorkingKey(key, encrypting);

    /// <remarks>
    /// Acknowledgements for this routine go to James Gillogly and Phil Karn.
    /// </remarks>
    static int[] CreateWorkingKey(byte[] key, bool encrypting)
    {
        int[] newKey = new int[32];
        Span<bool> pc1m = stackalloc bool[56];
        Span<bool> pcr = stackalloc bool[56];

        for (int j = 0; j < 56; j++)
        {
            int l = m_PC1[j];
            pc1m[j] = ((key[(uint)l >> 3] & m_ByteBit[l & 07]) != 0);
        }

        for (int i = 0; i < 16; i++)
        {
            int l, m, n;

            if (encrypting)
                m = i << 1;
            else
                m = (15 - i) << 1;

            n = m + 1;
            newKey[m] = newKey[n] = 0;

            for (int j = 0; j < 28; j++)
            {
                l = j + m_TotRot[i];
                pcr[j] = l < 28 ? pc1m[l] : pc1m[l - 28];
            }

            for (int j = 28; j < 56; j++)
            {
                l = j + m_TotRot[i];
                pcr[j] = l < 56 ? pc1m[l] : pc1m[l - 28];
            }

            for (int j = 0; j < 24; j++)
            {
                if (pcr[m_PC2[j]])
                    newKey[m] |= m_BigByte[j];

                if (pcr[m_PC2[j + 24]])
                    newKey[n] |= m_BigByte[j];
            }
        }

        // Store the processed key
        for (int i = 0; i != 32; i += 2)
        {
            int i1, i2;

            i1 = newKey[i];
            i2 = newKey[i + 1];

            newKey[i] = (int)((uint)((i1 & 0x00fc0000) << 6) |
                              (uint)((i1 & 0x00000fc0) << 10) |
                              ((uint)(i2 & 0x00fc0000) >> 10) |
                              ((uint)(i2 & 0x00000fc0) >> 6));

            newKey[i + 1] = (int)((uint)((i1 & 0x0003f000) << 12) |
                                  (uint)((i1 & 0x0000003f) << 16) |
                                  ((uint)(i2 & 0x0003f000) >> 4) |
                                  (uint)(i2 & 0x0000003f));
        }

        return newKey;
    }

    static readonly short[] m_ByteBit = [128, 64, 32, 16, 8, 4, 2, 1];

    static readonly int[] m_BigByte =
    [
        0x800000,  0x400000,  0x200000,  0x100000,
        0x80000,   0x40000,   0x20000,   0x10000,
        0x8000,    0x4000,    0x2000,    0x1000,
        0x800,     0x400,     0x200,     0x100,
        0x80,      0x40,      0x20,      0x10,
        0x8,       0x4,       0x2,       0x1
    ];

    // Use the key schedule specified in the Standard (ANSI X3.92-1981).
    static readonly byte[] m_PC1 =
    [
        56, 48, 40, 32, 24, 16,  8,  0, 57, 49, 41, 33, 25, 17,
        9,  1,  58, 50, 42, 34, 26,  18, 10,  2, 59, 51, 43, 35,
        62, 54, 46, 38, 30, 22, 14,   6, 61, 53, 45, 37, 29, 21,
        13,  5, 60, 52, 44, 36, 28,  20, 12,  4, 27, 19, 11,  3
    ];

    static readonly byte[] m_TotRot =
    [
        1, 2, 4, 6, 8, 10, 12, 14,
        15, 17, 19, 21, 23, 25, 27, 28
    ];

    static readonly byte[] m_PC2 =
    [
        13, 16, 10, 23,  0,  4,  2, 27, 14,  5, 20,  9,
        22, 18, 11,  3, 25,  7, 15,  6, 26, 19, 12,  1,
        40, 51, 30, 36, 46, 54, 29, 39, 50, 44, 32, 47,
        43, 48, 38, 55, 33, 52, 45, 41, 49, 35, 28, 31
    ];

    #endregion
}
