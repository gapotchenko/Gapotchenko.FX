using Gapotchenko.FX.Collections.Tests.Utils;

namespace Gapotchenko.FX.Collections.Tests.Generic.OrderedDictionary;

public sealed class OrderedDictionary_Tests_string_string : OrderedDictionary_Tests<string, string>
{
    protected override KeyValuePair<string, string> CreateT(int seed)
    {
        var random = new Random(seed);
        return new(TestData.CreateString(random), TestData.CreateString(random));
    }

    protected override string CreateTKey(int seed) => TestData.CreateString(new Random(seed));

    protected override string CreateTValue(int seed) => TestData.CreateString(new Random(seed));
}

public sealed class OrderedDictionary_Tests_int_int : OrderedDictionary_Tests<int, int>
{
    protected override KeyValuePair<int, int> CreateT(int seed)
    {
        var random = new Random(seed);
        return new(TestData.CreateInt32(random), TestData.CreateInt32(random));
    }

    protected override int CreateTKey(int seed) => TestData.CreateInt32(new Random(seed));

    protected override int CreateTValue(int seed) => TestData.CreateInt32(new Random(seed));
}

public sealed class OrderedDictionary_Tests_SimpleInt_int_With_Comparer_WrapStructural_SimpleInt : OrderedDictionary_Tests<SimpleInt, int>
{
    public override IEqualityComparer<SimpleInt> GetKeyIEqualityComparer() => new WrapStructural_SimpleInt();

    public override IComparer<SimpleInt> GetKeyIComparer() => new WrapStructural_SimpleInt();

    protected override KeyValuePair<SimpleInt, int> CreateT(int seed) =>
        new(CreateTKey(seed), CreateTValue(seed));

    protected override SimpleInt CreateTKey(int seed)
    {
        var random = new Random(seed);
        return new SimpleInt(random.Next());
    }

    protected override int CreateTValue(int seed) => TestData.CreateInt32(new Random(seed));
}
