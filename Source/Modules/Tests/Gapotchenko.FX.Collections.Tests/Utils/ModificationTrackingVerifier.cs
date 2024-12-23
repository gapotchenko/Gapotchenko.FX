﻿using Xunit;

namespace Gapotchenko.FX.Collections.Tests.Utils;

static class ModificationTrackingVerifier
{
    public static bool IsModified<T>(IEnumerable<T> collection, Action action)
    {
        var enumerator = collection.GetEnumerator();

        action();

        try
        {
            // Check whether out-of-band modification logic is triggered.
            enumerator.MoveNext();
        }
        catch (InvalidOperationException)
        {
            return true;
        }
        return false;
    }

    public static void EnsureModified<T>(IEnumerable<T> collection, Action action)
    {
        var enumerator = collection.GetEnumerator();

        action();

        // Ensure that out-of-band modification logic is triggered.
        Assert.Throws<InvalidOperationException>(() => enumerator.MoveNext());
    }

    public static T EnsureModified<T>(IEnumerable<T> collection, Func<T> func)
    {
        T result = default!;
        EnsureModified(collection, new Action(() => result = func()));
        return result;
    }

    public static void EnsureNotModified<T>(IEnumerable<T> collection, Action action)
    {
        var data = collection.ToArray();
        var enumerator = collection.GetEnumerator();

        action();

        // Ensure that data remains the same.
        Assert.Equal(data, collection);

        // Ensure that out-of-band modification logic is not triggered.
        try
        {
            enumerator.MoveNext();
        }
        catch (InvalidOperationException)
        {
            Assert.Fail("Collection is out-of-band modified while it should not be.");
        }
    }
}
