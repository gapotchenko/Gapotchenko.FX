using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gapotchenko.FX.Data.Encoding.Test
{
    [TestClass]
    public class TextDataEncodingAlphabetTests
    {
        [TestMethod]
        public void DataTextEncoding_Alphabet_I1()
        {
            var alphabet = new TextDataEncodingAlphabet("abc");

            Assert.AreEqual(3, alphabet.Size);
            Assert.AreEqual("abc", alphabet.Symbols);
            Assert.AreEqual('a', alphabet[0]);
            Assert.AreEqual(1, alphabet.IndexOf('b'));
            Assert.AreEqual(-1, alphabet.IndexOf('B'));
            Assert.AreEqual(-1, alphabet.IndexOf('d'));
        }

        [TestMethod]
        public void DataTextEncoding_Alphabet_I2()
        {
            var alphabet = new TextDataEncodingAlphabet("abcA");

            Assert.AreEqual(0, alphabet.IndexOf('a'));
            Assert.AreEqual(3, alphabet.IndexOf('A'));
        }

        [TestMethod]
        public void DataTextEncoding_Alphabet_I3()
        {
            var alphabet = new TextDataEncodingAlphabet("abc", false);

            Assert.AreEqual(3, alphabet.Size);
            Assert.AreEqual("abc", alphabet.Symbols);
            Assert.AreEqual('a', alphabet[0]);
            Assert.AreEqual(1, alphabet.IndexOf('b'));
            Assert.AreEqual(1, alphabet.IndexOf('B'));
            Assert.AreEqual(-1, alphabet.IndexOf('d'));
        }

        [TestMethod]
        public void DataTextEncoding_Alphabet_I4()
        {
            var alphabet = new TextDataEncodingAlphabet("stopВодка");

            Assert.AreEqual(1, alphabet.IndexOf('t'));
            Assert.AreEqual(-1, alphabet.IndexOf('T'));

            Assert.AreEqual(6, alphabet.IndexOf('д'));
            Assert.AreEqual(-1, alphabet.IndexOf('Д'));
        }

        [TestMethod]
        public void DataTextEncoding_Alphabet_I5()
        {
            var alphabet = new TextDataEncodingAlphabet("stopВодка", false);

            Assert.AreEqual('t', alphabet[1]);
            Assert.AreEqual(1, alphabet.IndexOf('t'));
            Assert.AreEqual(1, alphabet.IndexOf('T'));

            Assert.AreEqual('д', alphabet[6]);
            Assert.AreEqual(6, alphabet.IndexOf('д'));
            Assert.AreEqual(6, alphabet.IndexOf('Д'));
        }

        [TestMethod]
        public void DataTextEncoding_Alphabet_I6()
        {
            var alphabet = new TextDataEncodingAlphabet(
                "abc",
                true,
                new Dictionary<char, string>
                {
                    ['a'] = "xk",
                    ['b'] = "y"
                });

            Assert.AreEqual("abc", alphabet.Symbols);

            Assert.AreEqual(0, alphabet.IndexOf('a'));
            Assert.AreEqual(-1, alphabet.IndexOf('A'));
            Assert.AreEqual(0, alphabet.IndexOf('x'));
            Assert.AreEqual(-1, alphabet.IndexOf('X'));
            Assert.AreEqual(0, alphabet.IndexOf('k'));
            Assert.AreEqual(-1, alphabet.IndexOf('K'));

            Assert.AreEqual(1, alphabet.IndexOf('b'));
            Assert.AreEqual(-1, alphabet.IndexOf('B'));
            Assert.AreEqual(1, alphabet.IndexOf('y'));
            Assert.AreEqual(-1, alphabet.IndexOf('Y'));

            Assert.AreEqual(2, alphabet.IndexOf('c'));
            Assert.AreEqual(-1, alphabet.IndexOf('C'));
        }

        [TestMethod]
        public void DataTextEncoding_Alphabet_I7()
        {
            var alphabet = new TextDataEncodingAlphabet(
                "abc",
                false,
                new Dictionary<char, string>
                {
                    ['a'] = "xk",
                    ['b'] = "y"
                });

            Assert.AreEqual("abc", alphabet.Symbols);

            Assert.AreEqual(0, alphabet.IndexOf('a'));
            Assert.AreEqual(0, alphabet.IndexOf('A'));
            Assert.AreEqual(0, alphabet.IndexOf('x'));
            Assert.AreEqual(0, alphabet.IndexOf('X'));
            Assert.AreEqual(0, alphabet.IndexOf('k'));
            Assert.AreEqual(0, alphabet.IndexOf('K'));

            Assert.AreEqual(1, alphabet.IndexOf('b'));
            Assert.AreEqual(1, alphabet.IndexOf('B'));
            Assert.AreEqual(1, alphabet.IndexOf('y'));
            Assert.AreEqual(1, alphabet.IndexOf('Y'));

            Assert.AreEqual(2, alphabet.IndexOf('c'));
            Assert.AreEqual(2, alphabet.IndexOf('C'));
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void DataTextEncoding_Alphabet_C1()
        {
            new TextDataEncodingAlphabet("abca");
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void DataTextEncoding_Alphabet_C2()
        {
            new TextDataEncodingAlphabet("abcA", false);
        }
    }
}
