// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

#if !NET6_0_OR_GREATER

#pragma warning disable IDE0130 // Namespace does not match folder structure

namespace System.Diagnostics;

/// <summary>
/// Types and methods attributed with <see cref="StackTraceHiddenAttribute"/> will be omitted from a stack trace dump
/// produced by <see cref="StackTrace.ToString"/> method and <see cref="Exception.StackTrace"/> property.
/// </summary>
/// <remarks>
/// This is a polyfill provided by Gapotchenko.FX.
/// </remarks>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Struct, Inherited = false)]
public sealed class StackTraceHiddenAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StackTraceHiddenAttribute"/> class.
    /// </summary>
    public StackTraceHiddenAttribute()
    {
    }
}

#else

using System.Diagnostics;
using System.Runtime.CompilerServices;

[assembly: TypeForwardedTo(typeof(StackTraceHiddenAttribute))]

#endif
