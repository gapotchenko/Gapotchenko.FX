using Gapotchenko.FX.Properties;
using System;
using System.Runtime.Serialization;

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
            base(string.Format(Resources.ProgramExit_Message, 0))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgramExitException"/> class with a specified exit code.
        /// </summary>
        /// <param name="exitCode">The exit code.</param>
        public ProgramExitException(int exitCode) :
            base(string.Format(Resources.ProgramExit_Message, exitCode))
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
            ExitCode = info.GetInt32(nameof(ExitCode));
        }

        /// <summary>
        /// Sets the <see cref="SerializationInfo"/> with the parameter name and additional exception information.
        /// </summary>
        /// <param name="info">Serialization information.</param>
        /// <param name="context">Streaming context.</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(nameof(ExitCode), ExitCode);
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
