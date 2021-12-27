using Gapotchenko.FX.Collections.Generic;
using System.Collections.Generic;
using Xunit;

namespace Gapotchenko.FX.Collections.Tests.Generic.AssociativeArray
{
    public class AssociativeArray_Tests_Misc
    {
        [Fact]
        public void AssociativeArray_NullTricks()
        {
            var array = new AssociativeArray<string?, string?>();
            Assert.False(array.ContainsKey(null));

            array.Add(null, null);
            Assert.True(array.ContainsKey(null));
            Assert.Null(array[null]);

            array[null] = string.Empty;
            Assert.Equal(string.Empty, array[null]);

            array.Remove(null);
            Assert.False(array.ContainsKey(null));

            array.AddRange(new[]
            {
                new KeyValuePair<string?, string?>(null, string.Empty),
                new KeyValuePair<string?, string?>(string.Empty, null),
            });
            Assert.Equal(2, array.Count);
            Assert.Equal(new[] { null, string.Empty }, array.Keys);
            Assert.Equal(new[] { string.Empty, null }, array.Values);
        }
    }
}
