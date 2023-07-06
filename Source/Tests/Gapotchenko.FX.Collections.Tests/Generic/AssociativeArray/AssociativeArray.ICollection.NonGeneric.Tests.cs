using Gapotchenko.FX.Collections.Generic;
using Gapotchenko.FX.Collections.Tests.Bench;
using Gapotchenko.FX.Collections.Tests.Utils;
using System.Collections;

namespace Gapotchenko.FX.Collections.Tests.Generic.AssociativeArray;

public abstract class AssociativeArray_ICollection_NonGeneric_Tests<TKey, TValue> : ICollection_NonGeneric_Tests
{
    protected override bool DuplicateValuesAllowed => false;
    protected override bool Enumerator_Current_UndefinedOperation_Throws => true;
    protected override Type ICollection_NonGeneric_CopyTo_ArrayOfEnumType_ThrowType => typeof(ArgumentException);
    protected override Type ICollection_NonGeneric_CopyTo_IndexLargerThanArrayCount_ThrowType => typeof(ArgumentOutOfRangeException);

    protected override ICollection NonGenericICollectionFactory() => new AssociativeArray<TKey, TValue>();

    protected override void AddToCollection(ICollection collection, int numberOfItemsToAdd)
    {
        var dictionary = (IDictionary<TKey, TValue>)collection;

        int seed = 13453;
        for (int i = 0; i < numberOfItemsToAdd; ++i)
        {
            var entry = CreateT(seed++);
            while (dictionary.ContainsKey(entry.Key))
                entry = CreateT(seed++);

            dictionary.Add(entry);
        }
    }

    protected abstract KeyValuePair<TKey, TValue> CreateT(int seed);
}

public sealed class AssociativeArray_ICollection_NonGeneric_Tests_String_String : AssociativeArray_ICollection_NonGeneric_Tests<string, string>
{
    protected override KeyValuePair<string, string> CreateT(int seed)
    {
        var random = new Random(seed);
        return new(TestData.CreateString(random), TestData.CreateString(random));
    }
}

public sealed class AssociativeArray_ICollection_NonGeneric_Tests_Int32_Int32 : AssociativeArray_ICollection_NonGeneric_Tests<int, int>
{
    protected override KeyValuePair<int, int> CreateT(int seed)
    {
        var random = new Random(seed);
        return new(TestData.CreateInt32(random), TestData.CreateInt32(random));
    }
}
