using Gapotchenko.FX.Collections.Generic;
using Gapotchenko.FX.Collections.Tests.Bench;
using System.Collections;

namespace Gapotchenko.FX.Collections.Tests.Generic.Deque;

public abstract class Deque_IList_NonGeneric_Tests<T> : IList_NonGeneric_Tests
{
    protected override IList NonGenericIListFactory() => new Deque<T>();

    protected abstract override object CreateT(int seed);

    #region Test bench settings

    protected override bool Enumerator_Current_UndefinedOperation_Throws => true;

    protected override bool IList_CurrentAfterAdd_Throws => false;

    protected override Type ICollection_NonGeneric_CopyTo_NonZeroLowerBound_ThrowType => typeof(ArgumentOutOfRangeException);

    public override void ICollection_NonGeneric_CopyTo_ArrayOfEnumType(int count)
    {
    }

    public override void ICollection_NonGeneric_CopyTo_ArrayOfIncorrectValueType(int count)
    {
    }

    #endregion
}

public sealed class Deque_IList_NonGeneric_Tests_String : Deque_IList_NonGeneric_Tests<string>
{
    protected override object CreateT(int seed) => CreateString(new Random(seed));

    static string CreateString(Random random)
    {
        int length = random.Next(5, 15);
        var bytes = new byte[length];
        random.NextBytes(bytes);

        return Convert.ToBase64String(bytes);
    }
}

public sealed class Deque_IList_NonGeneric_Tests_Int32 : Deque_IList_NonGeneric_Tests<int>
{
    protected override object CreateT(int seed) => CreateInt32(new Random(seed));

    static int CreateInt32(Random random) => random.Next();

    protected override bool NullAllowed => false;
}
