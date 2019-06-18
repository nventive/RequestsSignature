namespace AspNetCoreRequestsSignature
{
    /// <summary>
    /// Results of a signature validation process.
    /// (<see cref="IRequestsSignatureValidationService.Validate(Microsoft.AspNetCore.Http.HttpRequest)"/>.
    /// </summary>
    public class SignatureValidationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SignatureValidationResult"/> class.
        /// </summary>
        /// <param name="status">The <see cref="SignatureValidationResultStatus"/>.</param>
        /// <param name="serverTimestamp">The current server timestamp, expressed as Unix Epoch in seconds.</param>
        /// <param name="clientId">The matched client id, if any.</param>
        /// <param name="signatureValue">The complete signature value, if any.</param>
        /// <param name="computedSignature">The computed signature (server-side), if any.</param>
        public SignatureValidationResult(
            SignatureValidationResultStatus status,
            long serverTimestamp,
            string clientId = null,
            string signatureValue = null,
            string computedSignature = null)
        {
            Status = status;
            ServerTimestamp = serverTimestamp;
            ClientId = clientId;
            SignatureValue = signatureValue;
            ComputedSignature = computedSignature;
        }

        /// <summary>
        /// Gets the <see cref="SignatureValidationResultStatus"/>.
        /// </summary>
        public SignatureValidationResultStatus Status { get; }

        /// <summary>
        /// Gets the current timestamp of the server, expressed as Unix Epoch in seconds.
        /// </summary>
        public long ServerTimestamp { get; }

        /// <summary>
        /// Gets the client id that matched the signature, if any.
        /// </summary>
        public string ClientId { get; }

        /// <summary>
        /// Gets the complete signature value, if any.
        /// This is the value parsed by <see cref="RequestsSignatureOptions.SignaturePattern"/>.
        /// </summary>
        public string SignatureValue { get; }

        /// <summary>
        /// Gets the computed signature (server-side), if any.
        /// </summary>
        public string ComputedSignature { get; }
    }
}
