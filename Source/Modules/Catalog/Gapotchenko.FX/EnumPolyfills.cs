// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

#if NET5_0_OR_GREATER
#define TFF_ENUM_GETVALUES_T
#endif

namespace Gapotchenko.FX;

/// <summary>
/// Provides polyfill extension members for <see cref="Enum"/> class.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class EnumPolyfills
{
    /// <inheritdoc cref="EnumPolyfills"/>
    extension(Enum)
    {
        /// <summary>
        /// Retrieves an array of the values of the constants in a specified enumeration type.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enumeration.</typeparam>
        /// <returns>
        /// An array that contains the values of the constants in <typeparamref name="TEnum"/>.
        /// </returns>
#if TFF_ENUM_GETVALUES_T
        [EditorBrowsable(EditorBrowsableState.Never)]
#endif
        public static TEnum[] GetValues<TEnum>() where TEnum : struct, Enum
        {
#if TFF_ENUM_GETVALUES_T
            return Enum.GetValues<TEnum>();
#else
            return (TEnum[])Enum.GetValues(typeof(TEnum));
#endif
        }
    }
}
