// Gapotchenko.FX
//
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
    /// <inheritdoc/>
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        if (sourceType == typeof(string))
            return true;
        else
            return base.CanConvertFrom(context, sourceType);
    }

    /// <inheritdoc/>
    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is string s)
            return Version.Parse(s);
        else
            return base.ConvertFrom(context, culture, value);
    }

    /// <inheritdoc/>
    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
    {
        if (destinationType == typeof(string))
            return true;
        else
            return base.CanConvertTo(context, destinationType);
    }

    /// <inheritdoc/>
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
        Registration.Activate();
    }

    static class Registration
    {
        static Registration()
        {
            var type = typeof(Version);
            if (TypeDescriptor.GetConverter(type).GetType() == typeof(TypeConverter)) // if no other converter is installed for the type,
                TypeDescriptor.AddAttributes(type, new TypeConverterAttribute(typeof(VersionConverter))); // install this converter.
        }

        public static void Activate()
        {
        }
    }
}
