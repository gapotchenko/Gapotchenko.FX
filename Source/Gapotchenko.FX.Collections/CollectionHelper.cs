namespace Gapotchenko.FX.Collections;

static class CollectionHelper
{
    public static T GetCompatibleValue<T>(object? value, string parameterName)
    {
        Throw.IfNullAndNullsAreIllegal<T>(value, parameterName);
        try
        {
            return (T)value!;
        }
        catch (InvalidOperationException)
        {
            Throw.InvalidArgumentTypeException(value, typeof(T), parameterName);
            throw; // never reached
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
}
