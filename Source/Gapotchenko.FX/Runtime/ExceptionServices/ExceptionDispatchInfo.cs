using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text;

#if NET40

namespace System.Runtime.ExceptionServices
{
    /// <summary>
    /// <para>
    /// Represents an exception whose state is captured at a certain point in code.
    /// </para>
    /// <para>
    /// This is a polyfill provided by Gapotchenko.FX.
    /// </para>
    /// </summary>
    public sealed class ExceptionDispatchInfo
    {
        ExceptionDispatchInfo()
        {
        }

        /// <summary>
        /// Gets the exception that is represented by the current instance.
        /// </summary>
        public Exception SourceException { get; private set; }

        /// <summary>
        /// Creates an <see cref="ExceptionDispatchInfo"/> object that represents the specified exception at the current point in code.
        /// </summary>
        /// <param name="source">The exception whose state is captured, and which is represented by the returned object.</param>
        /// <returns>An object that represents the specified exception at the current point in code.</returns>
        public static ExceptionDispatchInfo Capture(Exception source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            // TODO: Needs a more precise implementation

            var edi = new ExceptionDispatchInfo();
            edi.SourceException = source;
            return edi;
        }

        /// <summary>
        /// Throws the exception that is represented by the current <see cref="ExceptionDispatchInfo"/> object,
        /// after restoring the state that was saved when the exception was captured.
        /// </summary>
        public void Throw()
        {
            // TODO: Needs a more precise implementation

            throw SourceException;
        }
    }
}

#else

[assembly: TypeForwardedTo(typeof(ExceptionDispatchInfo))]

#endif
