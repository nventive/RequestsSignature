namespace AspNetCoreRequestsSignature
{
    /// <summary>
    /// Results of a signature validation process.
    /// (<see cref="IRequestsSignatureValidationService.Validate(Microsoft.AspNetCore.Http.HttpRequest)"/>.
    /// </summary>
    public class SignatureValidationResult
    {
        /// <summary>
        /// Gets a value indicating whether the signature is valid.
        /// </summary>
        public bool IsValid { get; }

        /// <summary>
        /// Gets the client id that matched the signature, if any.
        /// </summary>
        public string ClientId { get; }
    }
}
