namespace Gapotchenko.FX.Console;

static class AtomicNullable
{
    public static bool EnsureInitialized(ref AtomicNullableBool value, Func<bool> valueFactory)
    {
        var snapshot = value;
        if (snapshot == AtomicNullableBool.Null)
        {
            snapshot = valueFactory() ? AtomicNullableBool.True : AtomicNullableBool.False;
            value = snapshot;
            Thread.MemoryBarrier(); // freshness improvement
        }
        return snapshot == AtomicNullableBool.True;
    }
}
