using Gapotchenko.FX.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Gapotchenko.FX.Collections.Tests.Generic;

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

    [Fact]
    public void GenericList_StableSort_1()
    {
        var list = new List<SortItem>
        {
            new SortItem{ SortOrder = 1, Name = "Name1"},
            new SortItem{ SortOrder = 2, Name = "Name2"},
            new SortItem{ SortOrder = 2, Name = "Name3"},
        };

        list.StableSort();

        Assert.Equal(3, list.Count);

        Assert.Equal(1, list[0].SortOrder);
        Assert.Equal("Name1", list[0].Name);

        Assert.Equal(2, list[1].SortOrder);
        Assert.Equal("Name2", list[1].Name);

        Assert.Equal(2, list[2].SortOrder);
        Assert.Equal("Name3", list[2].Name);
    }

    [Fact]
    public void GenericList_StableSort_2()
    {
        var list = new List<SortItem>
        {
            new SortItem{ SortOrder = 2, Name = "Name2"},
            new SortItem{ SortOrder = 1, Name = "Name1"},
            new SortItem{ SortOrder = 2, Name = "Name3"},
        };

        list.StableSort();

        Assert.Equal(3, list.Count);

        Assert.Equal(1, list[0].SortOrder);
        Assert.Equal("Name1", list[0].Name);

        Assert.Equal(2, list[1].SortOrder);
        Assert.Equal("Name2", list[1].Name);

        Assert.Equal(2, list[2].SortOrder);
        Assert.Equal("Name3", list[2].Name);
    }

    [Fact]
    public void GenericList_StableSort_Order()
    {
        var elements = new[] { 10, 5, 20 };

        var list1 = new List<int>(elements);
        var list2 = new List<int>(elements);

        list1.Sort();
        list2.StableSort();

        Assert.True(list1.SequenceEqual(list2));
    }
}
