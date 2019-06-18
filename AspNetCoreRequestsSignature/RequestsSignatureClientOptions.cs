namespace AspNetCoreRequestsSignature
{
    /// <summary>
    /// Signature options for a specific client.
    /// </summary>
    public class RequestsSignatureClientOptions
    {
        /// <summary>
        /// Gets or sets the client id.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the Base-64 encoded signature key.
        /// </summary>
        public string Base64Key { get; set; }
    }
}
