namespace AspNetCoreRequestsSignature
{
    /// <summary>
    /// Signature options for a specific client.
    /// </summary>
    public class RequestsSignatureClientOptions
    {
        /// <summary>
        /// Gets the default <see cref="SignatureBodyPattern"/>.
        /// ({Method}.{Scheme}://{Host}{Path}{QueryString}).
        /// </summary>
        public const string DefaultSignatureBodyPattern = @"{Method} {Scheme}://{Host}{Path}{QueryString}";

        /// <summary>
        /// Gets or sets the client id.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the signature key.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the pattern used to constitute the value that needs to be signed.
        /// </summary>
        public string SignatureBodyPattern { get; set; } = DefaultSignatureBodyPattern;
    }
}
