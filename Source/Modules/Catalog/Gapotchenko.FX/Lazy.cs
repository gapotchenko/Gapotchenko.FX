namespace Gapotchenko.FX;

/// <summary>
/// Provides static methods for creating <see cref="Lazy{T}"/> objects.
/// </summary>
public static class Lazy
{
    /// <summary>
    /// Creates a new instance of the <see cref="Lazy{T}"/> class.
    /// When lazy initialization occurs, the default constructor of the target type <typeparamref name="T"/> is used.
    /// </summary>
    /// <typeparam name="T">Specifies the type of object that is being lazily initialized.</typeparam>
    /// <returns>New <see cref="Lazy{T}"/> instance.</returns>
    public static Lazy<T> Create
#if NET5_0_OR_GREATER
        <[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T>
#else
        <T>
#endif
        () where T : new() => new();

    /// <summary>
    /// Creates a new instance of the <see cref="Lazy{T}"/> class.
    /// When lazy initialization occurs, the default constructor of the target type <typeparamref name="T"/> is used.
    /// </summary>
    /// <typeparam name="T">Specifies the type of object that is being lazily initialized.</typeparam>
    /// <param name="isThreadSafe">
    /// <see langword="true"/> to make this instance usable concurrently by multiple threads;
    /// <see langword="false"/> to make the instance usable by only one thread at a time.
    /// </param>
    /// <returns>New <see cref="Lazy{T}"/> instance.</returns>
    public static Lazy<T> Create
#if NET5_0_OR_GREATER
        <[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T>
#else
        <T>
#endif
        (bool isThreadSafe) where T : new() => new(isThreadSafe);

    /// <summary>
    /// Creates a new instance of the <see cref="Lazy{T}"/> class.
    /// When lazy initialization occurs, the default constructor of the target type <typeparamref name="T"/> is used.
    /// </summary>
    /// <typeparam name="T">Specifies the type of object that is being lazily initialized.</typeparam>
    /// <param name="mode">One of the enumeration values that specifies the thread safety mode.</param>
    /// <returns>New <see cref="Lazy{T}"/> instance.</returns>
    public static Lazy<T> Create
#if NET5_0_OR_GREATER
        <[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T>
#else
        <T>
#endif
        (LazyThreadSafetyMode mode) where T : new() => new(mode);

    /// <summary>
    /// Creates a new instance of the <see cref="Lazy{T}"/> class.
    /// When lazy initialization occurs, the specified initialization function is used.
    /// </summary>
    /// <typeparam name="T">Specifies the type of object that is being lazily initialized.</typeparam>
    /// <param name="valueFactory">The delegate that is invoked to produce the lazily initialized value when it is needed.</param>
    /// <returns>New <see cref="Lazy{T}"/> instance.</returns>
    public static Lazy<T> Create
#if NET5_0_OR_GREATER
        <[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T>
#else
        <T>
#endif
        (Func<T> valueFactory) => new(valueFactory);

    /// <summary>
    /// Creates a new instance of the <see cref="Lazy{T}"/> class.
    /// When lazy initialization occurs, the specified initialization function is used.
    /// </summary>
    /// <typeparam name="T">Specifies the type of object that is being lazily initialized.</typeparam>
    /// <param name="valueFactory">The delegate that is invoked to produce the lazily initialized value when it is needed.</param>
    /// <param name="isThreadSafe">
    /// <see langword="true"/> to make this instance usable concurrently by multiple threads;
    /// <see langword="false"/> to make the instance usable by only one thread at a time.
    /// </param>
    /// <returns>New <see cref="Lazy{T}"/> instance.</returns>
    public static Lazy<T> Create
#if NET5_0_OR_GREATER
        <[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T>
#else
        <T>
#endif
        (Func<T> valueFactory, bool isThreadSafe) => new(valueFactory, isThreadSafe);

    /// <summary>
    /// Creates a new instance of the <see cref="Lazy{T}"/> class.
    /// When lazy initialization occurs, the specified initialization function is used.
    /// </summary>
    /// <typeparam name="T">Specifies the type of object that is being lazily initialized.</typeparam>
    /// <param name="valueFactory">The delegate that is invoked to produce the lazily initialized value when it is needed.</param>
    /// <param name="mode">One of the enumeration values that specifies the thread safety mode.</param>
    /// <returns>New <see cref="Lazy{T}"/> instance.</returns>
    public static Lazy<T> Create
#if NET5_0_OR_GREATER
        <[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T>
#else
        <T>
#endif
        (Func<T> valueFactory, LazyThreadSafetyMode mode) => new(valueFactory, mode);
}
