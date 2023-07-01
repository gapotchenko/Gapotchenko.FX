using System.Collections;
using Xunit;

namespace Gapotchenko.FX.Collections.Tests.Bench;

partial class IDictionary_Generic_Tests<TKey, TValue>
{
    [Theory]
    [MemberData(nameof(ValidCollectionSizes))]
    public void IDictionary_NonGeneric_Remove_RemovesDefaultKey(int count)
    {
        if (DefaultValueAllowed && !IsReadOnly)
        {
            var dictionary = GenericIDictionaryFactory(count);

            TKey defaultKey = default!;
            if (!dictionary.ContainsKey(defaultKey))
            {
                dictionary.Add(defaultKey, default);
                Assert.True(dictionary.ContainsKey(defaultKey));
            }

            var iDictionary = (IDictionary)dictionary;
            iDictionary.Remove(defaultKey);

            Assert.False(dictionary.ContainsKey(defaultKey));
        }
    }
}
