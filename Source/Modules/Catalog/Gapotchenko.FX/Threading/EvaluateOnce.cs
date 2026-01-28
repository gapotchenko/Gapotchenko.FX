// Gapotchenko.FX
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2019

namespace Gapotchenko.FX.Threading;

/// <summary>
/// Provides static methods for creating <see cref="EvaluateOnce{T}"/> objects.
/// </summary>
public static class EvaluateOnce
{
    /// <summary>
    /// Creates a new instance of the <see cref="EvaluateOnce{T}"/> structure.
    /// When lazy evaluation occurs, the default constructor of the target type <typeparamref name="T"/> is used.
    /// </summary>
    /// <typeparam name="T">Specifies the type of object that is being lazily evaluated.</typeparam>
    /// <returns>A new <see cref="EvaluateOnce{T}"/> instance.</returns>
    public static EvaluateOnce<T> Create<T>() where T : new() => new(() => new T());

    /// <summary>
    /// Creates a new instance of the <see cref="EvaluateOnce{T}"/> structure.
    /// </summary>
    /// <typeparam name="T">Specifies the type of object that is being lazily evaluated.</typeparam>
    /// <param name="valueFactory">The value factory that is invoked to produce a lazily evaluated value when it is needed.</param>
    /// <returns>A new <see cref="EvaluateOnce{T}"/> instance.</returns>
    public static EvaluateOnce<T> Create<T>(Func<T> valueFactory) => new(valueFactory);

    /// <summary>
    /// Creates a new instance of the <see cref="EvaluateOnce{T}"/> structure.
    /// </summary>
    /// <typeparam name="T">Specifies the type of object that is being lazily evaluated.</typeparam>
    /// <param name="valueFactory">The value factory that is invoked to produce a lazily evaluated value when it is needed.</param>
    /// <param name="syncLock">
    /// An object used as the mutually exclusive lock for value evaluation.
    /// When the given value is <see langword="null"/>, an unique synchronization lock object is used.
    /// </param>
    /// <returns>A new <see cref="EvaluateOnce{T}"/> instance.</returns>
    public static EvaluateOnce<T> Create<T>(Func<T> valueFactory, object? syncLock) => new(valueFactory, syncLock);

    /// <summary>
    /// Creates a new instance of the <see cref="EvaluateOnce{T}"/> structure.
    /// </summary>
    /// <typeparam name="T">Specifies the type of object that is being lazily evaluated.</typeparam>
    /// <param name="valueFactory">The value factory that is invoked to produce a lazily evaluated value when it is needed.</param>
    /// <param name="syncLock">
    /// A <see cref="Lock"/> object used as the mutually exclusive lock for value evaluation.
    /// When the given value is <see langword="null"/>, an unique synchronization lock object is used.
    /// </param>
    /// <returns>A new <see cref="EvaluateOnce{T}"/> instance.</returns>
    public static EvaluateOnce<T> Create<T>(Func<T> valueFactory, Lock? syncLock) => new(valueFactory, syncLock);

    /// <summary>
    /// Creates and initializes a new instance of the <see cref="EvaluateOnce{T}"/> structure with the specified value of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">Specifies the type of object that is being lazily evaluated.</typeparam>
    /// <param name="value">The preinitialized value to be used.</param>
    /// <returns>
    /// A new <see cref="EvaluateOnce{T}"/> instance
    /// with <see cref="EvaluateOnce{T}.IsValueCreated"/> property set to <see langword="true"/>
    /// and with <see cref="EvaluateOnce{T}.Value"/> property set to <paramref name="value"/>.
    /// </returns>
    public static EvaluateOnce<T> Create<T>(T value) => new(value);

#if SOURCE_COMPATIBILITY || BINARY_COMPATIBILITY // 2026

    /// <inheritdoc cref="Create{T}()"/>
    [Obsolete("Use Create<T>(T) method instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static EvaluateOnce<T> Value<T>(T value) => Create(value);

#endif
}
