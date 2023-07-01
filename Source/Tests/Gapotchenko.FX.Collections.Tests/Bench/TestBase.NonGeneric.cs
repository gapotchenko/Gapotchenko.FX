// (c) Portions of code from the .NET project by Microsoft and .NET Foundation

namespace Gapotchenko.FX.Collections.Tests.Bench;

/// <summary>
/// Provides a base set of nongeneric operations that are used by all other testing interfaces.
/// </summary>
public abstract class TestBase
{
    #region Helper Methods

    public static IEnumerable<object[]> ValidCollectionSizes()
    {
        yield return new object[] { 0 };
        yield return new object[] { 1 };
        yield return new object[] { 75 };
    }

    public static IEnumerable<object[]> ValidPositiveCollectionSizes()
    {
        yield return new object[] { 1 };
        yield return new object[] { 75 };
    }

    public enum EnumerableType
    {
        HashSet,
        SortedSet,
        List,
        Queue,
        Lazy,
    };

    [Flags]
    public enum ModifyOperation
    {
        None = 0,
        Add = 1,
        Insert = 2,
        Overwrite = 4,
        Remove = 8,
        Clear = 16
    }

    #endregion
}
