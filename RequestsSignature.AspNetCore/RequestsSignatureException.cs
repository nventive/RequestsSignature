using System;

namespace RequestsSignature.AspNetCore
{
    /// <summary>
    /// Exceptions thrown during Requests Signature processing.
    /// </summary>
    [Serializable]
    public class RequestsSignatureException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestsSignatureException"/> class.
        /// </summary>
        public RequestsSignatureException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestsSignatureException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public RequestsSignatureException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestsSignatureException"/> class with a specified error
        /// message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="inner">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public RequestsSignatureException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestsSignatureException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        protected RequestsSignatureException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }
}
