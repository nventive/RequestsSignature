using System;
using System.Runtime.Serialization;

namespace RequestsSignature.AspNetCore
{
    /// <summary>
    /// Exceptions thrown during Requests Signature Validation processing.
    /// </summary>
    [Serializable]
    public class RequestsSignatureValidationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestsSignatureValidationException"/> class.
        /// </summary>
        public RequestsSignatureValidationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestsSignatureValidationException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public RequestsSignatureValidationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestsSignatureValidationException"/> class with a specified <see cref="SignatureValidationResult"/>.
        /// </summary>
        /// <param name="result">The <see cref="SignatureValidationResult"/>.</param>
        public RequestsSignatureValidationException(SignatureValidationResult result)
            : base(result?.Status.ToString())
        {
            Result = result;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestsSignatureValidationException"/> class with a specified error
        /// message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="inner">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public RequestsSignatureValidationException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestsSignatureValidationException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        protected RequestsSignatureValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Result = info.GetValue(nameof(Result), typeof(SignatureValidationResult)) as SignatureValidationResult;
        }

        /// <summary>
        /// Gets the <see cref="SignatureValidationResult"/>.
        /// </summary>
        public SignatureValidationResult Result { get; }

        /// <inheritdoc />
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue(nameof(Result), Result);
            base.GetObjectData(info, context);
        }
    }
}
