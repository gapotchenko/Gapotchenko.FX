using System.Diagnostics;

namespace Gapotchenko.FX.Collections.Utils;

static class EnumerableHelper
{
    public static T[] ToArray<T>(IEnumerable<T> source)
    {
        Debug.Assert(source != null);

        if (source is ICollection<T> collection)
        {
            int count = collection.Count;
            if (count == 0)
            {
                return [];
            }
            else
            {
                var result = new T[count];
                collection.CopyTo(result, 0);
                return result;
            }
        }
        else
        {
            return source.ToArray();
        }
    }
}
