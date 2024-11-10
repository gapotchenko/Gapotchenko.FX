using Gapotchenko.FX.Collections.Tests.Bench;
using Xunit;

namespace Gapotchenko.FX.Collections.Tests.Generic.OrderedDictionary;

[Trait("Category", "OrderedDictionary")]
public abstract partial class OrderedDictionary_Tests<TKey, TValue> : IDictionary_Generic_Tests<TKey, TValue>
    where TKey : notnull
{
    #region Characteristics

    protected override bool DefaultValueAllowed => typeof(TKey).IsValueType;

    protected override bool Enumerator_Current_UndefinedOperation_Throws => false;
    protected override bool Enumerator_Empty_Current_UndefinedOperation_Throws => true;
    protected override bool Enumerator_Empty_ModifiedDuringEnumeration_ThrowsInvalidOperationException => false;
    protected override bool Enumerator_Empty_UsesSingletonInstance => true;

    protected override ModifyOperation ModifyEnumeratorAllowed => ModifyOperation.Overwrite;
    protected override ModifyOperation ModifyEnumeratorThrows =>
        ModifyOperation.Add | ModifyOperation.Insert | ModifyOperation.Remove | ModifyOperation.Clear;

    protected override Type ICollection_Generic_CopyTo_IndexLargerThanArrayCount_ThrowType => typeof(ArgumentException);

    #endregion

    #region Factories

    protected override IDictionary<TKey, TValue> GenericIDictionaryFactory() => new OrderedDictionary<TKey, TValue>();

    protected override IDictionary<TKey, TValue>? GenericIDictionaryFactory(IEqualityComparer<TKey> comparer) => new OrderedDictionary<TKey, TValue>(comparer);

    #endregion
}
