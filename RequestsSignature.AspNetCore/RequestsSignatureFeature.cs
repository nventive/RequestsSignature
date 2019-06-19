namespace RequestsSignature.AspNetCore
{
    /// <summary>
    /// <see cref="IRequestsSignatureFeature"/> default implementation.
    /// </summary>
    internal class RequestsSignatureFeature : IRequestsSignatureFeature
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestsSignatureFeature"/> class.
        /// </summary>
        /// <param name="validationResult">The <see cref="SignatureValidationResult"/>.</param>
        public RequestsSignatureFeature(SignatureValidationResult validationResult)
        {
            ValidationResult = validationResult ?? throw new System.ArgumentNullException(nameof(validationResult));
        }

        /// <inheritdoc />
        public SignatureValidationResult ValidationResult { get; }
    }
}
