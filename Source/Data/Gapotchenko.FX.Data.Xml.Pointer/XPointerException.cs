using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace Gapotchenko.FX.Data.Xml.Pointer
{
    /// <summary>
    /// Returns detailed information about the last exception related to XML pointer.
    /// </summary>
    [Serializable]
    public class XPointerException : XmlException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XPointerException"/> class.
        /// </summary>
        public XPointerException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XPointerException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public XPointerException(string message) :
            base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XPointerException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception.
        /// If the <paramref name="innerException" /> parameter is not a null reference (Nothing in Visual Basic), the current exception is raised in a catch block that handles the inner exception.
        /// </param>
        public XPointerException(string message, Exception innerException) :
            base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XPointerException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception.
        /// If the <paramref name="innerException" /> parameter is not a null reference (Nothing in Visual Basic), the current exception is raised in a catch block that handles the inner exception.
        /// </param>
        /// <param name="lineNumber">The line number.</param>
        /// <param name="linePosition">The line position.</param>
        public XPointerException(string message, Exception innerException, int lineNumber, int linePosition) :
            base(message, innerException, lineNumber, linePosition)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XPointerException"/> class.
        /// </summary>
        /// <param name="info">The object that holds the serialized object data.</param>
        /// <param name="context">The contextual information about the source or destination.</param>
        protected XPointerException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
        }
    }
}
