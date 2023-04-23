﻿namespace Gapotchenko.FX;

/// <summary>
/// Represents an object that has the functional notion of emptiness and can be empty.
/// </summary>
/// <remarks>
/// Types implementing this interface get automatic support of operations
/// provided by <see cref="Empty"/> type.
/// </remarks>
public interface IEmptiable
{
    /// <summary>
    /// Gets a value indicating whether the current object is empty.
    /// </summary>
    bool IsEmpty { get; }
}