using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Gapotchenko.FX.Test
{
    [TestClass]
    public class BigEndianBitConverterTests
    {
        [TestMethod]
        public void BigEndianBitConverter_Boolean()
        {
            var buffer = new byte[1];

            BigEndianBitConverter.FillBytes(true, buffer);
            Assert.AreEqual(1, buffer[0]);
            Assert.IsTrue(BigEndianBitConverter.GetBytes(true).SequenceEqual(buffer));
            Assert.AreEqual(true, BigEndianBitConverter.ToBoolean(buffer));

            BigEndianBitConverter.FillBytes(false, buffer);
            Assert.AreEqual(0, buffer[0]);
            Assert.IsTrue(BigEndianBitConverter.GetBytes(false).SequenceEqual(buffer));
            Assert.AreEqual(false, BigEndianBitConverter.ToBoolean(buffer));
        }

        [TestMethod]
        public void BigEndianBitConverter_Int16()
        {
            const int n = sizeof(Int16);

            const Int16 pos = 0x1122;
            const Int16 zero = 0;
            const Int16 neg = -pos;

            var posRepr = new byte[] { 0x11, 0x22 };

            // --------------------------------------------------------------

            var buffer = new byte[n];

            BigEndianBitConverter.FillBytes(pos, buffer);
            Assert.IsTrue(buffer.SequenceEqual(posRepr));
            Assert.IsTrue(BigEndianBitConverter.GetBytes(pos).SequenceEqual(buffer));
            Assert.AreEqual(pos, BigEndianBitConverter.ToInt16(buffer));

            BigEndianBitConverter.FillBytes(zero, buffer);
            Assert.IsTrue(buffer.SequenceEqual(new byte[n]));
            Assert.IsTrue(BigEndianBitConverter.GetBytes(zero).SequenceEqual(buffer));
            Assert.AreEqual(zero, BigEndianBitConverter.ToInt16(buffer));

            BigEndianBitConverter.FillBytes(neg, buffer);
            Assert.IsTrue(buffer.SequenceEqual(Neg(posRepr)));
            Assert.IsTrue(BigEndianBitConverter.GetBytes(neg).SequenceEqual(buffer));
            Assert.AreEqual(neg, BigEndianBitConverter.ToInt16(buffer));
        }

        [TestMethod]
        public void BigEndianBitConverter_UInt16()
        {
            const int n = sizeof(UInt16);

            const UInt16 pos = 0x1122;
            const UInt16 zero = 0;

            var posRepr = new byte[] { 0x11, 0x22 };

            // --------------------------------------------------------------

            var buffer = new byte[n];

            BigEndianBitConverter.FillBytes(pos, buffer);
            Assert.IsTrue(buffer.SequenceEqual(posRepr));
            Assert.IsTrue(BigEndianBitConverter.GetBytes(pos).SequenceEqual(buffer));
            Assert.AreEqual(pos, BigEndianBitConverter.ToUInt16(buffer));

            BigEndianBitConverter.FillBytes(zero, buffer);
            Assert.IsTrue(buffer.SequenceEqual(new byte[n]));
            Assert.IsTrue(BigEndianBitConverter.GetBytes(zero).SequenceEqual(buffer));
            Assert.AreEqual(zero, BigEndianBitConverter.ToUInt16(buffer));
        }

        [TestMethod]
        public void BigEndianBitConverter_Int32()
        {
            const int n = sizeof(Int32);

            const Int32 pos = 0x11223344;
            const Int32 zero = 0;
            const Int32 neg = -pos;

            var posRepr = new byte[] { 0x11, 0x22, 0x33, 0x44 };

            // --------------------------------------------------------------

            var buffer = new byte[n];

            BigEndianBitConverter.FillBytes(pos, buffer);
            Assert.IsTrue(buffer.SequenceEqual(posRepr));
            Assert.IsTrue(BigEndianBitConverter.GetBytes(pos).SequenceEqual(buffer));
            Assert.AreEqual(pos, BigEndianBitConverter.ToInt32(buffer));

            BigEndianBitConverter.FillBytes(zero, buffer);
            Assert.IsTrue(buffer.SequenceEqual(new byte[n]));
            Assert.IsTrue(BigEndianBitConverter.GetBytes(zero).SequenceEqual(buffer));
            Assert.AreEqual(zero, BigEndianBitConverter.ToInt32(buffer));

            BigEndianBitConverter.FillBytes(neg, buffer);
            Assert.IsTrue(buffer.SequenceEqual(Neg(posRepr)));
            Assert.IsTrue(BigEndianBitConverter.GetBytes(neg).SequenceEqual(buffer));
            Assert.AreEqual(neg, BigEndianBitConverter.ToInt32(buffer));
        }

        [TestMethod]
        public void BigEndianBitConverter_UInt32()
        {
            const int n = sizeof(UInt32);

            const UInt32 pos = 0x11223344;
            const UInt32 zero = 0;

            var posRepr = new byte[] { 0x11, 0x22, 0x33, 0x44 };

            // --------------------------------------------------------------

            var buffer = new byte[n];

            BigEndianBitConverter.FillBytes(pos, buffer);
            Assert.IsTrue(buffer.SequenceEqual(posRepr));
            Assert.IsTrue(BigEndianBitConverter.GetBytes(pos).SequenceEqual(buffer));
            Assert.AreEqual(pos, BigEndianBitConverter.ToUInt32(buffer));

            BigEndianBitConverter.FillBytes(zero, buffer);
            Assert.IsTrue(buffer.SequenceEqual(new byte[n]));
            Assert.IsTrue(BigEndianBitConverter.GetBytes(zero).SequenceEqual(buffer));
            Assert.AreEqual(zero, BigEndianBitConverter.ToUInt32(buffer));
        }

        [TestMethod]
        public void BigEndianBitConverter_Int64()
        {
            const int n = sizeof(Int64);

            const Int64 pos = 0x1122334455667788;
            const Int64 zero = 0;
            const Int64 neg = -pos;

            var posRepr = new byte[] { 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88 };

            // --------------------------------------------------------------

            var buffer = new byte[n];

            BigEndianBitConverter.FillBytes(pos, buffer);
            Assert.IsTrue(buffer.SequenceEqual(posRepr));
            Assert.IsTrue(BigEndianBitConverter.GetBytes(pos).SequenceEqual(buffer));
            Assert.AreEqual(pos, BigEndianBitConverter.ToInt64(buffer));

            BigEndianBitConverter.FillBytes(zero, buffer);
            Assert.IsTrue(buffer.SequenceEqual(new byte[n]));
            Assert.IsTrue(BigEndianBitConverter.GetBytes(zero).SequenceEqual(buffer));
            Assert.AreEqual(zero, BigEndianBitConverter.ToInt64(buffer));

            BigEndianBitConverter.FillBytes(neg, buffer);
            Assert.IsTrue(buffer.SequenceEqual(Neg(posRepr)));
            Assert.IsTrue(BigEndianBitConverter.GetBytes(neg).SequenceEqual(buffer));
            Assert.AreEqual(neg, BigEndianBitConverter.ToInt64(buffer));
        }

        [TestMethod]
        public void BigEndianBitConverter_UInt64()
        {
            const int n = sizeof(UInt64);

            const UInt64 pos = 0x1122334455667788;
            const UInt64 zero = 0;

            var posRepr = new byte[] { 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88 };

            // --------------------------------------------------------------

            var buffer = new byte[n];

            BigEndianBitConverter.FillBytes(pos, buffer);
            Assert.IsTrue(buffer.SequenceEqual(posRepr));
            Assert.IsTrue(BigEndianBitConverter.GetBytes(pos).SequenceEqual(buffer));
            Assert.AreEqual(pos, BigEndianBitConverter.ToUInt64(buffer));

            BigEndianBitConverter.FillBytes(zero, buffer);
            Assert.IsTrue(buffer.SequenceEqual(new byte[n]));
            Assert.IsTrue(BigEndianBitConverter.GetBytes(zero).SequenceEqual(buffer));
            Assert.AreEqual(zero, BigEndianBitConverter.ToUInt64(buffer));
        }

        [TestMethod]
        public void BigEndianBitConverter_Single()
        {
            const int n = sizeof(Single);

            const Single pos = (float)Math.PI;
            const Single zero = 0;
            const Single neg = -pos;

            var posRepr = GetCanonicalRepresentation(pos);
            var negRepr = GetCanonicalRepresentation(neg);

            // --------------------------------------------------------------

            var buffer = new byte[n];

            BigEndianBitConverter.FillBytes(pos, buffer);
            Assert.IsTrue(buffer.SequenceEqual(posRepr));
            Assert.IsTrue(BigEndianBitConverter.GetBytes(pos).SequenceEqual(buffer));
            Assert.AreEqual(pos, BigEndianBitConverter.ToSingle(buffer));

            BigEndianBitConverter.FillBytes(zero, buffer);
            Assert.IsTrue(buffer.SequenceEqual(new byte[n]));
            Assert.IsTrue(BigEndianBitConverter.GetBytes(zero).SequenceEqual(buffer));
            Assert.AreEqual(zero, BigEndianBitConverter.ToSingle(buffer));

            BigEndianBitConverter.FillBytes(neg, buffer);
            Assert.IsTrue(buffer.SequenceEqual(negRepr));
            Assert.IsTrue(BigEndianBitConverter.GetBytes(neg).SequenceEqual(buffer));
            Assert.AreEqual(neg, BigEndianBitConverter.ToSingle(buffer));
        }

        [TestMethod]
        public void BigEndianBitConverter_Decimal()
        {
            const int n = sizeof(Decimal);

            const Decimal pos = 3.1415926535897932384626433833m;
            const Decimal zero = 0;
            const Decimal neg = -pos;

            var posRepr = GetCanonicalRepresentation(pos);
            var negRepr = GetCanonicalRepresentation(neg);

            // --------------------------------------------------------------

            var buffer = new byte[n];

            BigEndianBitConverter.FillBytes(pos, buffer);
            Assert.IsTrue(buffer.SequenceEqual(posRepr));
            Assert.IsTrue(BigEndianBitConverter.GetBytes(pos).SequenceEqual(buffer));
            Assert.AreEqual(pos, BigEndianBitConverter.ToDecimal(buffer));

            BigEndianBitConverter.FillBytes(zero, buffer);
            Assert.IsTrue(buffer.SequenceEqual(new byte[n]));
            Assert.IsTrue(BigEndianBitConverter.GetBytes(zero).SequenceEqual(buffer));
            Assert.AreEqual(zero, BigEndianBitConverter.ToDecimal(buffer));

            BigEndianBitConverter.FillBytes(neg, buffer);
            Assert.IsTrue(buffer.SequenceEqual(negRepr));
            Assert.IsTrue(BigEndianBitConverter.GetBytes(neg).SequenceEqual(buffer));
            Assert.AreEqual(neg, BigEndianBitConverter.ToDecimal(buffer));
        }

        [TestMethod]
        public void BigEndianBitConverter_Double()
        {
            const int n = sizeof(Double);

            const Double pos = Math.PI;
            const Double zero = 0;
            const Double neg = -pos;

            var posRepr = GetCanonicalRepresentation(pos);
            var negRepr = GetCanonicalRepresentation(neg);

            // --------------------------------------------------------------

            var buffer = new byte[n];

            BigEndianBitConverter.FillBytes(pos, buffer);
            Assert.IsTrue(buffer.SequenceEqual(posRepr));
            Assert.IsTrue(BigEndianBitConverter.GetBytes(pos).SequenceEqual(buffer));
            Assert.AreEqual(pos, BigEndianBitConverter.ToDouble(buffer));

            BigEndianBitConverter.FillBytes(zero, buffer);
            Assert.IsTrue(buffer.SequenceEqual(new byte[n]));
            Assert.IsTrue(BigEndianBitConverter.GetBytes(zero).SequenceEqual(buffer));
            Assert.AreEqual(zero, BigEndianBitConverter.ToDouble(buffer));

            BigEndianBitConverter.FillBytes(neg, buffer);
            Assert.IsTrue(buffer.SequenceEqual(negRepr));
            Assert.IsTrue(BigEndianBitConverter.GetBytes(neg).SequenceEqual(buffer));
            Assert.AreEqual(neg, BigEndianBitConverter.ToDouble(buffer));
        }

        static byte[] GetCanonicalRepresentation(Single value) => _GetCanonicalRepresentationCore(x => x.Write(value));
        static byte[] GetCanonicalRepresentation(Double value) => _GetCanonicalRepresentationCore(x => x.Write(value));
        static byte[] GetCanonicalRepresentation(Decimal value) => _GetCanonicalRepresentationCore(x => x.Write(value));

        static byte[] _GetCanonicalRepresentationCore(Action<BinaryWriter> write)
        {
            var ms = new MemoryStream();
            var bw = new BinaryWriter(ms);
            write(bw);
            bw.Close();
            return ms.ToArray().Reverse().ToArray();
        }

        static IEnumerable<byte> Neg(IEnumerable<byte> source) => LittleEndianBitConverterTests.Neg(source.Reverse()).Reverse();
    }
}
