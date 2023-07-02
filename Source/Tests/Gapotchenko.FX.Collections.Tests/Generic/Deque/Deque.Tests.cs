using Gapotchenko.FX.Collections.Generic;
using Gapotchenko.FX.Collections.Tests.Bench;

namespace Gapotchenko.FX.Collections.Tests.Generic.Deque;

public abstract class Deque_Tests<T> : IList_Generic_Tests<T>
{
    protected override IList<T> GenericIListFactory() => new Deque<T>();

    protected abstract override T CreateT(int seed);
}
