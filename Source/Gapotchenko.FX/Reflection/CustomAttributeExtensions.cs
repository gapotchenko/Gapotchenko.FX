using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

#if !TFF_CUSTOM_ATTRIBUTE_EXTENSIONS

namespace System.Reflection
{
    /// <summary>
    /// <para>
    /// Contains static methods for retrieving custom attributes.
    /// </para>
    /// <para>
    /// This is a polyfill provided by Gapotchenko.FX.
    /// </para>
    /// </summary>
    public static class CustomAttributeExtensions
    {
        /// <summary>
        /// Retrieves a custom attribute of a specified type that is applied to a specified assembly.
        /// </summary>
        /// <param name="element">The assembly to inspect.</param>
        /// <param name="attributeType">The type of attribute to search for.</param>
        /// <returns>A custom attribute that matches <paramref name="attributeType"/>, or null if no such attribute is found.</returns>
        public static Attribute GetCustomAttribute(this Assembly element, Type attributeType) => Attribute.GetCustomAttribute(element, attributeType);

        /// <summary>
        /// Retrieves a custom attribute of a specified type that is applied to a specified module.
        /// </summary>
        /// <param name="element">The module to inspect.</param>
        /// <param name="attributeType">The type of attribute to search for.</param>
        /// <returns>A custom attribute that matches <paramref name="attributeType"/>, or null if no such attribute is found.</returns>
        public static Attribute GetCustomAttribute(this Module element, Type attributeType) => Attribute.GetCustomAttribute(element, attributeType);

        /// <summary>
        /// Retrieves a custom attribute of a specified type that is applied to a specified member.
        /// </summary>
        /// <param name="element">The member to inspect.</param>
        /// <param name="attributeType">The type of attribute to search for.</param>
        /// <returns>A custom attribute that matches <paramref name="attributeType"/>, or null if no such attribute is found.</returns>
        public static Attribute GetCustomAttribute(this MemberInfo element, Type attributeType) => Attribute.GetCustomAttribute(element, attributeType);

        /// <summary>
        /// Retrieves a custom attribute of a specified type that is applied to a specified parameter.
        /// </summary>
        /// <param name="element">The parameter to inspect.</param>
        /// <param name="attributeType">The type of attribute to search for.</param>
        /// <returns>A custom attribute that matches <paramref name="attributeType"/>, or null if no such attribute is found.</returns>
        public static Attribute GetCustomAttribute(this ParameterInfo element, Type attributeType) => Attribute.GetCustomAttribute(element, attributeType);

        /// <summary>
        /// Retrieves a custom attribute of a specified type that is applied to a specified assembly.
        /// </summary>
        /// <typeparam name="T">The type of attribute to search for.</typeparam>
        /// <param name="element">The assembly to inspect.</param>
        /// <returns>A custom attribute that matches <typeparamref name="T"/>, or null if no such attribute is found.</returns>
        public static T GetCustomAttribute<T>(this Assembly element) where T : Attribute => (T)GetCustomAttribute(element, typeof(T));

        /// <summary>
        /// Retrieves a custom attribute of a specified type that is applied to a specified module.
        /// </summary>
        /// <typeparam name="T">The type of attribute to search for.</typeparam>
        /// <param name="element">The module to inspect.</param>
        /// <returns>A custom attribute that matches <typeparamref name="T"/>, or null if no such attribute is found.</returns>
        public static T GetCustomAttribute<T>(this Module element) where T : Attribute => (T)GetCustomAttribute(element, typeof(T));

        /// <summary>
        /// Retrieves a custom attribute of a specified type that is applied to a specified member.
        /// </summary>
        /// <typeparam name="T">The type of attribute to search for.</typeparam>
        /// <param name="element">The member to inspect.</param>
        /// <returns>A custom attribute that matches <typeparamref name="T"/>, or null if no such attribute is found.</returns>
        public static T GetCustomAttribute<T>(this MemberInfo element) where T : Attribute => (T)GetCustomAttribute(element, typeof(T));

        /// <summary>
        /// Retrieves a custom attribute of a specified type that is applied to a specified parameter.
        /// </summary>
        /// <typeparam name="T">The type of attribute to search for.</typeparam>
        /// <param name="element">The parameter to inspect.</param>
        /// <returns>A custom attribute that matches <typeparamref name="T"/>, or null if no such attribute is found.</returns>
        public static T GetCustomAttribute<T>(this ParameterInfo element) where T : Attribute => (T)GetCustomAttribute(element, typeof(T));

        /// <summary>
        /// Retrieves a custom attribute of a specified type that is applied to a specified member,
        /// and optionally inspects the ancestors of that member.
        /// </summary>
        /// <param name="element">The member to inspect.</param>
        /// <param name="attributeType">The type of attribute to search for.</param>
        /// <param name="inherit"><c>true</c> to inspect the ancestors of element; otherwise, <c>false</c>.</param>
        /// <returns>A custom attribute that matches <paramref name="attributeType"/>, or null if no such attribute is found.</returns>
        public static Attribute GetCustomAttribute(this MemberInfo element, Type attributeType, bool inherit) => Attribute.GetCustomAttribute(element, attributeType, inherit);

        /// <summary>
        /// Retrieves a custom attribute of a specified type that is applied to a specified parameter,
        /// and optionally inspects the ancestors of that parameter.
        /// </summary>
        /// <param name="element">The parameter to inspect.</param>
        /// <param name="attributeType">The type of attribute to search for.</param>
        /// <param name="inherit"><c>true</c> to inspect the ancestors of element; otherwise, <c>false</c>.</param>
        /// <returns>A custom attribute that matches <paramref name="attributeType"/>, or null if no such attribute is found.</returns>
        public static Attribute GetCustomAttribute(this ParameterInfo element, Type attributeType, bool inherit) => Attribute.GetCustomAttribute(element, attributeType, inherit);

        /// <summary>
        /// Retrieves a custom attribute of a specified type that is applied to a specified member,
        /// and optionally inspects the ancestors of that member.
        /// </summary>
        /// <typeparam name="T">The type of attribute to search for.</typeparam>
        /// <param name="element">The member to inspect.</param>
        /// <param name="inherit"><c>true</c> to inspect the ancestors of element; otherwise, <c>false</c>.</param>
        /// <returns>A custom attribute that matches <typeparamref name="T"/>, or null if no such attribute is found.</returns>
        public static T GetCustomAttribute<T>(this MemberInfo element, bool inherit) where T : Attribute => (T)GetCustomAttribute(element, typeof(T), inherit);

        /// <summary>
        /// Retrieves a custom attribute of a specified type that is applied to a specified parameter,
        /// and optionally inspects the ancestors of that parameter.
        /// </summary>
        /// <typeparam name="T">The type of attribute to search for.</typeparam>
        /// <param name="element">The parameter to inspect.</param>
        /// <param name="inherit"><c>true</c> to inspect the ancestors of element; otherwise, <c>false</c>.</param>
        /// <returns>A custom attribute that matches <typeparamref name="T"/>, or null if no such attribute is found.</returns>
        public static T GetCustomAttribute<T>(this ParameterInfo element, bool inherit) where T : Attribute => (T)GetCustomAttribute(element, typeof(T), inherit);

        /// <summary>
        /// Retrieves a collection of custom attributes that are applied to a specified assembly.
        /// </summary>
        /// <param name="element">The assembly to inspect.</param>
        /// <returns>
        /// A collection of the custom attributes that are applied to element,
        /// or an empty collection if no such attributes exist.
        /// </returns>
        public static IEnumerable<Attribute> GetCustomAttributes(this Assembly element) => Attribute.GetCustomAttributes(element);

        /// <summary>
        /// Retrieves a collection of custom attributes that are applied to a specified module.
        /// </summary>
        /// <param name="element">The module to inspect.</param>
        /// <returns>
        /// A collection of the custom attributes that are applied to element,
        /// or an empty collection if no such attributes exist.
        /// </returns>
        public static IEnumerable<Attribute> GetCustomAttributes(this Module element) => Attribute.GetCustomAttributes(element);

        /// <summary>
        /// Retrieves a collection of custom attributes that are applied to a specified member.
        /// </summary>
        /// <param name="element">The member to inspect.</param>
        /// <returns>
        /// A collection of the custom attributes that are applied to element,
        /// or an empty collection if no such attributes exist.
        /// </returns>
        public static IEnumerable<Attribute> GetCustomAttributes(this MemberInfo element) => Attribute.GetCustomAttributes(element);

        /// <summary>
        /// Retrieves a collection of custom attributes that are applied to a specified parameter.
        /// </summary>
        /// <param name="element">The parameter to inspect.</param>
        /// <returns>
        /// A collection of the custom attributes that are applied to element,
        /// or an empty collection if no such attributes exist.
        /// </returns>
        public static IEnumerable<Attribute> GetCustomAttributes(this ParameterInfo element) => Attribute.GetCustomAttributes(element);

        /// <summary>
        /// Retrieves a collection of custom attributes that are applied to a specified member,
        /// and optionally inspects the ancestors of that member.
        /// </summary>
        /// <param name="element">The member to inspect.</param>
        /// <param name="inherit"><c>true</c> to inspect the ancestors of element; otherwise, <c>false</c>.</param>
        /// <returns>
        /// A collection of the custom attributes that are applied to element that match the specified criteria,
        /// or an empty collection if no such attributes exist.
        /// </returns>
        public static IEnumerable<Attribute> GetCustomAttributes(this MemberInfo element, bool inherit) => Attribute.GetCustomAttributes(element, inherit);

        /// <summary>
        /// Retrieves a collection of custom attributes that are applied to a specified parameter,
        /// and optionally inspects the ancestors of that parameter.
        /// </summary>
        /// <param name="element">The parameter to inspect.</param>
        /// <param name="inherit"><c>true</c> to inspect the ancestors of element; otherwise, <c>false</c>.</param>
        /// <returns>
        /// A collection of the custom attributes that are applied to element that match the specified criteria,
        /// or an empty collection if no such attributes exist.
        /// </returns>
        public static IEnumerable<Attribute> GetCustomAttributes(this ParameterInfo element, bool inherit) => Attribute.GetCustomAttributes(element, inherit);

        /// <summary>
        /// Retrieves a collection of custom attributes of a specified type that are applied to a specified assembly.
        /// </summary>
        /// <param name="element">The assembly to inspect.</param>
        /// <param name="attributeType">The type of attribute to search for.</param>
        /// <returns>
        /// A collection of the custom attributes that are applied to element and that match <paramref name="attributeType"/>,
        /// or an empty collection if no such attributes exist.
        /// </returns>
        public static IEnumerable<Attribute> GetCustomAttributes(this Assembly element, Type attributeType) => Attribute.GetCustomAttributes(element, attributeType);

        /// <summary>
        /// Retrieves a collection of custom attributes of a specified type that are applied to a specified module.
        /// </summary>
        /// <param name="element">The module to inspect.</param>
        /// <param name="attributeType">The type of attribute to search for.</param>
        /// <returns>
        /// A collection of the custom attributes that are applied to element and that match <paramref name="attributeType"/>,
        /// or an empty collection if no such attributes exist.
        /// </returns>
        public static IEnumerable<Attribute> GetCustomAttributes(this Module element, Type attributeType) => Attribute.GetCustomAttributes(element, attributeType);

        /// <summary>
        /// Retrieves a collection of custom attributes of a specified type that are applied to a specified member.
        /// </summary>
        /// <param name="element">The member to inspect.</param>
        /// <param name="attributeType">The type of attribute to search for.</param>
        /// <returns>
        /// A collection of the custom attributes that are applied to element and that match <paramref name="attributeType"/>,
        /// or an empty collection if no such attributes exist.
        /// </returns>
        public static IEnumerable<Attribute> GetCustomAttributes(this MemberInfo element, Type attributeType) => Attribute.GetCustomAttributes(element, attributeType);

        /// <summary>
        /// Retrieves a collection of custom attributes of a specified type that are applied to a specified parameter.
        /// </summary>
        /// <param name="element">The parameter to inspect.</param>
        /// <param name="attributeType">The type of attribute to search for.</param>
        /// <returns>
        /// A collection of the custom attributes that are applied to element and that match <paramref name="attributeType"/>,
        /// or an empty collection if no such attributes exist.
        /// </returns>
        public static IEnumerable<Attribute> GetCustomAttributes(this ParameterInfo element, Type attributeType) => Attribute.GetCustomAttributes(element, attributeType);

        /// <summary>
        /// Retrieves a collection of custom attributes of a specified type that are applied to a specified assembly.
        /// </summary>
        /// <typeparam name="T">The type of attribute to search for.</typeparam>
        /// <param name="element">The assembly to inspect.</param>
        /// <returns>
        /// A collection of the custom attributes that are applied to element and that match <typeparamref name="T"/>,
        /// or an empty collection if no such attributes exist.
        /// </returns>
        public static IEnumerable<T> GetCustomAttributes<T>(this Assembly element) where T : Attribute => (IEnumerable<T>)GetCustomAttributes(element, typeof(T));

        /// <summary>
        /// Retrieves a collection of custom attributes of a specified type that are applied to a specified module.
        /// </summary>
        /// <typeparam name="T">The type of attribute to search for.</typeparam>
        /// <param name="element">The module to inspect.</param>
        /// <returns>
        /// A collection of the custom attributes that are applied to element and that match <typeparamref name="T"/>,
        /// or an empty collection if no such attributes exist.
        /// </returns>
        public static IEnumerable<T> GetCustomAttributes<T>(this Module element) where T : Attribute => (IEnumerable<T>)GetCustomAttributes(element, typeof(T));

        /// <summary>
        /// Retrieves a collection of custom attributes of a specified type that are applied to a specified member.
        /// </summary>
        /// <typeparam name="T">The type of attribute to search for.</typeparam>
        /// <param name="element">The member to inspect.</param>
        /// <returns>
        /// A collection of the custom attributes that are applied to element and that match <typeparamref name="T"/>,
        /// or an empty collection if no such attributes exist.
        /// </returns>
        public static IEnumerable<T> GetCustomAttributes<T>(this MemberInfo element) where T : Attribute => (IEnumerable<T>)GetCustomAttributes(element, typeof(T));

        /// <summary>
        /// Retrieves a collection of custom attributes of a specified type that are applied to a specified parameter.
        /// </summary>
        /// <typeparam name="T">The type of attribute to search for.</typeparam>
        /// <param name="element">The parameter to inspect.</param>
        /// <returns>
        /// A collection of the custom attributes that are applied to element and that match <typeparamref name="T"/>,
        /// or an empty collection if no such attributes exist.
        /// </returns>
        public static IEnumerable<T> GetCustomAttributes<T>(this ParameterInfo element) where T : Attribute => (IEnumerable<T>)GetCustomAttributes(element, typeof(T));

        /// <summary>
        /// Retrieves a collection of custom attributes of a specified type that are applied to a specified member,
        /// and optionally inspects the ancestors of that member.
        /// </summary>
        /// <param name="element">The member to inspect.</param>
        /// <param name="attributeType">The type of attribute to search for.</param>
        /// <param name="inherit"><c>true</c> to inspect the ancestors of element; otherwise, <c>false</c>.</param>
        /// <returns>
        /// A collection of the custom attributes that are applied to element and that match <paramref name="attributeType"/>,
        /// or an empty collection if no such attributes exist.
        /// </returns>
        public static IEnumerable<Attribute> GetCustomAttributes(this MemberInfo element, Type attributeType, bool inherit) => Attribute.GetCustomAttributes(element, attributeType, inherit);

        /// <summary>
        /// Retrieves a collection of custom attributes of a specified type that are applied to a specified parameter,
        /// and optionally inspects the ancestors of that parameter.
        /// </summary>
        /// <param name="element">The parameter to inspect.</param>
        /// <param name="attributeType">The type of attribute to search for.</param>
        /// <param name="inherit"><c>true</c> to inspect the ancestors of element; otherwise, <c>false</c>.</param>
        /// <returns>
        /// A collection of the custom attributes that are applied to element and that match <paramref name="attributeType"/>,
        /// or an empty collection if no such attributes exist.
        /// </returns>
        public static IEnumerable<Attribute> GetCustomAttributes(this ParameterInfo element, Type attributeType, bool inherit) => Attribute.GetCustomAttributes(element, attributeType, inherit);

        /// <summary>
        /// Retrieves a collection of custom attributes of a specified type that are applied to a specified member,
        /// and optionally inspects the ancestors of that member.
        /// </summary>
        /// <typeparam name="T">The type of attribute to search for.</typeparam>
        /// <param name="element">The member to inspect.</param>
        /// <param name="inherit"><c>true</c> to inspect the ancestors of element; otherwise, <c>false</c>.</param>
        /// <returns>
        /// A collection of the custom attributes that are applied to element and that match <typeparamref name="T"/>,
        /// or an empty collection if no such attributes exist.
        /// </returns>
        public static IEnumerable<T> GetCustomAttributes<T>(this MemberInfo element, bool inherit) where T : Attribute => (IEnumerable<T>)GetCustomAttributes(element, typeof(T), inherit);

        /// <summary>
        /// Retrieves a collection of custom attributes of a specified type that are applied to a specified parameter,
        /// and optionally inspects the ancestors of that parameter.
        /// </summary>
        /// <typeparam name="T">The type of attribute to search for.</typeparam>
        /// <param name="element">The parameter to inspect.</param>
        /// <param name="inherit"><c>true</c> to inspect the ancestors of element; otherwise, <c>false</c>.</param>
        /// <returns>
        /// A collection of the custom attributes that are applied to element and that match <typeparamref name="T"/>,
        /// or an empty collection if no such attributes exist.
        /// </returns>
        public static IEnumerable<T> GetCustomAttributes<T>(this ParameterInfo element, bool inherit) where T : Attribute => (IEnumerable<T>)GetCustomAttributes(element, typeof(T), inherit);

        /// <summary>
        /// Indicates whether custom attributes of a specified type are applied to a specified assembly.
        /// </summary>
        /// <param name="element">The assembly to inspect.</param>
        /// <param name="attributeType">The type of the attribute to search for.</param>
        /// <returns><c>true</c> if an attribute of the specified type is applied to element; otherwise, <c>false</c>.</returns>
        public static bool IsDefined(this Assembly element, Type attributeType) => Attribute.IsDefined(element, attributeType);

        /// <summary>
        /// Indicates whether custom attributes of a specified type are applied to a specified module.
        /// </summary>
        /// <param name="element">The module to inspect.</param>
        /// <param name="attributeType">The type of the attribute to search for.</param>
        /// <returns><c>true</c> if an attribute of the specified type is applied to element; otherwise, <c>false</c>.</returns>
        public static bool IsDefined(this Module element, Type attributeType) => Attribute.IsDefined(element, attributeType);

        /// <summary>
        /// Indicates whether custom attributes of a specified type are applied to a specified member.
        /// </summary>
        /// <param name="element">The member to inspect.</param>
        /// <param name="attributeType">The type of the attribute to search for.</param>
        /// <returns><c>true</c> if an attribute of the specified type is applied to element; otherwise, <c>false</c>.</returns>
        public static bool IsDefined(this MemberInfo element, Type attributeType) => Attribute.IsDefined(element, attributeType);

        /// <summary>
        /// Indicates whether custom attributes of a specified type are applied to a specified parameter.
        /// </summary>
        /// <param name="element">The parameter to inspect.</param>
        /// <param name="attributeType">The type of the attribute to search for.</param>
        /// <returns><c>true</c> if an attribute of the specified type is applied to element; otherwise, <c>false</c>.</returns>
        public static bool IsDefined(this ParameterInfo element, Type attributeType) => Attribute.IsDefined(element, attributeType);

        /// <summary>
        /// Indicates whether custom attributes of a specified type are applied to a specified member,
        /// and, optionally, applied to its ancestors.
        /// </summary>
        /// <param name="element">The member to inspect.</param>
        /// <param name="attributeType">The type of the attribute to search for.</param>
        /// <param name="inherit"><c>true</c> to inspect the ancestors of element; otherwise, <c>false</c>.</param>
        /// <returns><c>true</c> if an attribute of the specified type is applied to element; otherwise, <c>false</c>.</returns>
        public static bool IsDefined(this MemberInfo element, Type attributeType, bool inherit) => Attribute.IsDefined(element, attributeType, inherit);

        /// <summary>
        /// Indicates whether custom attributes of a specified type are applied to a specified parameter,
        /// and, optionally, applied to its ancestors.
        /// </summary>
        /// <param name="element">The parameter to inspect.</param>
        /// <param name="attributeType">The type of the attribute to search for.</param>
        /// <param name="inherit"><c>true</c> to inspect the ancestors of element; otherwise, <c>false</c>.</param>
        /// <returns><c>true</c> if an attribute of the specified type is applied to element; otherwise, <c>false</c>.</returns>
        public static bool IsDefined(this ParameterInfo element, Type attributeType, bool inherit) => Attribute.IsDefined(element, attributeType, inherit);
    }
}

#else

[assembly: TypeForwardedTo(typeof(CustomAttributeExtensions))]

#endif
