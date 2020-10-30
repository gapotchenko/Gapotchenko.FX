using Gapotchenko.FX.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gapotchenko.FX.Collections.Tests.Generic
{
    [TestClass]
    public class ListExtensionsTests
    {
        class SortItem : IComparable<SortItem>
        {
            public int SortOrder { get; set; }
            public string? Name { get; set; }

            public int CompareTo(SortItem? other)
            {
                if (other is null)
                    return -1;
                return SortOrder.CompareTo(other.SortOrder);
            }
        }

        [TestMethod]
        public void GenericList_StableSort_1()
        {
            var list = new List<SortItem>
            {
                new SortItem{ SortOrder = 1, Name = "Name1"},
                new SortItem{ SortOrder = 2, Name = "Name2"},
                new SortItem{ SortOrder = 2, Name = "Name3"},
            };

            list.StableSort();

            Assert.AreEqual(3, list.Count);

            Assert.AreEqual(1, list[0].SortOrder);
            Assert.AreEqual("Name1", list[0].Name);

            Assert.AreEqual(2, list[1].SortOrder);
            Assert.AreEqual("Name2", list[1].Name);

            Assert.AreEqual(2, list[2].SortOrder);
            Assert.AreEqual("Name3", list[2].Name);
        }

        [TestMethod]
        public void GenericList_StableSort_2()
        {
            var list = new List<SortItem>
            {
                new SortItem{ SortOrder = 2, Name = "Name2"},
                new SortItem{ SortOrder = 1, Name = "Name1"},
                new SortItem{ SortOrder = 2, Name = "Name3"},
            };

            list.StableSort();

            Assert.AreEqual(3, list.Count);

            Assert.AreEqual(1, list[0].SortOrder);
            Assert.AreEqual("Name1", list[0].Name);

            Assert.AreEqual(2, list[1].SortOrder);
            Assert.AreEqual("Name2", list[1].Name);

            Assert.AreEqual(2, list[2].SortOrder);
            Assert.AreEqual("Name3", list[2].Name);
        }

        [TestMethod]
        public void GenericList_StableSort_Order()
        {
            var elements = new[] { 10, 5, 20 };

            var list1 = new List<int>(elements);
            var list2 = new List<int>(elements);

            list1.Sort();
            list2.StableSort();

            Assert.IsTrue(list1.SequenceEqual(list2));
        }
    }
}
