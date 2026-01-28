// Gapotchenko.FX
//
// Copyright © Gapotchenko and Contributors
//
// File introduced by: Oleksiy Gapotchenko
// Year of introduction: 2019

using Gapotchenko.FX.Properties;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Gapotchenko.FX.Threading;

/// <summary>
/// Provides a thread-safe evaluation strategy which delays the evaluation of an expression until its value is needed.
/// </summary>
/// <typeparam name="T">Specifies the type of object that is being lazily evaluated.</typeparam>
/// <remarks>
/// <para>
/// <see cref="EvaluateOnce{T}"/> is a struct and thus sometimes it may be a better choice than <see cref="Lazy{T}"/> in terms of performance and memory allocation.
/// </para>
/// <para>
/// <see cref="EvaluateOnce{T}"/> is thread-safe.
/// </para>
/// </remarks>
[Serializable]
#if TFF_HOST_PROTECTION
[HostProtection(Synchronization = true, ExternalThreading = true)]
#endif
[DebuggerDisplay("IsValueCreated={IsValueCreated}, Value={ValueForDebugDisplay}")]
[DebuggerTypeProxy(typeof(EvaluateOnce<>.DebugView))]
public struct EvaluateOnce<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EvaluateOnce{T}"/> structure.
    /// </summary>
    /// <param name="valueFactory">The value factory that is invoked to produce a lazily evaluated value when it is needed.</param>
    public EvaluateOnce(Func<T> valueFactory) :
        this(valueFactory, (object?)null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EvaluateOnce{T}"/> structure.
    /// </summary>
    /// <param name="valueFactory">The value factory that is invoked to produce a lazily evaluated value when it is needed.</param>
    /// <param name="syncLock">
    /// An object used as the mutually exclusive lock for value evaluation.
    /// When the given value is <see langword="null"/>, an unique synchronization lock object is used.
    /// </param>
    public EvaluateOnce(Func<T> valueFactory, object? syncLock)
    {
        m_ValueFactory = valueFactory ?? throw new ArgumentNullException(nameof(valueFactory));
        m_SyncLock = syncLock;
        m_Value = default;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EvaluateOnce{T}"/> structure.
    /// </summary>
    /// <param name="valueFactory">The value factory that is invoked to produce a lazily evaluated value when it is needed.</param>
    /// <param name="syncLock">
    /// A <see cref="Lock"/> object used as the mutually exclusive lock for value evaluation.
    /// When the given value is <see langword="null"/>, an unique synchronization lock object is used.
    /// </param>
    public EvaluateOnce(Func<T> valueFactory, Lock? syncLock) :
#pragma warning disable CS9216 // A value of type 'System.Threading.Lock' converted to a different type will use likely unintended monitor-based locking in 'lock' statement.
        this(valueFactory, (object?)syncLock)
#pragma warning restore CS9216
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EvaluateOnce{T}"/> structure with the specified value of type <typeparamref name="T"/>.
    /// </summary>
    /// <remarks>
    /// After using this constructor,
    /// <see cref="IsValueCreated"/> property is set to <see langword="true"/> and
    /// <see cref="Value"/> property is set to <paramref name="value"/>.
    /// </remarks>
    /// <param name="value">The preinitialized value to be used.</param>
    public EvaluateOnce(T value)
    {
        m_Value = value;
        m_SyncLock = DBNull.Value;
    }

    /// <summary>
    /// Gets the lazily evaluated value of the current <see cref="EvaluateOnce{T}"/> instance.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public T Value =>
        m_SyncLock is Lock ?
            LazyInitializerEx.EnsureInitialized(ref m_Value, ref Unsafe.As<object?, Lock?>(ref m_SyncLock), ref m_ValueFactory) :
            LazyInitializerEx.EnsureInitialized(ref m_Value, ref m_SyncLock, ref m_ValueFactory);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    T? m_Value;

    /// <summary>
    /// Gets a value that indicates whether a value has been created for this <see cref="EvaluateOnce{T}"/> instance.
    /// </summary>
    public readonly bool IsValueCreated
    {
        get
        {
            Thread.MemoryBarrier();
            return
                m_ValueFactory is null &&
                m_SyncLock is not null; // a check for m_SyncLock is needed to cover the uninitialized struct scenario
        }
    }

    [NonSerialized]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    object? m_SyncLock;

    [NonSerialized]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    Func<T>? m_ValueFactory;

    /// <summary>
    /// Creates and returns a string representation of the <see cref="Value"/> property for this instance.
    /// </summary>
    /// <returns>
    /// The result of calling the <see cref="Object.ToString()"/> method on the <see cref="Value"/> property for this instance,
    /// if the value has been created (that is, if the <see cref="IsValueCreated"/> property returns <see langword="true"/>).
    /// Otherwise, a string indicating that the value has not been created.
    /// </returns>
    public override string? ToString() => IsValueCreated ? Value?.ToString() : Resources.ValueNotCreated;

    [OnSerializing]
    void OnSerializing(StreamingContext context)
    {
        // Force evaluation before the value is serialized.
        _ = Value;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    T? ValueForDebugDisplay => IsValueCreated ? Value : default;

    internal sealed class DebugView(EvaluateOnce<T> instance)
    {
        public bool IsValueCreated => instance.IsValueCreated;

        public T? Value => instance.ValueForDebugDisplay;
    }
}
