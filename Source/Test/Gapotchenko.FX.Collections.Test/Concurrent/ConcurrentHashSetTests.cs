using Gapotchenko.FX.Collections.Concurrent;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public void ConcurrentHashSet_NullKey()
        {
            var hashSet = new ConcurrentHashSet<string>();

            Assert.IsFalse(hashSet.Contains(null));

            hashSet.Add(null);
            Assert.IsTrue(hashSet.Contains(null));

            Assert.AreEqual(1, hashSet.Count);

            hashSet.Add("A");
            hashSet.Add("B");
            hashSet.Add("C");

            Assert.AreEqual(4, hashSet.Count);

            Assert.IsTrue(hashSet.Contains(null));
            Assert.IsTrue(hashSet.Contains("A"));
            Assert.IsTrue(hashSet.Contains("B"));
            Assert.IsTrue(hashSet.Contains("C"));
            Assert.IsFalse(hashSet.Contains("D"));
        }
    }
}
