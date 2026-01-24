// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2026

using Gapotchenko.FX.Math.Intervals.Properties;
using Gapotchenko.FX.Math.Intervals.Utils;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;

using static Gapotchenko.FX.Math.Intervals.Utils.ReflectionHelper;

namespace Gapotchenko.FX.Math.Intervals;

/// <summary>
/// Provides a type converter to convert <see cref="Interval{T}"/> objects to and from other representations.
/// </summary>
public class IntervalTypeConverter() : IntervalTypeConverterBase(typeof(Interval<>))
{
    private protected sealed override object Parse(Type specializingType, string s, IFormatProvider? provider)
    {
        MethodInfo parseMethodDefinition = (MethodOf<Func<string, IFormatProvider, IComparer<Unit>, Interval<Unit>>>)Interval.Parse;
        parseMethodDefinition = parseMethodDefinition.GetGenericMethodDefinition();
        var parseMethod = parseMethodDefinition.MakeGenericMethod(specializingType);
        return parseMethod.Invoke(null, [s, provider, null])!;
    }

    private protected sealed override MethodInfo GetFactoryMethod(IntervalBoundaryKind from, IntervalBoundaryKind to)
    {
        var method =
            (from, to) switch
            {
                (IntervalBoundaryKind.Inclusive, IntervalBoundaryKind.Inclusive) => (MethodOf<Func<Unit, Unit, IComparer<Unit>, Interval<Unit>>>)Interval.Inclusive,
                (IntervalBoundaryKind.Exclusive, IntervalBoundaryKind.Exclusive) => (MethodOf<Func<Unit, Unit, IComparer<Unit>, Interval<Unit>>>)Interval.Exclusive,
                (IntervalBoundaryKind.Inclusive, IntervalBoundaryKind.Exclusive) => (MethodOf<Func<Unit, Unit, IComparer<Unit>, Interval<Unit>>>)Interval.InclusiveExclusive,
                (IntervalBoundaryKind.Exclusive, IntervalBoundaryKind.Inclusive) => (MethodOf<Func<Unit, Unit, IComparer<Unit>, Interval<Unit>>>)Interval.ExclusiveInclusive,
                (IntervalBoundaryKind.NegativeInfinity, IntervalBoundaryKind.PositiveInfinity) => MethodOf(() => Interval.Infinite<Unit>()),
                (IntervalBoundaryKind.Inclusive, IntervalBoundaryKind.PositiveInfinity) => (MethodOf<Func<Unit, IComparer<Unit>, Interval<Unit>>>)Interval.FromInclusive,
                (IntervalBoundaryKind.Exclusive, IntervalBoundaryKind.PositiveInfinity) => (MethodOf<Func<Unit, IComparer<Unit>, Interval<Unit>>>)Interval.FromExclusive,
                (IntervalBoundaryKind.NegativeInfinity, IntervalBoundaryKind.Inclusive) => (MethodOf<Func<Unit, IComparer<Unit>, Interval<Unit>>>)Interval.ToInclusive,
                (IntervalBoundaryKind.NegativeInfinity, IntervalBoundaryKind.Exclusive) => (MethodOf<Func<Unit, IComparer<Unit>, Interval<Unit>>>)Interval.ToExclusive,
                _ => throw new NotSupportedException(Resources.UnsupportedBoundaryKindCombination)
            };
        return method.GetGenericMethodDefinition();
    }
}

/// <summary>
/// Provides a type converter to convert <see cref="ValueInterval{T}"/> objects to and from other representations.
/// </summary>
public class ValueIntervalTypeConverter() : IntervalTypeConverterBase(typeof(ValueInterval<>))
{
    private protected sealed override object Parse(Type specializingType, string s, IFormatProvider? provider)
    {
        var parseMethodDefinition = MethodOf(() => ValueInterval.Parse<Unit>(string.Empty, null));
        parseMethodDefinition = parseMethodDefinition.GetGenericMethodDefinition();
        var parseMethod = parseMethodDefinition.MakeGenericMethod(specializingType);
        return parseMethod.Invoke(null, [s, provider])!;
    }

    private protected sealed override MethodInfo GetFactoryMethod(IntervalBoundaryKind from, IntervalBoundaryKind to)
    {
        var method =
            (from, to) switch
            {
                (IntervalBoundaryKind.Inclusive, IntervalBoundaryKind.Inclusive) => (MethodOf<Func<Unit, Unit, ValueInterval<Unit>>>)ValueInterval.Inclusive,
                (IntervalBoundaryKind.Exclusive, IntervalBoundaryKind.Exclusive) => (MethodOf<Func<Unit, Unit, ValueInterval<Unit>>>)ValueInterval.Exclusive,
                (IntervalBoundaryKind.Inclusive, IntervalBoundaryKind.Exclusive) => (MethodOf<Func<Unit, Unit, ValueInterval<Unit>>>)ValueInterval.InclusiveExclusive,
                (IntervalBoundaryKind.Exclusive, IntervalBoundaryKind.Inclusive) => (MethodOf<Func<Unit, Unit, ValueInterval<Unit>>>)ValueInterval.ExclusiveInclusive,
                (IntervalBoundaryKind.NegativeInfinity, IntervalBoundaryKind.PositiveInfinity) => MethodOf(() => ValueInterval.Infinite<Unit>()),
                (IntervalBoundaryKind.Inclusive, IntervalBoundaryKind.PositiveInfinity) => (MethodOf<Func<Unit, ValueInterval<Unit>>>)ValueInterval.FromInclusive,
                (IntervalBoundaryKind.Exclusive, IntervalBoundaryKind.PositiveInfinity) => (MethodOf<Func<Unit, ValueInterval<Unit>>>)ValueInterval.FromExclusive,
                (IntervalBoundaryKind.NegativeInfinity, IntervalBoundaryKind.Inclusive) => (MethodOf<Func<Unit, ValueInterval<Unit>>>)ValueInterval.ToInclusive,
                (IntervalBoundaryKind.NegativeInfinity, IntervalBoundaryKind.Exclusive) => (MethodOf<Func<Unit, ValueInterval<Unit>>>)ValueInterval.ToExclusive,
                _ => throw new NotSupportedException(Resources.UnsupportedBoundaryKindCombination)
            };
        return method.GetGenericMethodDefinition();
    }
}

/// <summary>
/// This is an infrastructure type that should never be used by user code.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public abstract class IntervalTypeConverterBase : TypeConverter
{
    private protected IntervalTypeConverterBase(Type intervalType)
    {
        m_IntervalType = intervalType;
    }

    /// <inheritdoc/>
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        return
            sourceType == typeof(string) ||
            sourceType.IsGenericType && sourceType.GetGenericTypeDefinition() == m_IntervalType ||
            base.CanConvertFrom(context, sourceType);
    }

    /// <inheritdoc/>
    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        return
            value switch
            {
                string s => Parse(GetSpecializingType(context), s, culture),
                _ => base.ConvertFrom(context, culture, value)
            };
    }

    /// <inheritdoc/>
    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
    {
        if (destinationType is not null)
        {
            if (destinationType == typeof(InstanceDescriptor) ||
                destinationType.IsGenericType && destinationType.GetGenericTypeDefinition() == m_IntervalType)
            {
                return true;
            }
        }

        return base.CanConvertTo(context, destinationType);
    }

    /// <inheritdoc/>
    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (value is IInterval interval)
        {
            if (destinationType.IsGenericType && destinationType.GetGenericTypeDefinition() == m_IntervalType)
                return ((ICloneableInterval)interval).CloneInterval();

            if (destinationType == typeof(InstanceDescriptor))
            {
                var method = GetFactoryMethod(interval.From.Kind, interval.To.Kind);

                var parameters = method.GetParameters();
                bool hasComparer = parameters is [.., var last] && last.ParameterType == typeof(IComparer<>);

                var arguments = new List<object?>(3);
                switch (parameters.Length - (hasComparer ? 1 : 0))
                {
                    case 0:
                        break;
                    case 1:
                        arguments.Add(interval.From.HasValue ? interval.From.Value : interval.To.Value);
                        break;
                    case 2:
                        arguments.Add(interval.From.Value);
                        arguments.Add(interval.To.Value);
                        break;
                    default:
                        throw new NotSupportedException("Cannot construct interval method arguments.");
                }
                if (hasComparer)
                    arguments.Add(null);

                method = method.MakeGenericMethod(GetSpecializingType(context));

                return new InstanceDescriptor(method, arguments);
            }
        }

        return base.ConvertTo(context, culture, value, destinationType);
    }

    /// <summary>
    /// Gets an interval specializing type from the specified context.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns>A type that specializes the generic interval type.</returns>
    /// <exception cref="NotSupportedException">No suitable destination type is specified by the context.</exception>
    protected Type GetSpecializingType(ITypeDescriptorContext? context)
    {
        return
            TryGetSpecializingType(context) ??
            throw new NotSupportedException("No suitable destination type is specified by the context.");
    }

    /// <summary>
    /// Tries to gets an interval specializing type from the specified context.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns>A type that specializes the generic interval type.</returns>
    protected Type? TryGetSpecializingType(ITypeDescriptorContext? context)
    {
        var type = context?.PropertyDescriptor?.PropertyType;
        if (type is null)
            return null;
        if (!type.IsGenericType)
            return null;
        var typeDefinition = type.GetGenericTypeDefinition();
        if (typeDefinition != m_IntervalType)
            return null;
        return type.GetGenericArguments()[0];
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    readonly Type m_IntervalType;

    private protected abstract object Parse(Type specializingType, string s, IFormatProvider? provider);

    private protected abstract MethodInfo GetFactoryMethod(IntervalBoundaryKind from, IntervalBoundaryKind to);
}
