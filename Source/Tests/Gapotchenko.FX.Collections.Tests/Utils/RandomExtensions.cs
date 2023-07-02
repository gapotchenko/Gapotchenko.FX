// (c) Portions of code from the .NET project by Microsoft and .NET Foundation

namespace Gapotchenko.FX.Collections.Tests.Utils;

static class RandomExtensions
{
    public static void Shuffle<T>(this Random random, T[] array)
    {
        int n = array.Length;
        while (n > 1)
        {
            int k = random.Next(n--);
            T temp = array[n];
            array[n] = array[k];
            array[k] = temp;
        }
    }
}

