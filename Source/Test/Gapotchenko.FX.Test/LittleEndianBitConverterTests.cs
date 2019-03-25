using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Gapotchenko.FX.Test
{
    [TestClass]
    public class LittleEndianBitConverterTests
    {
        [TestMethod]
        public void LittleEndianBitConverter_Boolean()
        {
            var buffer = new byte[1];

            LittleEndianBitConverter.FillBytes(true, buffer);
            Assert.AreEqual(1, buffer[0]);
            Assert.IsTrue(LittleEndianBitConverter.GetBytes(true).SequenceEqual(buffer));
            Assert.AreEqual(true, LittleEndianBitConverter.ToBoolean(buffer));

            LittleEndianBitConverter.FillBytes(false, buffer);
            Assert.AreEqual(0, buffer[0]);
            Assert.IsTrue(LittleEndianBitConverter.GetBytes(false).SequenceEqual(buffer));
            Assert.AreEqual(false, LittleEndianBitConverter.ToBoolean(buffer));
        }

        [TestMethod]
        public void LittleEndianBitConverter_Int16()
        {
            const int n = sizeof(Int16);

            const Int16 pos = 0x1122;
            const Int16 zero = 0;
            const Int16 neg = -pos;

            var posRepr = new byte[] { 0x22, 0x11 };

            // --------------------------------------------------------------

            var buffer = new byte[n];

            LittleEndianBitConverter.FillBytes(pos, buffer);
            Assert.IsTrue(buffer.SequenceEqual(posRepr));
            Assert.IsTrue(LittleEndianBitConverter.GetBytes(pos).SequenceEqual(buffer));
            Assert.AreEqual(pos, LittleEndianBitConverter.ToInt16(buffer));

            LittleEndianBitConverter.FillBytes(zero, buffer);
            Assert.IsTrue(buffer.SequenceEqual(new byte[n]));
            Assert.IsTrue(LittleEndianBitConverter.GetBytes(zero).SequenceEqual(buffer));
            Assert.AreEqual(zero, LittleEndianBitConverter.ToInt16(buffer));

            LittleEndianBitConverter.FillBytes(neg, buffer);
            Assert.IsTrue(buffer.SequenceEqual(Neg(posRepr)));
            Assert.IsTrue(LittleEndianBitConverter.GetBytes(neg).SequenceEqual(buffer));
            Assert.AreEqual(neg, LittleEndianBitConverter.ToInt16(buffer));
        }

        [TestMethod]
        public void LittleEndianBitConverter_UInt16()
        {
            const int n = sizeof(UInt16);

            const UInt16 pos = 0x1122;
            const UInt16 zero = 0;

            var posRepr = new byte[] { 0x22, 0x11 };

            // --------------------------------------------------------------

            var buffer = new byte[n];

            LittleEndianBitConverter.FillBytes(pos, buffer);
            Assert.IsTrue(buffer.SequenceEqual(posRepr));
            Assert.IsTrue(LittleEndianBitConverter.GetBytes(pos).SequenceEqual(buffer));
            Assert.AreEqual(pos, LittleEndianBitConverter.ToUInt16(buffer));

            LittleEndianBitConverter.FillBytes(zero, buffer);
            Assert.IsTrue(buffer.SequenceEqual(new byte[n]));
            Assert.IsTrue(LittleEndianBitConverter.GetBytes(zero).SequenceEqual(buffer));
            Assert.AreEqual(zero, LittleEndianBitConverter.ToUInt16(buffer));
        }

        [TestMethod]
        public void LittleEndianBitConverter_Int32()
        {
            const int n = sizeof(Int32);

            const Int32 pos = 0x11223344;
            const Int32 zero = 0;
            const Int32 neg = -pos;

            var posRepr = new byte[] { 0x44, 0x33, 0x22, 0x11 };

            // --------------------------------------------------------------

            var buffer = new byte[n];

            LittleEndianBitConverter.FillBytes(pos, buffer);
            Assert.IsTrue(buffer.SequenceEqual(posRepr));
            Assert.IsTrue(LittleEndianBitConverter.GetBytes(pos).SequenceEqual(buffer));
            Assert.AreEqual(pos, LittleEndianBitConverter.ToInt32(buffer));

            LittleEndianBitConverter.FillBytes(zero, buffer);
            Assert.IsTrue(buffer.SequenceEqual(new byte[n]));
            Assert.IsTrue(LittleEndianBitConverter.GetBytes(zero).SequenceEqual(buffer));
            Assert.AreEqual(zero, LittleEndianBitConverter.ToInt32(buffer));

            LittleEndianBitConverter.FillBytes(neg, buffer);
            Assert.IsTrue(buffer.SequenceEqual(Neg(posRepr)));
            Assert.IsTrue(LittleEndianBitConverter.GetBytes(neg).SequenceEqual(buffer));
            Assert.AreEqual(neg, LittleEndianBitConverter.ToInt32(buffer));
        }

        [TestMethod]
        public void LittleEndianBitConverter_UInt32()
        {
            const int n = sizeof(UInt32);

            const UInt32 pos = 0x11223344;
            const UInt32 zero = 0;

            var posRepr = new byte[] { 0x44, 0x33, 0x22, 0x11 };

            // --------------------------------------------------------------

            var buffer = new byte[n];

            LittleEndianBitConverter.FillBytes(pos, buffer);
            Assert.IsTrue(buffer.SequenceEqual(posRepr));
            Assert.IsTrue(LittleEndianBitConverter.GetBytes(pos).SequenceEqual(buffer));
            Assert.AreEqual(pos, LittleEndianBitConverter.ToUInt32(buffer));

            LittleEndianBitConverter.FillBytes(zero, buffer);
            Assert.IsTrue(buffer.SequenceEqual(new byte[n]));
            Assert.IsTrue(LittleEndianBitConverter.GetBytes(zero).SequenceEqual(buffer));
            Assert.AreEqual(zero, LittleEndianBitConverter.ToUInt32(buffer));
        }

        [TestMethod]
        public void LittleEndianBitConverter_Int64()
        {
            const int n = sizeof(Int64);

            const Int64 pos = 0x1122334455667788;
            const Int64 zero = 0;
            const Int64 neg = -pos;

            var posRepr = new byte[] { 0x88, 0x77, 0x66, 0x55, 0x44, 0x33, 0x22, 0x11 };

            // --------------------------------------------------------------

            var buffer = new byte[n];

            LittleEndianBitConverter.FillBytes(pos, buffer);
            Assert.IsTrue(buffer.SequenceEqual(posRepr));
            Assert.IsTrue(LittleEndianBitConverter.GetBytes(pos).SequenceEqual(buffer));
            Assert.AreEqual(pos, LittleEndianBitConverter.ToInt64(buffer));

            LittleEndianBitConverter.FillBytes(zero, buffer);
            Assert.IsTrue(buffer.SequenceEqual(new byte[n]));
            Assert.IsTrue(LittleEndianBitConverter.GetBytes(zero).SequenceEqual(buffer));
            Assert.AreEqual(zero, LittleEndianBitConverter.ToInt64(buffer));

            LittleEndianBitConverter.FillBytes(neg, buffer);
            Assert.IsTrue(buffer.SequenceEqual(Neg(posRepr)));
            Assert.IsTrue(LittleEndianBitConverter.GetBytes(neg).SequenceEqual(buffer));
            Assert.AreEqual(neg, LittleEndianBitConverter.ToInt64(buffer));
        }

        [TestMethod]
        public void LittleEndianBitConverter_UInt64()
        {
            const int n = sizeof(UInt64);

            const UInt64 pos = 0x1122334455667788;
            const UInt64 zero = 0;

            var posRepr = new byte[] { 0x88, 0x77, 0x66, 0x55, 0x44, 0x33, 0x22, 0x11 };

            // --------------------------------------------------------------

            var buffer = new byte[n];

            LittleEndianBitConverter.FillBytes(pos, buffer);
            Assert.IsTrue(buffer.SequenceEqual(posRepr));
            Assert.IsTrue(LittleEndianBitConverter.GetBytes(pos).SequenceEqual(buffer));
            Assert.AreEqual(pos, LittleEndianBitConverter.ToUInt64(buffer));

            LittleEndianBitConverter.FillBytes(zero, buffer);
            Assert.IsTrue(buffer.SequenceEqual(new byte[n]));
            Assert.IsTrue(LittleEndianBitConverter.GetBytes(zero).SequenceEqual(buffer));
            Assert.AreEqual(zero, LittleEndianBitConverter.ToUInt64(buffer));
        }

        [TestMethod]
        public void LittleEndianBitConverter_Single()
        {
            const int n = sizeof(Single);

            const Single pos = MathF.PI;
            const Single zero = 0;
            const Single neg = -pos;

            var posRepr = GetCanonicalRepresentation(pos);
            var negRepr = GetCanonicalRepresentation(neg);

            // --------------------------------------------------------------

            var buffer = new byte[n];

            LittleEndianBitConverter.FillBytes(pos, buffer);
            Assert.IsTrue(buffer.SequenceEqual(posRepr));
            Assert.IsTrue(LittleEndianBitConverter.GetBytes(pos).SequenceEqual(buffer));
            Assert.AreEqual(pos, LittleEndianBitConverter.ToSingle(buffer));

            LittleEndianBitConverter.FillBytes(zero, buffer);
            Assert.IsTrue(buffer.SequenceEqual(new byte[n]));
            Assert.IsTrue(LittleEndianBitConverter.GetBytes(zero).SequenceEqual(buffer));
            Assert.AreEqual(zero, LittleEndianBitConverter.ToSingle(buffer));

            LittleEndianBitConverter.FillBytes(neg, buffer);
            Assert.IsTrue(buffer.SequenceEqual(negRepr));
            Assert.IsTrue(LittleEndianBitConverter.GetBytes(neg).SequenceEqual(buffer));
            Assert.AreEqual(neg, LittleEndianBitConverter.ToSingle(buffer));
        }

        [TestMethod]
        public void LittleEndianBitConverter_Decimal()
        {
            const int n = sizeof(Decimal);

            const Decimal pos = 3.1415926535897932384626433833m;
            const Decimal zero = 0;
            const Decimal neg = -pos;

            var posRepr = GetCanonicalRepresentation(pos);
            var negRepr = GetCanonicalRepresentation(neg);

            // --------------------------------------------------------------

            var buffer = new byte[n];

            LittleEndianBitConverter.FillBytes(pos, buffer);
            Assert.IsTrue(buffer.SequenceEqual(posRepr));
            Assert.IsTrue(LittleEndianBitConverter.GetBytes(pos).SequenceEqual(buffer));
            Assert.AreEqual(pos, LittleEndianBitConverter.ToDecimal(buffer));

            LittleEndianBitConverter.FillBytes(zero, buffer);
            Assert.IsTrue(buffer.SequenceEqual(new byte[n]));
            Assert.IsTrue(LittleEndianBitConverter.GetBytes(zero).SequenceEqual(buffer));
            Assert.AreEqual(zero, LittleEndianBitConverter.ToDecimal(buffer));

            LittleEndianBitConverter.FillBytes(neg, buffer);
            Assert.IsTrue(buffer.SequenceEqual(negRepr));
            Assert.IsTrue(LittleEndianBitConverter.GetBytes(neg).SequenceEqual(buffer));
            Assert.AreEqual(neg, LittleEndianBitConverter.ToDecimal(buffer));
        }

        [TestMethod]
        public void LittleEndianBitConverter_Double()
        {
            const int n = sizeof(Double);

            const Double pos = Math.PI;
            const Double zero = 0;
            const Double neg = -pos;

            var posRepr = GetCanonicalRepresentation(pos);
            var negRepr = GetCanonicalRepresentation(neg);

            // --------------------------------------------------------------

            var buffer = new byte[n];

            LittleEndianBitConverter.FillBytes(pos, buffer);
            Assert.IsTrue(buffer.SequenceEqual(posRepr));
            Assert.IsTrue(LittleEndianBitConverter.GetBytes(pos).SequenceEqual(buffer));
            Assert.AreEqual(pos, LittleEndianBitConverter.ToDouble(buffer));

            LittleEndianBitConverter.FillBytes(zero, buffer);
            Assert.IsTrue(buffer.SequenceEqual(new byte[n]));
            Assert.IsTrue(LittleEndianBitConverter.GetBytes(zero).SequenceEqual(buffer));
            Assert.AreEqual(zero, LittleEndianBitConverter.ToDouble(buffer));

            LittleEndianBitConverter.FillBytes(neg, buffer);
            Assert.IsTrue(buffer.SequenceEqual(negRepr));
            Assert.IsTrue(LittleEndianBitConverter.GetBytes(neg).SequenceEqual(buffer));
            Assert.AreEqual(neg, LittleEndianBitConverter.ToDouble(buffer));
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
            return ms.ToArray();
        }

        internal static IEnumerable<byte> Neg(IEnumerable<byte> source)
        {
            bool carry = true;
            foreach (var x in source)
            {
                if (carry)
                {
                    var r = (byte)(~x + 1);
                    yield return r;
                    if (r != 0)
                        carry = false;
                }
                else
                {
                    yield return (byte)~x;
                }
            }
        }
    }
}
