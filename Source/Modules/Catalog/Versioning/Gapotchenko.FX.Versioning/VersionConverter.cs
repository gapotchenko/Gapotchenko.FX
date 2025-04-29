// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using System.Globalization;

namespace Gapotchenko.FX.Versioning;

/// <summary>
/// Provides a type converter to convert <see cref="Version"/> objects to and from other representations.
/// </summary>
public sealed class VersionConverter : TypeConverter
{
    /// <summary>
    /// Determines if this converter can convert an object in the given source type to the native type of the converter.
    /// </summary>
    /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
    /// <param name="sourceType">A <see cref="Type"/> that represents the type from which you want to convert.</param>
    /// <returns>
    /// <see langword="true"/> if this converter can perform the operation;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        if (sourceType == typeof(string))
            return true;
        else
            return base.CanConvertFrom(context, sourceType);
    }

    /// <summary>
    /// Returns a value indicating whether this converter can convert an object to the given destination type using the context.
    /// </summary>
    /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
    /// <param name="destinationType">A <see cref="Type"/> that represents the type to which you want to convert.</param>
    /// <returns>
    /// <see langword="true"/> if this converter can perform the operation;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
    {
        if (destinationType == typeof(string))
            return true;
        else
            return base.CanConvertTo(context, destinationType);
    }

    /// <summary>
    /// Converts the given object to the converter's native type.
    /// </summary>
    /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
    /// <param name="culture">A <see cref="CultureInfo"/> that specifies the culture to represent the version.</param>
    /// <param name="value">The object to convert.</param>
    /// <returns>An <see cref="object"/> that represents the converted value.</returns>
    /// <exception cref="Exception"><paramref name="value"/> is not a valid value for the target type.</exception>
    /// <exception cref="NotSupportedException">The conversion cannot be performed.</exception>
    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value == null)
            return null;
        else if (value is string s)
            return Version.Parse(s);
        else
            return base.ConvertFrom(context, culture, value);
    }

    /// <summary>
    /// Converts the specified object to another type.
    /// </summary>
    /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
    /// <param name="culture">A <see cref="CultureInfo"/> that specifies the culture to represent the version.</param>
    /// <param name="value">The object to convert.</param>
    /// <param name="destinationType">The type to convert the object to.</param>
    /// <returns>An <see cref="object"/> that represents the converted value.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="destinationType"/> is <see langword="null"/>.</exception>
    /// <exception cref="NotSupportedException">The conversion cannot be performed.</exception>
    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (destinationType == typeof(string))
            return ((Version?)value)?.ToString();
        else
            return base.ConvertTo(context, culture, value, destinationType);
    }

    /// <summary>
    /// Ensures that <see cref="VersionConverter"/> is registered for <see cref="Version"/> type.
    /// </summary>
    public static void Register()
    {
        var type = typeof(Version);
        if (TypeDescriptor.GetConverter(type).GetType() == typeof(TypeConverter))
            TypeDescriptor.AddAttributes(type, new TypeConverterAttribute(typeof(VersionConverter)));
    }
}
