using System.Collections;
using Xunit;

namespace Gapotchenko.FX.Collections.Tests.Bench;

partial class IDictionary_Generic_Tests<TKey, TValue>
{
    static void EnsureDictionaryHasKey(
        IDictionary<TKey, TValue> dictionary,
        TKey key,
        TValue valueToAdd = default!)
    {
        if (!dictionary.ContainsKey(key))
        {
            dictionary.Add(key, valueToAdd);
            Assert.True(dictionary.ContainsKey(key));
        }
    }

    [Theory]
    [MemberData(nameof(ValidCollectionSizes))]
    public void IDictionary_NonGeneric_Remove_RemovesByDefaultKey(int count)
    {
        if (DefaultValueAllowed && !IsReadOnly)
        {
            var dictionary = GenericIDictionaryFactory(count);

            TKey key = default!;
            EnsureDictionaryHasKey(dictionary, key);

            var iDictionary = (IDictionary)dictionary;
            iDictionary.Remove(key);

            Assert.False(dictionary.ContainsKey(key));
        }
    }

    [Theory]
    [MemberData(nameof(ValidCollectionSizes))]
    public void IDictionary_NonGeneric_Contains_FindsByDefaultKey(int count)
    {
        if (DefaultValueAllowed && !IsReadOnly)
        {
            var dictionary = GenericIDictionaryFactory(count);

            TKey key = default!;
            EnsureDictionaryHasKey(dictionary, key);

            var iDictionary = (IDictionary)dictionary;
            Assert.True(iDictionary.Contains(key));
        }
    }

    [Theory]
    [MemberData(nameof(ValidCollectionSizes))]
    public void IDictionary_NonGeneric_Indexer_GetsByDefaultKey(int count)
    {
        if (DefaultValueAllowed && !IsReadOnly && count > 0)
        {
            var dictionary = GenericIDictionaryFactory(count);

            TKey key = default!;
            var value = dictionary.First().Value;

            EnsureDictionaryHasKey(dictionary, key, value);

            var iDictionary = (IDictionary)dictionary;
            Assert.Equal(value, iDictionary[key]);
        }
    }
}
