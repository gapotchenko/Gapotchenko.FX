using System.Runtime.CompilerServices;

namespace Gapotchenko.FX.Collections;

static class CollectionHelper
{
    public static T GetCompatibleValue<T>(
        object? value,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
        ExceptionHelper.ValidateNullArgumentLegality<T>(value, parameterName);

        try
        {
            return (T)value!;
        }
        catch (InvalidOperationException)
        {
            throw ExceptionHelper.CreateInvalidArgumentTypeException(value, typeof(T), parameterName);
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
