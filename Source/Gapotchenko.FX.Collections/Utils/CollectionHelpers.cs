using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.Collections.Utils;

static class CollectionHelpers
{
    public static T GetCompatibleValue<T>(
        object? value,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
        ExceptionHelpers.ValidateNullArgumentLegality<T>(value, parameterName);

        try
        {
            return (T)value!;
        }
        catch (InvalidOperationException)
        {
            throw ExceptionHelpers.CreateInvalidArgumentTypeException(value, typeof(T), parameterName);
        }
    }

    public static bool TryGetCompatibleValue<T>(object? value, out T compatibleValue)
    {
        if (value is T t)
        {
            compatibleValue = t;
            return true;
        }
        else
        {
            compatibleValue = default!;
            return value is null && default(T) is null;
        }
    }

    public static void GrowCapacity<T>(ref T[] array, int capacity, int defaultCapacity)
    {
        Debug.Assert(array.Length < capacity);

        int newCapacity = array.Length == 0 ? defaultCapacity : array.Length * 2;

        int arrayMaxLength = ArrayHelpers.ArrayMaxLength;
        if ((uint)newCapacity > arrayMaxLength)
            newCapacity = arrayMaxLength;

        if (newCapacity < capacity)
            newCapacity = capacity;

        Array.Resize(ref array, newCapacity);
    }

    public static void TrimExcess<T>(ref T[] array, int size)
    {
        int threshold = (int)(array.Length * 0.9);
        if (size < threshold)
            Array.Resize(ref array, size);
    }
}
