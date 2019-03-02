using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Test
{
    [TestClass]
    public class OptionalTests
    {
        [TestMethod]
        public void Optional_TestAV1()
        {
            var option = Optional<int>.None;
            Assert.IsFalse(option.HasValue);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Optional_TestAV2()
        {
            var option = Optional<int>.None;
            var value = option.Value;
        }

        [TestMethod]
        public void Optional_TestAV3()
        {
            var option = Optional.Some(10);
            Assert.IsTrue(option.HasValue);
            Assert.AreEqual(10, option.Value);
        }

        [TestMethod]
        public void Optional_TestAV4()
        {
            var option = new Optional<int>();
            Assert.IsFalse(option.HasValue);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Optional_TestAV5()
        {
            var option = new Optional<int>();
            var value = option.Value;
        }

        [TestMethod]
        public void Optional_TestAV6()
        {
            var option = new Optional<int>(100);
            Assert.IsTrue(option.HasValue);
            Assert.AreEqual(100, option.Value);
        }

        [TestMethod]
        public void Optional_TestAR1()
        {
            var option = Optional<string>.None;
            Assert.IsFalse(option.HasValue);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Optional_TestAR2()
        {
            var option = Optional<string>.None;
            var value = option.Value;
        }

        [TestMethod]
        public void Optional_TestAR3()
        {
            var option = Optional.Some("ABC");
            Assert.IsTrue(option.HasValue);
            Assert.AreEqual("ABC", option.Value);
        }

        [TestMethod]
        public void Optional_TestAR4()
        {
            var option = new Optional<string>();
            Assert.IsFalse(option.HasValue);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Optional_TestAR5()
        {
            var option = new Optional<string>();
            var value = option.Value;
        }

        [TestMethod]
        public void Optional_TestAR6()
        {
            var option = new Optional<string>("ABCDEF");
            Assert.IsTrue(option.HasValue);
            Assert.AreEqual("ABCDEF", option.Value);
        }

        [TestMethod]
        public void Optional_TestAR7()
        {
            var option = new Optional<string>(null);
            Assert.IsTrue(option.HasValue);
            Assert.IsNull(option.Value);
        }

        [TestMethod]
        public void Optional_TestBV1()
        {
            var option = Optional<int>.None;
            Assert.IsFalse(option.Equals(0));
            Assert.IsFalse(option.Equals(null));
        }

        [TestMethod]
        public void Optional_TestBV2()
        {
            var option = Optional<int>.None;
            Assert.IsFalse(option.Equals((object)0));
        }

        [TestMethod]
        public void Optional_TestBV3()
        {
            var option = Optional<int>.None;
            Assert.IsFalse(option.Equals(Optional.Some(0)));

            Assert.IsTrue(option.Equals(Optional<int>.None));
            Assert.IsFalse(option.Equals(Optional<long>.None));

            Assert.IsTrue(option.Equals((object)Optional<int>.None));
            Assert.IsFalse(option.Equals((object)Optional<long>.None));
        }

        [TestMethod]
        public void Optional_TestBV4()
        {
            var option = Optional.Some(10);
            Assert.IsTrue(option.Equals(10));
            Assert.IsFalse(option.Equals(11));
        }

        [TestMethod]
        public void Optional_TestBV5()
        {
            var option = Optional.Some(10);
            Assert.IsTrue(option.Equals((object)10));
            Assert.IsFalse(option.Equals((object)11));
        }

        [TestMethod]
        public void Optional_TestBV6()
        {
            var option = Optional.Some(10);
            Assert.IsTrue(option.Equals(Optional.Some(10)));
            Assert.IsFalse(option.Equals(Optional.Some(11)));
        }

        [TestMethod]
        public void Optional_TestBR1()
        {
            var option = Optional<string>.None;
            Assert.IsFalse(option.Equals(null));
        }

        [TestMethod]
        public void Optional_TestBR2()
        {
            var option = Optional<string>.None;
            Assert.IsFalse(option.Equals((object)null));
        }

        [TestMethod]
        public void Optional_TestBR3()
        {
            var option = Optional<string>.None;
            Assert.IsFalse(option.Equals(Optional.Some<string>(null)));

            Assert.IsTrue(option.Equals(Optional<string>.None));
            Assert.IsFalse(option.Equals(Optional<Version>.None));

            Assert.IsTrue(option.Equals((object)Optional<string>.None));
            Assert.IsFalse(option.Equals(Optional<Version>.None));
        }

        [TestMethod]
        public void Optional_TestBR4()
        {
            var option = Optional.Some("10");
            Assert.IsTrue(option.Equals("10"));
            Assert.IsFalse(option.Equals("11"));
        }

        [TestMethod]
        public void Optional_TestBR5()
        {
            var option = Optional.Some("10");
            Assert.IsTrue(option.Equals((object)"10"));
            Assert.IsFalse(option.Equals((object)"11"));
        }

        [TestMethod]
        public void Optional_TestBR6()
        {
            var option = Optional.Some("10");
            Assert.IsTrue(option.Equals(Optional.Some("10")));
            Assert.IsFalse(option.Equals(Optional.Some("11")));
        }

        [TestMethod]
        public void Optional_TestCV1()
        {
            var option = Optional<int>.None;
            Assert.AreEqual(0, option.GetHashCode());
        }

        [TestMethod]
        public void Optional_TestCV2()
        {
            var option = Optional.Some(10);
            Assert.AreEqual(10.GetHashCode(), option.GetHashCode());
        }

        [TestMethod]
        public void Optional_TestCR1()
        {
            var option = Optional<string>.None;
            Assert.AreEqual(0, option.GetHashCode());
        }

        [TestMethod]
        public void Optional_TestCR2()
        {
            var option = Optional.Some("10");
            Assert.AreEqual("10".GetHashCode(), option.GetHashCode());
        }

        [TestMethod]
        public void Optional_TestCR3()
        {
            var option = Optional.Some<string>(null);
            Assert.AreEqual(0, option.GetHashCode());
        }
    }
}
