﻿// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2020

#if !TFF_NULLABLE_ATTRIBUTES

#pragma warning disable IDE0130 // Namespace does not match folder structure

namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// Specifies that an output will not be <see langword="null"/> even if the corresponding type allows it.
/// Specifies that an input argument was not <see langword="null"/> when the call returns.
/// </summary>
/// <remarks>
/// This is a polyfill provided by Gapotchenko.FX.
/// </remarks>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, Inherited = false)]
public sealed class NotNullAttribute : Attribute
{
}

#else

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

[assembly: TypeForwardedTo(typeof(NotNullAttribute))]

#endif