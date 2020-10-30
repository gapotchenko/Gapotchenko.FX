using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Tests
{
    [TestClass]
    public class OptionalTests
    {
        [TestMethod]
        public void Optional_AV1()
        {
            var optional = Optional<int>.None;
            Assert.IsFalse(optional.HasValue);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Optional_AV2()
        {
            var optional = Optional<int>.None;
            var value = optional.Value;
        }

        [TestMethod]
        public void Optional_AV3()
        {
            var optional = Optional.Some(10);
            Assert.IsTrue(optional.HasValue);
            Assert.AreEqual(10, optional.Value);
        }

        [TestMethod]
        public void Optional_AV4()
        {
            var optional = new Optional<int>();
            Assert.IsFalse(optional.HasValue);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Optional_AV5()
        {
            var optional = new Optional<int>();
            var value = optional.Value;
        }

        [TestMethod]
        public void Optional_AV6()
        {
            var optional = new Optional<int>(100);
            Assert.IsTrue(optional.HasValue);
            Assert.AreEqual(100, optional.Value);
        }

        [TestMethod]
        public void Optional_AR1()
        {
            var optional = Optional<string>.None;
            Assert.IsFalse(optional.HasValue);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Optional_AR2()
        {
            var optional = Optional<string>.None;
            _ = optional.Value;
        }

        [TestMethod]
        public void Optional_AR3()
        {
            var optional = Optional.Some("ABC");
            Assert.IsTrue(optional.HasValue);
            Assert.AreEqual("ABC", optional.Value);
        }

        [TestMethod]
        public void Optional_AR4()
        {
            var optional = new Optional<string>();
            Assert.IsFalse(optional.HasValue);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Optional_AR5()
        {
            var optional = new Optional<string>();
            var value = optional.Value;
        }

        [TestMethod]
        public void Optional_AR6()
        {
            var optional = new Optional<string>("ABCDEF");
            Assert.IsTrue(optional.HasValue);
            Assert.AreEqual("ABCDEF", optional.Value);
        }

        [TestMethod]
        public void Optional_AR7()
        {
            var optional = new Optional<string?>(null);
            Assert.IsTrue(optional.HasValue);
            Assert.IsNull(optional.Value);
        }

        [TestMethod]
        public void Optional_BV1()
        {
            var optional = Optional<int>.None;
            Assert.IsFalse(optional.Equals(0));
            Assert.IsFalse(optional.Equals(null));
        }

        [TestMethod]
        public void Optional_BV2()
        {
            var optional = Optional<int>.None;
            Assert.IsFalse(optional.Equals((object)0));
        }

        [TestMethod]
        public void Optional_BV3()
        {
            var optional = Optional<int>.None;
            Assert.IsFalse(optional.Equals(Optional.Some(0)));

            Assert.IsTrue(optional.Equals(Optional<int>.None));
            Assert.IsFalse(optional.Equals(Optional<long>.None));

            Assert.IsTrue(optional.Equals((object)Optional<int>.None));
            Assert.IsFalse(optional.Equals((object)Optional<long>.None));
        }

        [TestMethod]
        public void Optional_BV4()
        {
            var optional = Optional.Some(10);
            Assert.IsTrue(optional.Equals(10));
            Assert.IsFalse(optional.Equals(11));
        }

        [TestMethod]
        public void Optional_BV5()
        {
            var optional = Optional.Some(10);
            Assert.IsTrue(optional.Equals((object)10));
            Assert.IsFalse(optional.Equals((object)11));
        }

        [TestMethod]
        public void Optional_BV6()
        {
            var optional = Optional.Some(10);
            Assert.IsTrue(optional.Equals(Optional.Some(10)));
            Assert.IsFalse(optional.Equals(Optional.Some(11)));
        }

        [TestMethod]
        public void Optional_BR1()
        {
            var optional = Optional<string>.None;
            Assert.IsFalse(optional.Equals(null));
        }

        [TestMethod]
        public void Optional_BR2()
        {
            var optional = Optional<string>.None;
            Assert.IsFalse(optional.Equals((object?)null));
        }

        [TestMethod]
        public void Optional_BR3()
        {
            var optional = Optional<string?>.None;
            Assert.IsFalse(optional.Equals(Optional.Some<string?>(null)));

            Assert.IsTrue(optional.Equals(Optional<string?>.None));
            Assert.IsFalse(optional.Equals(Optional<Version>.None));

            Assert.IsTrue(optional.Equals((object)Optional<string>.None));
            Assert.IsFalse(optional.Equals(Optional<Version>.None));
        }

        [TestMethod]
        public void Optional_BR4()
        {
            var optional = Optional.Some("10");
            Assert.IsTrue(optional.Equals("10"));
            Assert.IsFalse(optional.Equals("11"));
        }

        [TestMethod]
        public void Optional_BR5()
        {
            var optional = Optional.Some("10");
            Assert.IsTrue(optional.Equals((object)"10"));
            Assert.IsFalse(optional.Equals((object)"11"));
        }

        [TestMethod]
        public void Optional_BR6()
        {
            var optional = Optional.Some("10");
            Assert.IsTrue(optional.Equals(Optional.Some("10")));
            Assert.IsFalse(optional.Equals(Optional.Some("11")));
        }

        [TestMethod]
        public void Optional_CV1()
        {
            var optional = Optional<int>.None;
            Assert.AreEqual(0, optional.GetHashCode());
        }

        [TestMethod]
        public void Optional_CV2()
        {
            var optional = Optional.Some(10);
            Assert.AreEqual(10.GetHashCode(), optional.GetHashCode());
        }

        [TestMethod]
        public void Optional_CR1()
        {
            var optional = Optional<string>.None;
            Assert.AreEqual(0, optional.GetHashCode());
        }

        [TestMethod]
        public void Optional_CR2()
        {
            var optional = Optional.Some("10");
            Assert.AreEqual("10".GetHashCode(), optional.GetHashCode());
        }

        [TestMethod]
        public void Optional_CR3()
        {
            var optional = Optional.Some<string?>(null);
            Assert.AreEqual(0, optional.GetHashCode());
        }

        [TestMethod]
        public void Optional_DA1()
        {
            Assert.AreEqual(Optional.Some(-1), Optional.Discriminate(-1));
            Assert.AreEqual(Optional<int>.None, Optional.Discriminate(0));
            Assert.AreEqual(Optional.Some(1), Optional.Discriminate(1));
            Assert.AreEqual(Optional.Some(2), Optional.Discriminate(2));
        }

        [TestMethod]
        public void Optional_DA2()
        {
            Assert.AreEqual(Optional.Some(-1), Optional.Discriminate(-1, 1));
            Assert.AreEqual(Optional.Some(0), Optional.Discriminate(0, 1));
            Assert.AreEqual(Optional<int>.None, Optional.Discriminate(1, 1));
            Assert.AreEqual(Optional.Some(2), Optional.Discriminate(2, 1));
        }

        [TestMethod]
        public void Optional_DA3()
        {
            Assert.AreEqual(Optional<string?>.None, Optional.Discriminate<string?>(null, string.IsNullOrEmpty));
            Assert.AreEqual(Optional<string>.None, Optional.Discriminate("", string.IsNullOrEmpty));
            Assert.AreEqual("A", Optional.Discriminate("A", string.IsNullOrEmpty));
        }
    }
}
