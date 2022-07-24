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
    public static Lazy<T> Create<T>() where T : new() => new Lazy<T>();

    /// <summary>
    /// Creates a new instance of the <see cref="Lazy{T}"/> class.
    /// When lazy initialization occurs, the default constructor of the target type <typeparamref name="T"/> is used.
    /// </summary>
    /// <typeparam name="T">Specifies the type of object that is being lazily initialized.</typeparam>
    /// <param name="isThreadSafe">
    /// <c>true</c> to make this instance usable concurrently by multiple threads;
    /// <c>false</c> to make the instance usable by only one thread at a time.
    /// </param>
    /// <returns>New <see cref="Lazy{T}"/> instance.</returns>
    public static Lazy<T> Create<T>(bool isThreadSafe) where T : new() => new Lazy<T>(isThreadSafe);

    /// <summary>
    /// Creates a new instance of the <see cref="Lazy{T}"/> class.
    /// When lazy initialization occurs, the default constructor of the target type <typeparamref name="T"/> is used.
    /// </summary>
    /// <typeparam name="T">Specifies the type of object that is being lazily initialized.</typeparam>
    /// <param name="mode">One of the enumeration values that specifies the thread safety mode.</param>
    /// <returns>New <see cref="Lazy{T}"/> instance.</returns>
    public static Lazy<T> Create<T>(LazyThreadSafetyMode mode) where T : new() => new Lazy<T>(mode);

    /// <summary>
    /// Creates a new instance of the <see cref="Lazy{T}"/> class.
    /// When lazy initialization occurs, the specified initialization function is used.
    /// </summary>
    /// <typeparam name="T">Specifies the type of object that is being lazily initialized.</typeparam>
    /// <param name="valueFactory">The delegate that is invoked to produce the lazily initialized value when it is needed.</param>
    /// <returns>New <see cref="Lazy{T}"/> instance.</returns>
    public static Lazy<T> Create<T>(Func<T> valueFactory) => new Lazy<T>(valueFactory);

    /// <summary>
    /// Creates a new instance of the <see cref="Lazy{T}"/> class.
    /// When lazy initialization occurs, the specified initialization function is used.
    /// </summary>
    /// <typeparam name="T">Specifies the type of object that is being lazily initialized.</typeparam>
    /// <param name="valueFactory">The delegate that is invoked to produce the lazily initialized value when it is needed.</param>
    /// <param name="isThreadSafe">
    /// <c>true</c> to make this instance usable concurrently by multiple threads;
    /// <c>false</c> to make the instance usable by only one thread at a time.
    /// </param>
    /// <returns>New <see cref="Lazy{T}"/> instance.</returns>
    public static Lazy<T> Create<T>(Func<T> valueFactory, bool isThreadSafe) => new Lazy<T>(valueFactory, isThreadSafe);

    /// <summary>
    /// Creates a new instance of the <see cref="Lazy{T}"/> class.
    /// When lazy initialization occurs, the specified initialization function is used.
    /// </summary>
    /// <typeparam name="T">Specifies the type of object that is being lazily initialized.</typeparam>
    /// <param name="valueFactory">The delegate that is invoked to produce the lazily initialized value when it is needed.</param>
    /// <param name="mode">One of the enumeration values that specifies the thread safety mode.</param>
    /// <returns>New <see cref="Lazy{T}"/> instance.</returns>
    public static Lazy<T> Create<T>(Func<T> valueFactory, LazyThreadSafetyMode mode) => new Lazy<T>(valueFactory, mode);
}
