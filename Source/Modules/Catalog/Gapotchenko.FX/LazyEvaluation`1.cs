using Gapotchenko.FX.Properties;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Gapotchenko.FX;

/// <summary>
/// Provides an evaluation strategy which delays the evaluation of an expression until its value is needed.
/// </summary>
/// <typeparam name="T">Specifies the type of object that is being lazily evaluated.</typeparam>
/// <remarks>
/// <para>
/// <see cref="LazyEvaluation{T}"/> is a better alternative to <see cref="Lazy{T}"/> when the evaluated value is located at a local variable.
/// </para>
/// <para>
/// <see cref="LazyEvaluation{T}"/> is not thread-safe.
/// For thread-safe lazy evaluation, please use <see cref="Threading.EvaluateOnce{T}"/> struct.
/// </para>
/// </remarks>
[Serializable]
[DebuggerDisplay("IsValueCreated={IsValueCreated}, Value={ValueForDebugDisplay}")]
[DebuggerTypeProxy(typeof(LazyEvaluation<>.DebugView))]
public struct LazyEvaluation<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LazyEvaluation{T}"/> struct.
    /// </summary>
    /// <param name="valueFactory">The value factory that is invoked to produce a lazily evaluated value when it is needed.</param>
    public LazyEvaluation(Func<T> valueFactory)
    {
        if (valueFactory == null)
            throw new ArgumentNullException(nameof(valueFactory));

        m_ValueFactory = Empty.Nullify(valueFactory);
        m_Value = default;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [AllowNull]
    T m_Value;

    [NonSerialized]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    Func<T>? m_ValueFactory;

    /// <summary>
    /// Gets the lazily evaluated value of the current <see cref="LazyEvaluation{T}"/> instance.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public T Value
    {
        get
        {
            var defaultFunc = Fn<T>.Default;
            if (m_ValueFactory != defaultFunc)
            {
                var valueFactory = m_ValueFactory;
                if (valueFactory != null)
                    m_Value = valueFactory();
                m_ValueFactory = defaultFunc!;
            }
            return m_Value;
        }
    }

    /// <summary>
    /// Gets a value that indicates whether a value has been created for this <see cref="LazyEvaluation{T}"/> instance.
    /// </summary>
    public bool IsValueCreated => m_ValueFactory == Fn<T>.Default;

    /// <summary>
    /// Creates and returns a string representation of the <see cref="Value"/> property for this instance.
    /// </summary>
    /// <returns>
    /// The result of calling the <see cref="Object.ToString()"/> method on the <see cref="Value"/> property for this instance,
    /// if the value has been created (that is, if the <see cref="IsValueCreated"/> property returns <see langword="true"/>).
    /// Otherwise, a string indicating that the value has not been created.
    /// </returns>
    public override string? ToString() =>
        IsValueCreated ?
            Value?.ToString() :
            Resources.ValueNotCreated;

    [OnSerializing]
    void OnSerializing(StreamingContext context)
    {
        // Force evaluation before the value is serialized.
        Fn.Ignore(Value);
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [MaybeNull]
    T ValueForDebugDisplay => IsValueCreated ? Value : default;

    internal sealed class DebugView(LazyEvaluation<T> instance)
    {
        public bool IsValueCreated => instance.IsValueCreated;

        [MaybeNull]
        public T Value => instance.ValueForDebugDisplay;
    }
}
