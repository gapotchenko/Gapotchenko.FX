// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using Gapotchenko.FX.Numerics;
using Gapotchenko.FX.Security.Cryptography.Utils;
using System.Buffers.Binary;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Numerics;

namespace Gapotchenko.FX.Security.Cryptography;

sealed class RSAUnapprovedImpl : RSA
{
    public RSAUnapprovedImpl()
    {
        LegalKeySizesValue = [new KeySizes(384, 16384, 8)];
        KeySizeValue = GetDefaultKeySize();
    }

    static int GetDefaultKeySize()
    {
#if NETCOREAPP
        return 2048;
#elif NETFRAMEWORK
        return 1024;
#else
        if (Environment.Version.Major >= 5)
            return 2048;
        else
            return 1024;
#endif
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ClearKey();
            m_Flags[F_Disposed] = true;
        }

        base.Dispose(disposing);
    }

    public override string? KeyExchangeAlgorithm => "RSA";

    public override string SignatureAlgorithm => "RSA";

    public override int KeySize
    {
        get => base.KeySize;
        set
        {
            int oldValue = KeySizeValue;
            base.KeySize = value;
            if (value != oldValue)
                ClearKey();
        }
    }

    /// <remarks>
    /// The method does not prove that <c>P</c> and <c>Q</c> are prime by design
    /// to avoid imposing substantial CPU costs.
    /// </remarks>
    public override void ImportParameters(RSAParameters parameters)
    {
        EnsureNotDisposed();

        ValidateRequiredParameter(parameters.Modulus, nameof(parameters.Modulus));

        int keyByteSize = parameters.Modulus.Length;
        int keySize = keyByteSize * 8;
        if (!IsValidKeySize(keySize))
            throw new CryptographicException("Specified key is not a valid size for this algorithm.");

        ValidateRequiredParameter(parameters.Exponent, nameof(parameters.Exponent));

        bool hasPrivateParameters =
            parameters.D is not null ||
            parameters.P is not null ||
            parameters.Q is not null ||
            parameters.DP is not null ||
            parameters.DQ is not null ||
            parameters.InverseQ is not null;

        if (hasPrivateParameters)
        {
            ValidateRequiredParameter(parameters.D, nameof(parameters.D));
            ValidateRequiredParameter(parameters.P, nameof(parameters.P));
            ValidateRequiredParameter(parameters.Q, nameof(parameters.Q));
            ValidateRequiredParameter(parameters.DP, nameof(parameters.DP));
            ValidateRequiredParameter(parameters.DQ, nameof(parameters.DQ));
            ValidateRequiredParameter(parameters.InverseQ, nameof(parameters.InverseQ));

            int primeByteSize = (keyByteSize + 1) / 2;
            if (parameters.D.Length != keyByteSize ||
                parameters.P.Length != primeByteSize ||
                parameters.Q.Length != primeByteSize ||
                parameters.DP.Length != primeByteSize ||
                parameters.DQ.Length != primeByteSize ||
                parameters.InverseQ.Length != primeByteSize)
            {
                throw InvalidParameters();
            }
        }

        var modulus = FromBytes(parameters.Modulus);
        var exponent = FromBytes(parameters.Exponent);
        var d = hasPrivateParameters ? FromBytes(parameters.D) : default;
        var p = hasPrivateParameters ? FromBytes(parameters.P) : default;
        var q = hasPrivateParameters ? FromBytes(parameters.Q) : default;
        var dp = hasPrivateParameters ? FromBytes(parameters.DP) : default;
        var dq = hasPrivateParameters ? FromBytes(parameters.DQ) : default;
        var inverseQ = hasPrivateParameters ? FromBytes(parameters.InverseQ) : default;

        ValidatePublicParameters(modulus, exponent, keySize);
        if (hasPrivateParameters)
            ValidatePrivateParameters(modulus, exponent, d, p, q, dp, dq, inverseQ);

        m_Modulus = modulus;
        m_Exponent = exponent;
        m_D = d;
        m_P = p;
        m_Q = q;
        m_DP = dp;
        m_DQ = dq;
        m_InverseQ = inverseQ;
        m_Flags[F_HasPrivateParameters] = hasPrivateParameters;
        m_Flags[F_HasKey] = true;
        KeySizeValue = keySize;
    }

    public override RSAParameters ExportParameters(bool includePrivateParameters)
    {
        EnsureNotDisposed();
        EnsureKey();

        if (includePrivateParameters && !m_Flags[F_HasPrivateParameters])
            throw PrivateKeyIsNotAvailable();

        int keyByteSize = KeyByteSize;
        int primeByteSize = (keyByteSize + 1) / 2;

        return
            new RSAParameters
            {
                Modulus = ToBytes(m_Modulus, keyByteSize),
                Exponent = ToBytes(m_Exponent),
                P = includePrivateParameters ? ToBytes(m_P, primeByteSize) : null,
                Q = includePrivateParameters ? ToBytes(m_Q, primeByteSize) : null,
                DP = includePrivateParameters ? ToBytes(m_DP, primeByteSize) : null,
                DQ = includePrivateParameters ? ToBytes(m_DQ, primeByteSize) : null,
                InverseQ = includePrivateParameters ? ToBytes(m_InverseQ, primeByteSize) : null,
                D = includePrivateParameters ? ToBytes(m_D, keyByteSize) : null
            };
    }

    public override byte[] Encrypt(byte[] data, RSAEncryptionPadding padding)
    {
        ArgumentNullException.ThrowIfNull(data);
        ArgumentNullException.ThrowIfNull(padding);

        EnsureNotDisposed();
        EnsureKey();
        byte[] encodedMessage;

        if (padding == RSAEncryptionPadding.Pkcs1)
            encodedMessage = EncodePkcs1Encryption(data, KeyByteSize);
        else if (padding.Mode == RSAEncryptionPaddingMode.Oaep)
            encodedMessage = EncodeOaep(data, KeyByteSize, padding.OaepHashAlgorithm);
        else
            throw PaddingModeNotSupported();

        return Transform(encodedMessage, m_Exponent, m_Modulus, KeyByteSize);
    }

    public override byte[] Decrypt(byte[] data, RSAEncryptionPadding padding)
    {
        ArgumentNullException.ThrowIfNull(data);
        ArgumentNullException.ThrowIfNull(padding);

        EnsureNotDisposed();
        EnsurePrivateKey();
        if (data.Length != KeyByteSize)
            throw new CryptographicException("Ciphertext length does not match the key size.");

        byte[] encodedMessage = TransformPrivate(data);

        if (padding == RSAEncryptionPadding.Pkcs1)
            return DecodePkcs1Encryption(encodedMessage);
        if (padding.Mode == RSAEncryptionPaddingMode.Oaep)
            return DecodeOaep(encodedMessage, padding.OaepHashAlgorithm);

        throw PaddingModeNotSupported();
    }

    public override byte[] SignHash(byte[] hash, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding)
    {
        ArgumentNullException.ThrowIfNull(hash);
        ArgumentNullException.ThrowIfNull(padding);
        ValidateHashAlgorithm(hashAlgorithm);

        EnsureNotDisposed();
        EnsurePrivateKey();
        byte[] encodedMessage;

        if (padding == RSASignaturePadding.Pkcs1)
            encodedMessage = EncodePkcs1Signature(hash, hashAlgorithm, KeyByteSize);
        else if (padding == RSASignaturePadding.Pss)
            encodedMessage = EncodePss(hash, hashAlgorithm, KeySize - 1, KeyByteSize);
        else
            throw PaddingModeNotSupported();

        return TransformPrivate(encodedMessage);
    }

    public override bool VerifyHash(byte[] hash, byte[] signature, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding)
    {
        ArgumentNullException.ThrowIfNull(hash);
        ArgumentNullException.ThrowIfNull(signature);
        ArgumentNullException.ThrowIfNull(padding);
        ValidateHashAlgorithm(hashAlgorithm);

        EnsureNotDisposed();

        if (signature.Length != KeyByteSize)
            return false;

        EnsureKey();
        byte[] encodedMessage;
        try
        {
            encodedMessage = Transform(signature, m_Exponent, m_Modulus, KeyByteSize);
        }
        catch (CryptographicException)
        {
            return false;
        }

        if (padding == RSASignaturePadding.Pkcs1)
        {
            byte[] expected = EncodePkcs1Signature(hash, hashAlgorithm, KeyByteSize);
            return CryptographicOperations.FixedTimeEquals(encodedMessage, expected);
        }
        if (padding == RSASignaturePadding.Pss)
        {
            return VerifyPss(hash, encodedMessage, hashAlgorithm, KeySize - 1);
        }

        throw PaddingModeNotSupported();
    }

    void EnsureKey()
    {
        if (!m_Flags[F_HasKey])
            GenerateKey();
    }

    void ClearKey()
    {
        m_Modulus = default;
        m_Exponent = default;
        m_P = default;
        m_Q = default;
        m_DP = default;
        m_DQ = default;
        m_InverseQ = default;
        m_D = default;

        m_Flags[F_HasKey | F_HasPrivateParameters] = false;
    }

    void EnsurePrivateKey()
    {
        EnsureKey();

        if (!m_Flags[F_HasPrivateParameters])
            throw PrivateKeyIsNotAvailable();
    }

    void GenerateKey()
    {
        int keySize = KeySize;
        BigInteger e = 65537;

        for (; ; )
        {
            var p = PrimeUtil.GeneratePrime(keySize / 2);
            BigInteger q;
            do
            {
                q = PrimeUtil.GeneratePrime(keySize - keySize / 2);
            }
            while (q == p);

            var n = p * q;
            if (n.GetBitLength() != keySize)
                continue;

            var pMinus1 = p - 1;
            var qMinus1 = q - 1;
            var phi = pMinus1 * qMinus1;
            if (BigInteger.GreatestCommonDivisor(e, phi) != 1)
                continue;

            var d = ModInverse(e, phi);
            var dp = d % pMinus1;
            var dq = d % qMinus1;
            var inverseQ = ModInverse(q, p);

            m_Modulus = n;
            m_Exponent = e;
            m_P = p;
            m_Q = q;
            m_D = d;
            m_DP = dp;
            m_DQ = dq;
            m_InverseQ = inverseQ;
            m_Flags[F_HasKey | F_HasPrivateParameters] = true;
            return;
        }
    }

    static byte[] EncodePkcs1Encryption(byte[] data, int keyByteSize)
    {
        int paddingLength = keyByteSize - data.Length - 3;
        if (paddingLength < 8)
            throw new CryptographicException("The data to be encrypted exceeds the maximum for this modulus.");

        byte[] encodedMessage = new byte[keyByteSize];
        encodedMessage[1] = 2;
        RngUtil.FillNonZero(encodedMessage.AsSpan(2, paddingLength));
        data.CopyTo(encodedMessage.AsSpan(keyByteSize - data.Length));
        return encodedMessage;
    }

    static byte[] DecodePkcs1Encryption(byte[] encodedMessage)
    {
        if (encodedMessage.Length < 11)
            throw new CryptographicException("Invalid PKCS#1 padding.");

        int valid = FixedTimeLogic.IsZero(encodedMessage[0]) & FixedTimeLogic.IsZero(encodedMessage[1] ^ 2);
        int separatorIndex = 0;
        int lookingForSeparator = 1;

        for (int i = 2; i < encodedMessage.Length; ++i)
        {
            int isSeparator = FixedTimeLogic.IsZero(encodedMessage[i]);
            separatorIndex = FixedTimeLogic.Select(lookingForSeparator & isSeparator, i, separatorIndex);
            lookingForSeparator &= 1 - isSeparator;
        }

        valid &= (1 - lookingForSeparator) & (1 - ((separatorIndex - 10 >> 31) & 1));
        if (valid == 0)
            throw new CryptographicException("Invalid PKCS#1 padding.");

        return encodedMessage.AsSpan(separatorIndex + 1).ToArray();
    }

    static byte[] EncodePkcs1Signature(byte[] hash, HashAlgorithmName hashAlgorithm, int keyByteSize)
    {
        var digestInfoPrefix = GetDigestInfoPrefix(hashAlgorithm);
        int hashLength = HashData(hashAlgorithm, []).Length;
        if (hash.Length != hashLength)
            throw new CryptographicException("Invalid hash length.");

        int digestInfoLength = digestInfoPrefix.Length + hash.Length;
        int paddingLength = keyByteSize - digestInfoLength - 3;
        if (paddingLength < 8)
            throw new CryptographicException("The hash value is too large for this key size.");

        byte[] encodedMessage = new byte[keyByteSize];
        encodedMessage[1] = 1;
        encodedMessage.AsSpan(2, paddingLength).Fill(0xff);
        digestInfoPrefix.CopyTo(encodedMessage.AsSpan(3 + paddingLength));
        hash.CopyTo(encodedMessage.AsSpan(keyByteSize - hash.Length));
        return encodedMessage;
    }

    static byte[] EncodeOaep(byte[] message, int keyByteSize, HashAlgorithmName hashAlgorithm)
    {
        byte[] labelHash = HashData(hashAlgorithm, []);
        int hashLength = labelHash.Length;
        if (message.Length > keyByteSize - 2 * hashLength - 2)
            throw new CryptographicException("The data to be encrypted exceeds the maximum for this modulus.");

        byte[] dataBlock = new byte[keyByteSize - hashLength - 1];
        labelHash.CopyTo(dataBlock, 0);
        dataBlock[dataBlock.Length - message.Length - 1] = 1;
        message.CopyTo(dataBlock.AsSpan(dataBlock.Length - message.Length));

        byte[] seed = RandomNumberGenerator.GetBytes(hashLength);
        byte[] dataBlockMask = Mgf1(seed, dataBlock.Length, hashAlgorithm);
        Xor(dataBlock, dataBlockMask);
        byte[] seedMask = Mgf1(dataBlock, hashLength, hashAlgorithm);
        Xor(seed, seedMask);

        byte[] encodedMessage = new byte[keyByteSize];
        seed.CopyTo(encodedMessage.AsSpan(1));
        dataBlock.CopyTo(encodedMessage.AsSpan(1 + hashLength));
        return encodedMessage;
    }

    static byte[] DecodeOaep(byte[] encodedMessage, HashAlgorithmName hashAlgorithm)
    {
        byte[] labelHash = HashData(hashAlgorithm, []);
        int hashLength = labelHash.Length;
        if (encodedMessage.Length < 2 * hashLength + 2)
            throw new CryptographicException("Invalid OAEP padding.");

        int valid = FixedTimeLogic.IsZero(encodedMessage[0]);

        byte[] seed = encodedMessage.AsSpan(1, hashLength).ToArray();
        byte[] dataBlock = encodedMessage.AsSpan(1 + hashLength).ToArray();

        byte[] seedMask = Mgf1(dataBlock, hashLength, hashAlgorithm);
        Xor(seed, seedMask);
        byte[] dataBlockMask = Mgf1(seed, dataBlock.Length, hashAlgorithm);
        Xor(dataBlock, dataBlockMask);

        valid &= CryptographicOperations.FixedTimeEquals(dataBlock.AsSpan(0, hashLength), labelHash) ? 1 : 0;

        int separatorIndex = 0;
        int lookingForSeparator = 1;
        for (int i = hashLength; i < dataBlock.Length; ++i)
        {
            int b = dataBlock[i];
            int isZero = FixedTimeLogic.IsZero(b);
            int isSeparator = FixedTimeLogic.IsZero(b ^ 1);
            valid &= 1 - (lookingForSeparator & (1 - isZero) & (1 - isSeparator));

            separatorIndex = FixedTimeLogic.Select(lookingForSeparator & isSeparator, i, separatorIndex);
            lookingForSeparator &= 1 - isSeparator;
        }

        valid &= 1 - lookingForSeparator;
        if (valid == 0)
            throw new CryptographicException("Invalid OAEP padding.");

        return dataBlock.AsSpan(separatorIndex + 1).ToArray();
    }

    static byte[] EncodePss(byte[] hash, HashAlgorithmName hashAlgorithm, int emBits, int emLength)
    {
        int hashLength = HashData(hashAlgorithm, []).Length;
        if (hash.Length != hashLength)
            throw new CryptographicException("Invalid hash length.");
        if (emLength < hashLength * 2 + 2)
            throw new CryptographicException("The hash value is too large for this key size.");

        byte[] salt = RandomNumberGenerator.GetBytes(hashLength);
        byte[] mPrime = new byte[8 + hashLength + salt.Length];
        hash.CopyTo(mPrime.AsSpan(8));
        salt.CopyTo(mPrime.AsSpan(8 + hashLength));
        byte[] h = HashData(hashAlgorithm, mPrime);

        byte[] dataBlock = new byte[emLength - hashLength - 1];
        dataBlock[dataBlock.Length - salt.Length - 1] = 1;
        salt.CopyTo(dataBlock.AsSpan(dataBlock.Length - salt.Length));

        byte[] mask = Mgf1(h, dataBlock.Length, hashAlgorithm);
        Xor(dataBlock, mask);
        ClearUnusedBits(dataBlock, emLength * 8 - emBits);

        byte[] encodedMessage = new byte[emLength];
        dataBlock.CopyTo(encodedMessage, 0);
        h.CopyTo(encodedMessage.AsSpan(dataBlock.Length));
        encodedMessage[^1] = 0xbc;
        return encodedMessage;
    }

    static bool VerifyPss(byte[] hash, byte[] encodedMessage, HashAlgorithmName hashAlgorithm, int emBits)
    {
        int hashLength = HashData(hashAlgorithm, []).Length;
        int emLength = encodedMessage.Length;
        if (hash.Length != hashLength || emLength < hashLength * 2 + 2 || encodedMessage[^1] != 0xbc)
            return false;

        int unusedBits = emLength * 8 - emBits;
        ReadOnlySpan<byte> maskedDataBlock = encodedMessage.AsSpan(0, emLength - hashLength - 1);
        byte[] h = encodedMessage.AsSpan(maskedDataBlock.Length, hashLength).ToArray();

        if (unusedBits != 0 && (maskedDataBlock[0] & (0xff << (8 - unusedBits))) != 0)
            return false;

        byte[] dataBlock = maskedDataBlock.ToArray();
        byte[] mask = Mgf1(h, dataBlock.Length, hashAlgorithm);
        Xor(dataBlock, mask);
        ClearUnusedBits(dataBlock, unusedBits);

        int saltLength = hashLength;
        int paddingLength = dataBlock.Length - saltLength - 1;
        for (int i = 0; i < paddingLength; ++i)
        {
            if (dataBlock[i] != 0)
                return false;
        }
        if (dataBlock[paddingLength] != 1)
            return false;

        byte[] mPrime = new byte[8 + hashLength + saltLength];
        hash.CopyTo(mPrime.AsSpan(8));
        dataBlock.AsSpan(dataBlock.Length - saltLength).CopyTo(mPrime.AsSpan(8 + hashLength));
        byte[] expected = HashData(hashAlgorithm, mPrime);
        return CryptographicOperations.FixedTimeEquals(h, expected);
    }

    static byte[] Transform(byte[] data, BigInteger exponent, BigInteger modulus, int outputLength)
    {
        var m = FromBytes(data);
        if (m >= modulus)
            throw new CryptographicException("Invalid input data.");

        return ToBytes(BigInteger.ModPow(m, exponent, modulus), outputLength);
    }

    byte[] TransformPrivate(byte[] data)
    {
        var m = FromBytes(data);
        if (m >= m_Modulus)
            throw new CryptographicException("Invalid input data.");

        BigInteger r;
        do
        {
            r = FromBytes(RandomNumberGenerator.GetBytes(KeyByteSize)) % (m_Modulus - 1) + 1;
        }
        while (BigInteger.GreatestCommonDivisor(r, m_Modulus) != 1);

        var blindedMessage = m * BigInteger.ModPow(r, m_Exponent, m_Modulus) % m_Modulus;
        var blindedResult = BigInteger.ModPow(blindedMessage, m_D, m_Modulus);
        var result = blindedResult * ModInverse(r, m_Modulus) % m_Modulus;
        return ToBytes(result, KeyByteSize);
    }

    static BigInteger ModInverse(BigInteger value, BigInteger modulus)
    {
        var a = value;
        var b = modulus;
        BigInteger x0 = 1;
        BigInteger x1 = 0;

        while (b != 0)
        {
            var quotient = a / b;
            (a, b) = (b, a - quotient * b);
            (x0, x1) = (x1, x0 - quotient * x1);
        }

        if (a != 1)
            throw new CryptographicException("The modular inverse does not exist.");

        return x0.Sign < 0 ? x0 + modulus : x0;
    }

    bool IsValidKeySize(int keySize)
    {
        foreach (var legalKeySize in LegalKeySizes)
        {
            if (legalKeySize.SkipSize == 0)
            {
                if (keySize == legalKeySize.MinSize)
                    return true;
            }
            else if (keySize >= legalKeySize.MinSize && keySize <= legalKeySize.MaxSize && (keySize - legalKeySize.MinSize) % legalKeySize.SkipSize == 0)
            {
                return true;
            }
        }

        return false;
    }

    static BigInteger FromBytes(ReadOnlySpan<byte> bytes)
    {
        return BigIntegerUtil.FromBytes(bytes, true, true);
    }

    static byte[] ToBytes(in BigInteger value)
    {
        return value.ToByteArray(true, true);
    }

    static byte[] ToBytes(in BigInteger value, int length)
    {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        int byteCount = value.GetByteCount(isUnsigned: true);
        if (byteCount > length)
            throw new CryptographicException("Integer value is too large.");

        byte[] result = new byte[length];

        bool success = value.TryWriteBytes(
            result.AsSpan(length - byteCount),
            out int bytesWritten,
            isUnsigned: true,
            isBigEndian: true);

        Debug.Assert(success);
        Debug.Assert(bytesWritten == byteCount);

        return result;
#else
        byte[] bytes = ToBytes(value);
        if (bytes.Length == length)
            return bytes;

        if (bytes.Length > length)
            throw new CryptographicException("Integer value is too large.");

        byte[] result = new byte[length];
        bytes.CopyTo(result.AsSpan(length - bytes.Length));
        return result;
#endif
    }

    static byte[] Mgf1(byte[] seed, int length, HashAlgorithmName hashAlgorithm)
    {
        byte[] result = new byte[length];
        Span<byte> counter = stackalloc byte[4];

        int offset = 0;
        for (uint i = 0; offset < length; ++i)
        {
            BinaryPrimitives.WriteUInt32BigEndian(counter, i);
            byte[] hashInput = new byte[seed.Length + counter.Length];
            seed.CopyTo(hashInput, 0);
            counter.CopyTo(hashInput.AsSpan(seed.Length));

            byte[] hash = HashData(hashAlgorithm, hashInput);
            int count = Math.Min(hash.Length, length - offset);
            hash.AsSpan(0, count).CopyTo(result.AsSpan(offset));
            offset += count;
        }

        return result;
    }

    static byte[] HashData(HashAlgorithmName hashAlgorithm, byte[] data)
    {
        using HashAlgorithm algorithm = hashAlgorithm.Name switch
        {
#pragma warning disable CA5350 // Do Not Use Weak Cryptographic Algorithms
            "MD5" => MD5Unapproved.Create(),
            "SHA1" => SHA1.Create(),
#pragma warning restore CA5350 // Do Not Use Weak Cryptographic Algorithms
            "SHA256" => SHA256.Create(),
            "SHA384" => SHA384.Create(),
            "SHA512" => SHA512.Create(),
            _ => throw new CryptographicException("Specified hash algorithm is not supported.")
        };
        return algorithm.ComputeHash(data);
    }

    static void Xor(byte[] x, byte[] y)
    {
        for (int i = 0; i < x.Length; ++i)
            x[i] ^= y[i];
    }

    static void ClearUnusedBits(byte[] data, int unusedBits)
    {
        if (unusedBits != 0)
            data[0] &= (byte)(0xff >>> unusedBits);
    }

    static ReadOnlySpan<byte> GetDigestInfoPrefix(HashAlgorithmName hashAlgorithm)
    {
        return hashAlgorithm.Name switch
        {
            "MD5" => [0x30, 0x20, 0x30, 0x0c, 0x06, 0x08, 0x2a, 0x86, 0x48, 0x86, 0xf7, 0x0d, 0x02, 0x05, 0x05, 0x00, 0x04, 0x10],
            "SHA1" => [0x30, 0x21, 0x30, 0x09, 0x06, 0x05, 0x2b, 0x0e, 0x03, 0x02, 0x1a, 0x05, 0x00, 0x04, 0x14],
            "SHA256" => [0x30, 0x31, 0x30, 0x0d, 0x06, 0x09, 0x60, 0x86, 0x48, 0x01, 0x65, 0x03, 0x04, 0x02, 0x01, 0x05, 0x00, 0x04, 0x20],
            "SHA384" => [0x30, 0x41, 0x30, 0x0d, 0x06, 0x09, 0x60, 0x86, 0x48, 0x01, 0x65, 0x03, 0x04, 0x02, 0x02, 0x05, 0x00, 0x04, 0x30],
            "SHA512" => [0x30, 0x51, 0x30, 0x0d, 0x06, 0x09, 0x60, 0x86, 0x48, 0x01, 0x65, 0x03, 0x04, 0x02, 0x03, 0x05, 0x00, 0x04, 0x40],
            _ => throw new CryptographicException("Specified hash algorithm is not supported.")
        };
    }

    static void ValidateHashAlgorithm(HashAlgorithmName hashAlgorithm)
    {
        if (string.IsNullOrEmpty(hashAlgorithm.Name))
            throw new ArgumentException("Hash algorithm name cannot be null or empty.", nameof(hashAlgorithm));
    }

    static void ValidateRequiredParameter([NotNull] byte[]? value, string name)
    {
        if (value is null or [])
            throw new CryptographicException(name + " parameter is required.");
    }

    static void ValidatePublicParameters(
        in BigInteger modulus,
        in BigInteger exponent,
        int keySize)
    {
        /*
         * Validates that:
         *
         *   - Modulus is greater than one, odd, and has the advertised bit length
         *   - Exponent is odd, greater than one, and smaller than the modulus
         */

        if (modulus <= 1 ||
            modulus.IsEven ||
            modulus.GetBitLength() != keySize ||
            exponent <= 1 ||
            exponent.IsEven ||
            exponent >= modulus)
        {
            throw InvalidParameters();
        }
    }

    static void ValidatePrivateParameters(
        in BigInteger modulus,
        in BigInteger exponent,
        in BigInteger d,
        in BigInteger p,
        in BigInteger q,
        in BigInteger dp,
        in BigInteger dq,
        in BigInteger inverseQ)
    {
        /*
         * Validates that:
         *
         *   - n == p * q, with distinct positive odd factors
         *   - d is in range and satisfies e * d ≡ 1 mod lcm(p−1, q−1)
         *   - DP == d mod (p−1)
         *   - DQ == d mod (q−1)
         *   - InverseQ * q ≡ 1 mod p, with InverseQ in range
         */

        if (p <= 1 ||
            q <= 1 ||
            p == q ||
            p.IsEven ||
            q.IsEven ||
            p * q != modulus ||
            d <= 1 ||
            d >= modulus)
        {
            throw InvalidParameters();
        }

        var pMinus1 = p - 1;
        var qMinus1 = q - 1;
        var lambda = pMinus1 / BigInteger.GreatestCommonDivisor(pMinus1, qMinus1) * qMinus1;

        if (exponent * d % lambda != 1 ||
            dp != d % pMinus1 ||
            dq != d % qMinus1 ||
            inverseQ <= 0 ||
            inverseQ >= p ||
            inverseQ * q % p != 1)
        {
            throw InvalidParameters();
        }
    }

    static CryptographicException InvalidParameters() => new("Invalid RSA parameters.");

    static CryptographicException PaddingModeNotSupported() => new("Specified padding mode is not supported.");

    static CryptographicException PrivateKeyIsNotAvailable() => new("Private key is not available.");

    int KeyByteSize => KeySize / 8;

    void EnsureNotDisposed()
    {
        ObjectDisposedException.ThrowIf(m_Flags[F_Disposed], this);
    }

    #region Flags

    BitVector32 m_Flags;

    const int F_HasKey = 1 << 0;
    const int F_HasPrivateParameters = 1 << 1;
    const int F_Disposed = 1 << 2;

    #endregion

    #region Parameters

    BigInteger m_Modulus;
    BigInteger m_Exponent;
    BigInteger m_P;
    BigInteger m_Q;
    BigInteger m_DP;
    BigInteger m_DQ;
    BigInteger m_InverseQ;
    BigInteger m_D;

    #endregion
}
