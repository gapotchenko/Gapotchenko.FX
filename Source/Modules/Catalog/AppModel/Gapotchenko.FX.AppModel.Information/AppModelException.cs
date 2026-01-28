// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2025

namespace Gapotchenko.FX.AppModel;

/// <summary>
/// Represents an error that can occur in Gapotchenko.FX.AppModel module.
/// </summary>
public class AppModelException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppModelException"/> class.
    /// </summary>
    public AppModelException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AppModelException"/> class with a specified error message.
    /// </summary>
    /// <inheritdoc/>
    public AppModelException(string? message) :
        base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AppModelException"/> class with a specified error message
    /// and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <inheritdoc/>
    public AppModelException(string? message, Exception? innerException) :
        base(message, innerException)
    {
    }
}

// NOTE: this class conceptually belongs to Gapotchenko.FX.AppModel module
// which does not exist yet. When it appears, AppModelException type should
// be forwarded there for binary compatibility.
