using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gapotchenko.FX.Test
{
    [TestClass]
    public class OptionalTests
    {
        [TestMethod]
        public void Optional_AV1()
        {
            var option = Optional<int>.None;
            Assert.IsFalse(option.HasValue);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Optional_AV2()
        {
            var option = Optional<int>.None;
            var value = option.Value;
        }

        [TestMethod]
        public void Optional_AV3()
        {
            var option = Optional.Some(10);
            Assert.IsTrue(option.HasValue);
            Assert.AreEqual(10, option.Value);
        }

        [TestMethod]
        public void Optional_AV4()
        {
            var option = new Optional<int>();
            Assert.IsFalse(option.HasValue);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Optional_AV5()
        {
            var option = new Optional<int>();
            var value = option.Value;
        }

        [TestMethod]
        public void Optional_AV6()
        {
            var option = new Optional<int>(100);
            Assert.IsTrue(option.HasValue);
            Assert.AreEqual(100, option.Value);
        }

        [TestMethod]
        public void Optional_AR1()
        {
            var option = Optional<string>.None;
            Assert.IsFalse(option.HasValue);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Optional_AR2()
        {
            var option = Optional<string>.None;
            var value = option.Value;
        }

        [TestMethod]
        public void Optional_AR3()
        {
            var option = Optional.Some("ABC");
            Assert.IsTrue(option.HasValue);
            Assert.AreEqual("ABC", option.Value);
        }

        [TestMethod]
        public void Optional_AR4()
        {
            var option = new Optional<string>();
            Assert.IsFalse(option.HasValue);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Optional_AR5()
        {
            var option = new Optional<string>();
            var value = option.Value;
        }

        [TestMethod]
        public void Optional_AR6()
        {
            var option = new Optional<string>("ABCDEF");
            Assert.IsTrue(option.HasValue);
            Assert.AreEqual("ABCDEF", option.Value);
        }

        [TestMethod]
        public void Optional_AR7()
        {
            var option = new Optional<string>(null);
            Assert.IsTrue(option.HasValue);
            Assert.IsNull(option.Value);
        }

        [TestMethod]
        public void Optional_BV1()
        {
            var option = Optional<int>.None;
            Assert.IsFalse(option.Equals(0));
            Assert.IsFalse(option.Equals(null));
        }

        [TestMethod]
        public void Optional_BV2()
        {
            var option = Optional<int>.None;
            Assert.IsFalse(option.Equals((object)0));
        }

        [TestMethod]
        public void Optional_BV3()
        {
            var option = Optional<int>.None;
            Assert.IsFalse(option.Equals(Optional.Some(0)));

            Assert.IsTrue(option.Equals(Optional<int>.None));
            Assert.IsFalse(option.Equals(Optional<long>.None));

            Assert.IsTrue(option.Equals((object)Optional<int>.None));
            Assert.IsFalse(option.Equals((object)Optional<long>.None));
        }

        [TestMethod]
        public void Optional_BV4()
        {
            var option = Optional.Some(10);
            Assert.IsTrue(option.Equals(10));
            Assert.IsFalse(option.Equals(11));
        }

        [TestMethod]
        public void Optional_BV5()
        {
            var option = Optional.Some(10);
            Assert.IsTrue(option.Equals((object)10));
            Assert.IsFalse(option.Equals((object)11));
        }

        [TestMethod]
        public void Optional_BV6()
        {
            var option = Optional.Some(10);
            Assert.IsTrue(option.Equals(Optional.Some(10)));
            Assert.IsFalse(option.Equals(Optional.Some(11)));
        }

        [TestMethod]
        public void Optional_BR1()
        {
            var option = Optional<string>.None;
            Assert.IsFalse(option.Equals(null));
        }

        [TestMethod]
        public void Optional_BR2()
        {
            var option = Optional<string>.None;
            Assert.IsFalse(option.Equals((object)null));
        }

        [TestMethod]
        public void Optional_BR3()
        {
            var option = Optional<string>.None;
            Assert.IsFalse(option.Equals(Optional.Some<string>(null)));

            Assert.IsTrue(option.Equals(Optional<string>.None));
            Assert.IsFalse(option.Equals(Optional<Version>.None));

            Assert.IsTrue(option.Equals((object)Optional<string>.None));
            Assert.IsFalse(option.Equals(Optional<Version>.None));
        }

        [TestMethod]
        public void Optional_BR4()
        {
            var option = Optional.Some("10");
            Assert.IsTrue(option.Equals("10"));
            Assert.IsFalse(option.Equals("11"));
        }

        [TestMethod]
        public void Optional_BR5()
        {
            var option = Optional.Some("10");
            Assert.IsTrue(option.Equals((object)"10"));
            Assert.IsFalse(option.Equals((object)"11"));
        }

        [TestMethod]
        public void Optional_BR6()
        {
            var option = Optional.Some("10");
            Assert.IsTrue(option.Equals(Optional.Some("10")));
            Assert.IsFalse(option.Equals(Optional.Some("11")));
        }

        [TestMethod]
        public void Optional_CV1()
        {
            var option = Optional<int>.None;
            Assert.AreEqual(0, option.GetHashCode());
        }

        [TestMethod]
        public void Optional_CV2()
        {
            var option = Optional.Some(10);
            Assert.AreEqual(10.GetHashCode(), option.GetHashCode());
        }

        [TestMethod]
        public void Optional_CR1()
        {
            var option = Optional<string>.None;
            Assert.AreEqual(0, option.GetHashCode());
        }

        [TestMethod]
        public void Optional_CR2()
        {
            var option = Optional.Some("10");
            Assert.AreEqual("10".GetHashCode(), option.GetHashCode());
        }

        [TestMethod]
        public void Optional_CR3()
        {
            var option = Optional.Some<string>(null);
            Assert.AreEqual(0, option.GetHashCode());
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
            Assert.AreEqual(Optional<string>.None, Optional.Discriminate<string>(null, string.IsNullOrEmpty));
            Assert.AreEqual(Optional<string>.None, Optional.Discriminate("", string.IsNullOrEmpty));
            Assert.AreEqual("A", Optional.Discriminate("A", string.IsNullOrEmpty));
        }
    }
}
