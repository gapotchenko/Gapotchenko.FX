using Gapotchenko.FX.Collections.Concurrent;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Gapotchenko.FX.Collections.Test.Concurrent
{
    [TestClass]
    public class ConcurrentHashSetTests
    {
        [TestMethod]
        public void ConcurrentHashSet_Ctor_1()
        {
            var hashSet = new ConcurrentHashSet<string>((IEqualityComparer<string>)null);
            Assert.AreEqual(0, hashSet.Count);
        }

        [TestMethod]
        public void ConcurrentHashSet_Ctor_2()
        {
            var hashSet = new ConcurrentHashSet<string>(StringComparer.Ordinal);
            Assert.AreEqual(0, hashSet.Count);
        }

        [TestMethod]
        public void ConcurrentHashSet_NullStringKey_DefaultEqualityComparer()
        {
            var hashSet = new ConcurrentHashSet<string>();

            Assert.IsFalse(hashSet.Contains(null));

            hashSet.Add(null);
            Assert.IsTrue(hashSet.Contains(null));

            Assert.AreEqual(1, hashSet.Count);

            Assert.IsTrue(hashSet.Add("A"));
            Assert.IsTrue(hashSet.Add("B"));
            Assert.IsTrue(hashSet.Add("C"));
            Assert.IsTrue(hashSet.Add("a"));
            Assert.IsTrue(hashSet.Add("b"));
            Assert.IsTrue(hashSet.Add("c"));

            Assert.AreEqual(7, hashSet.Count);

            Assert.IsTrue(hashSet.Contains(null));
            Assert.IsTrue(hashSet.Contains("A"));
            Assert.IsTrue(hashSet.Contains("B"));
            Assert.IsTrue(hashSet.Contains("C"));
            Assert.IsFalse(hashSet.Contains("D"));
            Assert.IsTrue(hashSet.Contains("a"));
            Assert.IsTrue(hashSet.Contains("b"));
            Assert.IsTrue(hashSet.Contains("c"));
            Assert.IsFalse(hashSet.Contains("d"));
        }

        [TestMethod]
        public void ConcurrentHashSet_NullStringKey_OicEqualityComparer()
        {
            var hashSet = new ConcurrentHashSet<string>(StringComparer.OrdinalIgnoreCase);

            Assert.IsFalse(hashSet.Contains(null));

            hashSet.Add(null);
            Assert.IsTrue(hashSet.Contains(null));

            Assert.AreEqual(1, hashSet.Count);

            Assert.IsTrue(hashSet.Add("A"));
            Assert.IsTrue(hashSet.Add("B"));
            Assert.IsTrue(hashSet.Add("C"));
            Assert.IsFalse(hashSet.Add("a"));
            Assert.IsFalse(hashSet.Add("b"));
            Assert.IsFalse(hashSet.Add("c"));

            Assert.AreEqual(4, hashSet.Count);

            Assert.IsTrue(hashSet.Contains(null));
            Assert.IsTrue(hashSet.Contains("A"));
            Assert.IsTrue(hashSet.Contains("B"));
            Assert.IsTrue(hashSet.Contains("C"));
            Assert.IsFalse(hashSet.Contains("D"));
            Assert.IsTrue(hashSet.Contains("a"));
            Assert.IsTrue(hashSet.Contains("b"));
            Assert.IsTrue(hashSet.Contains("c"));
            Assert.IsFalse(hashSet.Contains("d"));
        }
    }
}
