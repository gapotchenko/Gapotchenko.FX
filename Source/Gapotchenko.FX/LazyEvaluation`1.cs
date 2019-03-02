using Gapotchenko.FX.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Permissions;
using System.Text;

namespace Gapotchenko.FX
{
    /// <summary>
    /// Provides an evaluation strategy which delays the evaluation of an expression until its value is needed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="LazyEvaluation{T}"/> struct does not provide thread safety guarantees except memory model consistency.
    /// In this way, it brings considerable performance and memory benefits.
    /// </para>
    /// <para>
    /// For example, <see cref="LazyEvaluation{T}"/> is a much better alternative to <see cref="Lazy{T}"/> when the evaluated value is located at a local variable.
    /// </para>
    /// </remarks>
    /// <typeparam name="T">Specifies the type of object that is being lazily evaluated.</typeparam>
    [DebuggerDisplay("IsValueCreated={IsValueCreated}, Value={ValueForDebugDisplay}")]
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

            _ValueFactory = Empty.Nullify(valueFactory);
            _Value = default;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        T _Value;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        Func<T> _ValueFactory;

        /// <summary>
        /// Gets the lazily evaluated value of the current <see cref="LazyEvaluation{T}"/> instance.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public T Value
        {
            get
            {
                var defaultFunc = Empty<T>.DefaultFunc;
                var valueFactory = _ValueFactory;
                if (valueFactory != defaultFunc)
                {
                    if (valueFactory != null)
                        _Value = valueFactory();
                    _ValueFactory = defaultFunc;
                }
                return _Value;
            }
        }

        /// <summary>
        /// Gets a value that indicates whether a value has been created for this <see cref="LazyEvaluation{T}"/> instance.
        /// </summary>
        public bool IsValueCreated => _ValueFactory == Empty<T>.DefaultFunc;

        /// <summary>
        /// Creates and returns a string representation of the <see cref="Value"/> property for this instance.
        /// </summary>
        /// <returns>
        /// The result of calling the <see cref="Object.ToString()"/> method on the <see cref="Value"/> property for this instance,
        /// if the value has been created (that is, if the <see cref="IsValueCreated"/> property returns <c>true</c>).
        /// Otherwise, a string indicating that the value has not been created.
        /// </returns>
        public override string ToString() => IsValueCreated ? Value.ToString() : Resources.ValueNotCreated;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal T ValueForDebugDisplay => IsValueCreated ? Value : default;
    }
}
