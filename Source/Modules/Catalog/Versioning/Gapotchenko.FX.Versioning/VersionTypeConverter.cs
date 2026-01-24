// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

#if NETCOREAPP3_0_OR_GREATER
#define TFF_VERSIONCONVERTER
#endif

using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;

namespace Gapotchenko.FX.Versioning;

/// <summary>
/// Provides a type converter to convert <see cref="Version"/> objects to and from other representations.
/// </summary>
public class VersionTypeConverter :
#if TFF_VERSIONCONVERTER
    System.ComponentModel.VersionConverter
#else
    TypeConverter
#endif
{
    /// <summary>
    /// Ensures that a version converter is registered for the <see cref="Version"/> type.
    /// </summary>
    public static void Register()
    {
#if TFF_VERSIONCONVERTER
        // System.ComponentModel.VersionConverter is registered by default.
#else
        Registration.Activate();
#endif
    }

#if !TFF_VERSIONCONVERTER

    /// <inheritdoc/>
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        return
            sourceType == typeof(string) ||
            sourceType == typeof(Version) ||
            base.CanConvertFrom(context, sourceType);
    }

    /// <inheritdoc/>
    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        return value switch
        {
            string s => Version.Parse(s),
            Version version => new Version(version.Major, version.Minor, version.Build, version.Revision),
            _ => base.ConvertFrom(context, culture, value)
        };
    }

    /// <inheritdoc/>
    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
    {
        return
            destinationType == typeof(Version) ||
            destinationType == typeof(InstanceDescriptor) ||
            base.CanConvertTo(context, destinationType);
    }

    /// <inheritdoc/>
    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (value is Version version)
        {
            if (destinationType == typeof(string))
                return version.ToString();

            if (destinationType == typeof(Version))
                return new Version(version.Major, version.Minor, version.Build, version.Revision);

            if (destinationType == typeof(InstanceDescriptor))
            {
                var ctor = typeof(Version).GetConstructor(
                    BindingFlags.Public | BindingFlags.Instance,
                    null,
                    [typeof(int), typeof(int), typeof(int), typeof(int)],
                    null);
                Debug.Assert(ctor != null);

                return new InstanceDescriptor(
                    ctor,
                    new object[] { version.Major, version.Minor, version.Build, version.Revision });
            }
        }

        return base.ConvertTo(context, culture, value, destinationType);
    }

    static class Registration
    {
        static Registration()
        {
            var type = typeof(Version);
            if (TypeDescriptor.GetConverter(type).GetType() == typeof(TypeConverter)) // if no other converter is installed for the type,
                TypeDescriptor.AddAttributes(type, new TypeConverterAttribute(typeof(VersionTypeConverter))); // install this converter.
        }

        public static void Activate()
        {
        }
    }

#endif
}

#if BINARY_COMPATIBILITY || SOURCE_COMPATIBILITY // 2026

/// <inheritdoc/>
[Obsolete("Use VersionTypeConverter instead.")]
public class VersionConverter : VersionTypeConverter
{
}

#endif
