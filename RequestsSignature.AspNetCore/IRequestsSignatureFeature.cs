namespace RequestsSignature.AspNetCore
{
    /// <summary>
    /// Requests feature for the result of the signature validation requests.
    /// </summary>
    public interface IRequestsSignatureFeature
    {
        /// <summary>
        /// Gets the <see cref="SignatureValidationResult"/>.
        /// </summary>
        SignatureValidationResult ValidationResult { get; }
    }
}
