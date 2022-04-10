using Gapotchenko.FX.IO;
using Gapotchenko.FX.Linq;
using Gapotchenko.FX.Security.Cryptography.Tests.Arc4.TestVectors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace Gapotchenko.FX.Security.Cryptography.Tests.Arc4
{
    [TestClass]
    public class Arc4Tests
    {
        [TestMethod]
        public void Arc4_TV_RFC6229_1() => CheckTestVector(TestVectorReader.Read("RFC6229/01.txt"));

        [TestMethod]
        public void Arc4_TV_RFC6229_2() => CheckTestVector(TestVectorReader.Read("RFC6229/02.txt"));

        [TestMethod]
        public void Arc4_TV_RFC6229_3() => CheckTestVector(TestVectorReader.Read("RFC6229/03.txt"));

        [TestMethod]
        public void Arc4_TV_RFC6229_4() => CheckTestVector(TestVectorReader.Read("RFC6229/04.txt"));

        [TestMethod]
        public void Arc4_TV_RFC6229_5() => CheckTestVector(TestVectorReader.Read("RFC6229/05.txt"));

        [TestMethod]
        public void Arc4_TV_RFC6229_6() => CheckTestVector(TestVectorReader.Read("RFC6229/06.txt"));

        [TestMethod]
        public void Arc4_TV_RFC6229_7() => CheckTestVector(TestVectorReader.Read("RFC6229/07.txt"));

        [TestMethod]
        public void Arc4_TV_RFC6229_8() => CheckTestVector(TestVectorReader.Read("RFC6229/08.txt"));

        [TestMethod]
        public void Arc4_TV_RFC6229_9() => CheckTestVector(TestVectorReader.Read("RFC6229/09.txt"));

        [TestMethod]
        public void Arc4_TV_RFC6229_10() => CheckTestVector(TestVectorReader.Read("RFC6229/10.txt"));

        [TestMethod]
        public void Arc4_TV_RFC6229_11() => CheckTestVector(TestVectorReader.Read("RFC6229/11.txt"));

        [TestMethod]
        public void Arc4_TV_RFC6229_12() => CheckTestVector(TestVectorReader.Read("RFC6229/12.txt"));

        [TestMethod]
        public void Arc4_TV_RFC6229_13() => CheckTestVector(TestVectorReader.Read("RFC6229/13.txt"));

        [TestMethod]
        public void Arc4_TV_RFC6229_14() => CheckTestVector(TestVectorReader.Read("RFC6229/14.txt"));

        static void CheckTestVector(TestVector tv)
        {
            var arc4 = new Arc4Managed
            {
                Key = tv.Key
            };

            int dataSize = tv.Chunks.Select(x => x.Offset + x.Data.Length).Max();
            if (dataSize == 0)
                throw new InvalidOperationException("Test vector has no data.");

            var plainData = new byte[dataSize];
            var rng = RandomNumberGenerator.Create();
            rng.GetBytes(plainData);
            var plainStream = new MemoryStream(plainData, false);

            var encryptedStream = new MemoryStream();
            using (var cryptoStream = new CryptoStream(encryptedStream, arc4.CreateEncryptor(), CryptoStreamMode.Write, true))
                cryptoStream.Write(plainData, 0, plainData.Length);

            foreach (var chunk in tv.Chunks)
            {
                plainStream.Position = encryptedStream.Position = chunk.Offset;
                var keyStream = plainStream.AsEnumerable()
                    .Zip(
                        encryptedStream.AsEnumerable(),
                        (p, e) => (byte)(p ^ e)); // extract the key by a reverse xor
                Assert.IsTrue(keyStream.StartsWith(chunk.Data));
            }

            encryptedStream.Position = 0;

            var decryptedStream = new MemoryStream();
            using (var cryptoStream = new CryptoStream(decryptedStream, arc4.CreateDecryptor(), CryptoStreamMode.Write, true))
                encryptedStream.CopyTo(cryptoStream);

            plainStream.Position = decryptedStream.Position = 0;
            Assert.IsTrue(plainStream.AsEnumerable().SequenceEqual(decryptedStream.AsEnumerable()));
        }

        [TestMethod]
        public void Arc4_DisposableRoundTrip()
        {
            using var arc4 = new Arc4Managed();

            var plainData = new byte[4096];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(plainData);
            var plainStream = new MemoryStream(plainData, false);

            var encryptedStream = new MemoryStream();
            using (var encryptor = arc4.CreateEncryptor())
            using (var cryptoStream = new CryptoStream(encryptedStream, encryptor, CryptoStreamMode.Write, true))
                cryptoStream.Write(plainData, 0, plainData.Length);

            encryptedStream.Position = 0;

            var decryptedStream = new MemoryStream();
            using (var decryptor = arc4.CreateDecryptor())
            using (var cryptoStream = new CryptoStream(decryptedStream, decryptor, CryptoStreamMode.Write, true))
                encryptedStream.CopyTo(cryptoStream);

            decryptedStream.Position = 0;
            Assert.IsTrue(plainStream.AsEnumerable().SequenceEqual(decryptedStream.AsEnumerable()));
        }
    }
}
