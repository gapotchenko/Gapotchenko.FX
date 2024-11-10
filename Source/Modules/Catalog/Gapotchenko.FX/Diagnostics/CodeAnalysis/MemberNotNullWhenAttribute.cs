﻿// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2020

#if !TFF_MEMBERNOTNULLATTRIBUTE

#pragma warning disable IDE0130 // Namespace does not match folder structure

namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// Specifies that the method or property will ensure that the listed field and property members have not-null values.
/// </summary>
/// <remarks>
/// This is a polyfill provided by Gapotchenko.FX.
/// </remarks>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
public sealed class MemberNotNullWhenAttribute : Attribute
{
    /// <summary>
    /// Initializes the attribute with the specified return value condition and a field or property member.
    /// </summary>
    /// <param name="returnValue">
    /// The return value condition.
    /// If the method returns this value, the associated parameter will not be null.
    /// </param>
    /// <param name="member">The field or property member that is promised to be not-null.</param>
    public MemberNotNullWhenAttribute(bool returnValue, string member)
    {
        ReturnValue = returnValue;
        Members = [member];
    }

    /// <summary>
    /// Initializes the attribute with the specified return value condition and list of field and property members.
    /// </summary>
    /// <param name="returnValue">
    /// The return value condition.
    /// If the method returns this value, the associated parameter will not be null.
    /// </param>
    /// <param name="members">The list of field and property members that are promised to be not-null.</param>
    public MemberNotNullWhenAttribute(bool returnValue, params string[] members)
    {
        ReturnValue = returnValue;
        Members = members;
    }

    /// <summary>
    /// Gets the return value condition.
    /// </summary>
    public bool ReturnValue { get; }

    /// <summary>
    /// Gets field or property member names.
    /// </summary>
    public string[] Members { get; }
}

#else

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

[assembly: TypeForwardedTo(typeof(MemberNotNullWhenAttribute))]

#endif
