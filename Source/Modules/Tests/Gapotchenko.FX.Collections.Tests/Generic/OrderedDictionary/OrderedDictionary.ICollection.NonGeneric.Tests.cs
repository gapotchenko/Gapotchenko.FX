#if NET9_0_OR_GREATER

using Gapotchenko.FX.Collections.Tests.Bench;
using Gapotchenko.FX.Collections.Tests.Utils;
using System.Collections;
using Xunit;

namespace Gapotchenko.FX.Collections.Tests.Generic.OrderedDictionary;

[Trait("Category", "OrderedDictionary")]
public abstract class OrderedDictionary_ICollection_NonGeneric_Tests<TKey, TValue> : ICollection_NonGeneric_Tests
    where TKey : notnull
{
    #region Characteristics

    protected override bool DuplicateValuesAllowed => false;
    protected override bool Enumerator_Empty_UsesSingletonInstance => true;
    protected override Type ICollection_NonGeneric_CopyTo_ArrayOfEnumType_ThrowType => typeof(ArgumentException);

    #endregion

    #region Factories

    protected override ICollection NonGenericICollectionFactory() => new OrderedDictionary<TKey, TValue>();

    protected override void AddToCollection(ICollection collection, int numberOfItemsToAdd)
    {
        var dictionary = (IDictionary<TKey, TValue>)collection;

        int seed = 13453;
        for (int i = 0; i < numberOfItemsToAdd; ++i)
        {
            KeyValuePair<TKey, TValue> entry;
            do
                entry = CreateT(seed++);
            while (dictionary.ContainsKey(entry.Key));

            dictionary.Add(entry);
        }
    }

    protected abstract KeyValuePair<TKey, TValue> CreateT(int seed);

    #endregion

    public override void Enumerator_Current_AfterEndOfEnumerable_UndefinedBehavior(int count)
    {
    }

    public override void Enumerator_Current_BeforeFirstMoveNext_UndefinedBehavior(int count)
    {
    }
}

public sealed class OrderedDictionary_ICollection_NonGeneric_Tests_String_String : OrderedDictionary_ICollection_NonGeneric_Tests<string, string>
{
    protected override KeyValuePair<string, string> CreateT(int seed)
    {
        var random = new Random(seed);
        return new(TestData.CreateString(random), TestData.CreateString(random));
    }
}

public sealed class OrderedDictionary_ICollection_NonGeneric_Tests_Int32_Int32 : OrderedDictionary_ICollection_NonGeneric_Tests<int, int>
{
    protected override KeyValuePair<int, int> CreateT(int seed)
    {
        var random = new Random(seed);
        return new(TestData.CreateInt32(random), TestData.CreateInt32(random));
    }
}

#endif
