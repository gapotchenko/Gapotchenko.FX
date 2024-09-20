// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2021

namespace Gapotchenko.FX.Math.Graphs;

/// <summary>
/// Thrown when a graph has an unresolved circular reference (also known as cycle).
/// </summary>
public class GraphCircularReferenceException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GraphCircularReferenceException"/> class.
    /// </summary>
    public GraphCircularReferenceException() :
        this("The graph contains an unresolved circular reference.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GraphCircularReferenceException"/> class
    /// with a specified error message.
    /// </summary>
    /// <param name="message"><inheritdoc/></param>
    public GraphCircularReferenceException(string? message) :
        base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GraphCircularReferenceException"/> class
    /// with a specified error message and
    /// a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message"><inheritdoc/></param>
    /// <param name="innerException"><inheritdoc/></param>
    public GraphCircularReferenceException(string? message, Exception? innerException) :
        base(message, innerException)
    {
    }
}
