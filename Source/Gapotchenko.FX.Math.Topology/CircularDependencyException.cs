// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2021

using System.Runtime.Serialization;

namespace Gapotchenko.FX.Math.Topology;

/// <summary>
/// Thrown when a topology has an unresolved circular reference.
/// </summary>
[Serializable]
public class CircularDependencyException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CircularDependencyException"/> class.
    /// </summary>
    public CircularDependencyException() :
        this("The topology contains a circular reference.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CircularDependencyException"/> class
    /// with a specified error message.
    /// </summary>
    /// <param name="message"><inheritdoc/></param>
    public CircularDependencyException(string? message) :
        base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CircularDependencyException"/> class
    /// with a specified error message and
    /// a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message"><inheritdoc/></param>
    /// <param name="innerException"><inheritdoc/></param>
    public CircularDependencyException(string? message, Exception? innerException) :
        base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CircularDependencyException"/> class
    /// with serialized data.
    /// </summary>
    /// <param name="info"><inheritdoc/></param>
    /// <param name="context"><inheritdoc/></param>
#if NET8_0_OR_GREATER
    [Obsolete(DiagnosticId = "SYSLIB0051")]
#endif
    protected CircularDependencyException(SerializationInfo info, StreamingContext context) :
        base(info, context)
    {
    }
}
