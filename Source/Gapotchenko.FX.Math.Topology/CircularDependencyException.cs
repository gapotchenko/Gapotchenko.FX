using System;
using System.Runtime.Serialization;

namespace Gapotchenko.FX.Math.Topology
{
    /// <summary>
    /// Thrown when a topology (graph) has an unresolved circular reference.
    /// </summary>
    [Serializable]
    public class CircularDependencyException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CircularDependencyException"/> class.
        /// </summary>
        public CircularDependencyException() :
            this("The graph contains a circular reference.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CircularDependencyException"/> class
        /// with a specified error message.
        /// </summary>
        /// <param name="message"><inheritdoc/></param>
        public CircularDependencyException(string? message) :
            base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CircularDependencyException"/> class
        /// with a specified error message and
        /// a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message"><inheritdoc/></param>
        /// <param name="innerException"><inheritdoc/></param>
        public CircularDependencyException(string? message, Exception? innerException) :
            base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CircularDependencyException"/> class
        /// with serialized data.
        /// </summary>
        /// <param name="info"><inheritdoc/></param>
        /// <param name="context"><inheritdoc/></param>
        protected CircularDependencyException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
        }
    }
}
