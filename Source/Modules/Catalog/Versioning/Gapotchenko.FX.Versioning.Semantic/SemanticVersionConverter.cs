// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;

namespace Gapotchenko.FX.Versioning;

/// <summary>
/// Provides a type converter to convert <see cref="SemanticVersion"/> objects to and from other representations.
/// </summary>
public class SemanticVersionConverter : TypeConverter
{
    /// <inheritdoc/>
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        return
            sourceType == typeof(string) ||
            sourceType == typeof(Version) ||
            sourceType == typeof(SemanticVersion) ||
            base.CanConvertFrom(context, sourceType);
    }

    /// <inheritdoc/>
    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        return value switch
        {
            string s => SemanticVersion.Parse(s),
            SemanticVersion semanticVersion => semanticVersion with { },
            Version version => new SemanticVersion(version),
            _ => base.ConvertFrom(context, culture, value)
        };
    }

    /// <inheritdoc/>
    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
    {
        return
            destinationType == typeof(Version) ||
            destinationType == typeof(SemanticVersion) ||
            destinationType == typeof(InstanceDescriptor) ||
            base.CanConvertTo(context, destinationType);
    }

    /// <inheritdoc/>
    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (value is SemanticVersion semanticVersion)
        {
            if (destinationType == typeof(string))
                return semanticVersion.ToString();

            if (destinationType == typeof(Version))
                return (Version)semanticVersion;

            if (destinationType == typeof(SemanticVersion))
                return semanticVersion with { };

            if (destinationType == typeof(InstanceDescriptor))
            {
                var ctor = typeof(SemanticVersion).GetConstructor(
                    BindingFlags.Public | BindingFlags.Instance,
                    null,
                    [typeof(int), typeof(int), typeof(int), typeof(string), typeof(string)],
                    null);
                Debug.Assert(ctor != null);

                return new InstanceDescriptor(
                    ctor,
                    new object?[] { semanticVersion.Major, semanticVersion.Minor, semanticVersion.Patch, semanticVersion.Prerelease, semanticVersion.Build });
            }
        }

        return base.ConvertTo(context, culture, value, destinationType);
    }
}
