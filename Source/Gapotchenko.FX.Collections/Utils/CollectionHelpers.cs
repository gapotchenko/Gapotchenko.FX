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
        catch (InvalidCastException)
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

    public static int GrowCapacity(int existingCapacity, int desiredCapacity, int defaultCapacity)
    {
        Debug.Assert(existingCapacity >= 0);
        Debug.Assert(desiredCapacity > existingCapacity);
        Debug.Assert(defaultCapacity >= 0);

        int newCapacity = existingCapacity == 0 ? defaultCapacity : existingCapacity * 2;

        int arrayMaxLength = ArrayHelpers.ArrayMaxLength;
        if ((uint)newCapacity > arrayMaxLength)
            newCapacity = arrayMaxLength;

        if (newCapacity < desiredCapacity)
            newCapacity = desiredCapacity;

        return newCapacity;
    }

    public static int TrimExcess(int capacity, int size)
    {
        Debug.Assert(capacity >= 0);
        Debug.Assert(size >= 0);
        Debug.Assert(size <= capacity);

        int threshold = (int)(capacity * 0.9);
        if (size < threshold)
            return size;
        else
            return capacity;
    }
}
