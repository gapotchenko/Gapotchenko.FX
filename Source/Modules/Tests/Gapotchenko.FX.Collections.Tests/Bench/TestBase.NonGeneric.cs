// Portions © .NET Foundation and its Licensors

namespace Gapotchenko.FX.Collections.Tests.Bench;

#pragma warning disable IDE0040 // Add accessibility modifiers
#pragma warning disable IDE0059 // Unnecessary assignment of a value
#pragma warning disable IDE1006 // Naming Styles

#nullable disable

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
