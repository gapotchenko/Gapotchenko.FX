namespace Gapotchenko.FX.Collections.Tests.Generic.AssociativeArray;

public sealed class AssociativeArray_Tests_string_string : AssociativeArray_Tests<string, string>
{
    protected override bool DefaultValueAllowed => true;

    protected override KeyValuePair<string, string> CreateT(int seed)
    {
        var random = new Random(seed);
        return new(CreateString(random), CreateString(random));
    }

    protected override string CreateTKey(int seed) => CreateString(new Random(seed));

    protected override string CreateTValue(int seed) => CreateString(new Random(seed));

    static string CreateString(Random random)
    {
        int length = random.Next(5, 15);
        var bytes = new byte[length];
        random.NextBytes(bytes);

        return Convert.ToBase64String(bytes);
    }
}

public sealed class AssociativeArray_Tests_int_int : AssociativeArray_Tests<int, int>
{
    protected override bool DefaultValueAllowed => true;

    protected override KeyValuePair<int, int> CreateT(int seed)
    {
        var random = new Random(seed);
        return new(CreateInt32(random), CreateInt32(random));
    }

    protected override int CreateTKey(int seed) => CreateInt32(new Random(seed));

    protected override int CreateTValue(int seed) => CreateInt32(new Random(seed));

    static int CreateInt32(Random random) => random.Next();
}

public sealed class AssociativeArray_Tests_SimpleInt_int_With_Comparer_WrapStructural_SimpleInt : AssociativeArray_Tests<SimpleInt, int>
{
    protected override bool DefaultValueAllowed => true;

    public override IEqualityComparer<SimpleInt> GetKeyIEqualityComparer()
    {
        return new WrapStructural_SimpleInt();
    }

    public override IComparer<SimpleInt> GetKeyIComparer()
    {
        return new WrapStructural_SimpleInt();
    }

    protected override KeyValuePair<SimpleInt, int> CreateT(int seed) =>
        new(CreateTKey(seed), CreateTValue(seed));

    protected override SimpleInt CreateTKey(int seed)
    {
        var random = new Random(seed);
        return new SimpleInt(random.Next());
    }

    protected override int CreateTValue(int seed)
    {
        var random = new Random(seed);
        return random.Next();
    }
}
