using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Gapotchenko.FX
{
    /// <summary>
    /// Represents a program exit exception.
    /// Throwing an object of this class should return control to the global main function, which should exit with the given exit code.
    /// </summary>
    [Serializable]
    public class ProgramExitException : Exception, IControlFlowException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProgramExitException"/> class.
        /// </summary>
        public ProgramExitException() :
            base("The program exit has been requested.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgramExitException"/> class with a specified exit code.
        /// </summary>
        /// <param name="exitCode">The exit code.</param>
        public ProgramExitException(int exitCode) :
            this()
        {
            ExitCode = exitCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgramExitException"/> class with serialized data.
        /// </summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
        /// </param>
        protected ProgramExitException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
        }

        /// <summary>
        /// Gets the exit code.
        /// </summary>
        public int ExitCode
        {
            get;
        }
    }
}
