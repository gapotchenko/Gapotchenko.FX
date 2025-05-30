﻿// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// Portions © .NET Foundation and its Licensors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2023

#if !NET7_0_OR_GREATER

#pragma warning disable IDE0130 // Namespace does not match folder structure

namespace System.Diagnostics;

/// <summary>
/// Exception thrown when the program executes an instruction that was thought to be unreachable.
/// </summary>
/// <remarks>
/// This is a polyfill provided by Gapotchenko.FX.
/// </remarks>
public sealed class UnreachableException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnreachableException"/> class with the default error message.
    /// </summary>
    public UnreachableException() :
        base("The program executed an instruction that was thought to be unreachable.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnreachableException"/>
    /// class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public UnreachableException(string? message) :
        base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnreachableException"/>
    /// class with a specified error message and a reference to the inner exception that is the cause of
    /// this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public UnreachableException(string? message, Exception? innerException) :
        base(message, innerException)
    {
    }
}

#else

using System.Diagnostics;
using System.Runtime.CompilerServices;

[assembly: TypeForwardedTo(typeof(UnreachableException))]

#endif
