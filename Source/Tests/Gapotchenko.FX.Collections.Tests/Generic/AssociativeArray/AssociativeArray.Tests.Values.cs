using Gapotchenko.FX.Collections.Generic;
using Gapotchenko.FX.Collections.Tests.NonGeneric;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Xunit;

namespace Gapotchenko.FX.Collections.Tests.Generic.AssociativeArray;

public class AssociativeArray_Tests_Values : ICollection_Generic_Tests<string>
{
    protected override bool DefaultValueAllowed => true;
    protected override bool DuplicateValuesAllowed => true;
    protected override bool IsReadOnly => true;
    protected override IEnumerable<ModifyEnumerable> GetModifyEnumerables(ModifyOperation operations) => new List<ModifyEnumerable>();

    protected override ICollection<string> GenericICollectionFactory()
    {
        return new AssociativeArray<string, string>().Values;
    }

    protected override ICollection<string> GenericICollectionFactory(int count)
    {
        var list = new AssociativeArray<string?, string>();
        int seed = 13453;
        for (int i = 0; i < count; i++)
            list.Add(i == 0 ? null : CreateT(seed++), CreateT(seed++));
        return list.Values;
    }

    protected override ICollection<string> GenericICollectionFactory(IEnumerable<string> elements)
    {
        var array = new AssociativeArray<string?, string>();
        int seed = 13453;
        var isFirstElement = true;
        foreach (var element in elements)
        {
            array.Add(isFirstElement ? null : CreateT(seed++), element);
            isFirstElement = false;
        }
        return array.Values;
    }

    protected override string CreateT(int seed)
    {
        int stringLength = seed % 10 + 5;
        Random rand = new Random(seed);
        byte[] bytes = new byte[stringLength];
        rand.NextBytes(bytes);
        return Convert.ToBase64String(bytes);
    }

    protected override Type ICollection_Generic_CopyTo_IndexLargerThanArrayCount_ThrowType => typeof(ArgumentOutOfRangeException);

    [Theory]
    [MemberData(nameof(ValidCollectionSizes))]
    public void AssociativeArray_ValueCollection_GetEnumerator(int count)
    {
        var array = new AssociativeArray<string, string>();
        int seed = 13453;
        while (array.Count < count)
            array.Add(CreateT(seed++), CreateT(seed++));
        array.Values.GetEnumerator();
    }
}

public class AssociativeArray_Tests_Values_AsICollection : ICollection_NonGeneric_Tests
{
    protected override bool NullAllowed => true;
    protected override bool DuplicateValuesAllowed => true;
    protected override bool IsReadOnly => true;
    protected override bool Enumerator_Current_UndefinedOperation_Throws => true;
    protected override Type ICollection_NonGeneric_CopyTo_ArrayOfEnumType_ThrowType => typeof(ArgumentException);
    protected override IEnumerable<ModifyEnumerable> GetModifyEnumerables(ModifyOperation operations) => new List<ModifyEnumerable>();
    protected override bool SupportsSerialization => false;

    protected override Type ICollection_NonGeneric_CopyTo_IndexLargerThanArrayCount_ThrowType => typeof(ArgumentOutOfRangeException);

    protected override ICollection NonGenericICollectionFactory()
    {
        return (ICollection)new AssociativeArray<string, string>().Values;
    }

    protected override ICollection NonGenericICollectionFactory(int count)
    {
        var array = new AssociativeArray<string, string>();
        int seed = 13453;
        for (int i = 0; i < count; i++)
            array.Add(CreateT(seed++), CreateT(seed++));
        return (ICollection)array.Values;
    }

    string CreateT(int seed)
    {
        int stringLength = seed % 10 + 5;
        Random rand = new Random(seed);
        byte[] bytes = new byte[stringLength];
        rand.NextBytes(bytes);
        return Convert.ToBase64String(bytes);
    }

    protected override void AddToCollection(ICollection collection, int numberOfItemsToAdd)
    {
        Debug.Assert(false);
    }

    [Theory]
    [MemberData(nameof(ValidCollectionSizes))]
    public void AssociativeArray_ValueCollection_CopyTo_ExactlyEnoughSpaceInTypeCorrectArray(int count)
    {
        ICollection collection = NonGenericICollectionFactory(count);
        string[] array = new string[count];
        collection.CopyTo(array, 0);
        int i = 0;
        foreach (var obj in collection)
            Assert.Equal(array[i++], obj);
    }
}
