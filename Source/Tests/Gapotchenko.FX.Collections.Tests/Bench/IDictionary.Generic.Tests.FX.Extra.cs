using System.Collections;
using Xunit;

namespace Gapotchenko.FX.Collections.Tests.Bench;

partial class IDictionary_Generic_Tests<TKey, TValue>
{
    static void EnsureHasKey(IDictionary<TKey, TValue> dictionary, TKey key)
    {
        if (!dictionary.ContainsKey(key))
        {
            dictionary.Add(key, default!);
            Assert.True(dictionary.ContainsKey(key));
        }
    }

    [Theory]
    [MemberData(nameof(ValidCollectionSizes))]
    public void IDictionary_NonGeneric_Remove_RemovesDefaultKey(int count)
    {
        if (DefaultValueAllowed && !IsReadOnly)
        {
            var dictionary = GenericIDictionaryFactory(count);

            TKey defaultKey = default!;
            EnsureHasKey(dictionary, defaultKey);

            var iDictionary = (IDictionary)dictionary;
            iDictionary.Remove(defaultKey);

            Assert.False(dictionary.ContainsKey(defaultKey));
        }
    }

    [Theory]
    [MemberData(nameof(ValidCollectionSizes))]
    public void IDictionary_NonGeneric_Contains_FindsDefaultKey(int count)
    {
        if (DefaultValueAllowed && !IsReadOnly)
        {
            var dictionary = GenericIDictionaryFactory(count);

            TKey defaultKey = default!;
            EnsureHasKey(dictionary, defaultKey);

            var iDictionary = (IDictionary)dictionary;
            Assert.True(iDictionary.Contains(defaultKey));
        }
    }
}
