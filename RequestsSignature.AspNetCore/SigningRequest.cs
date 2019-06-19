using Microsoft.AspNetCore.Http;

namespace RequestsSignature.AspNetCore
{
    /// <summary>
    /// Holds parameters for signing a request.
    /// </summary>
    public class SigningRequest
    {
        /// <summary>
        /// Gets or sets the <see cref="HttpRequest"/>.
        /// </summary>
        public HttpRequest Request { get; set; }

        /// <summary>
        /// Gets or sets the nonce.
        /// </summary>
        public string Nonce { get; set; }

        /// <summary>
        /// Gets or sets the timestamp.
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="RequestsSignatureClientOptions"/>.
        /// </summary>
        public RequestsSignatureClientOptions Options { get; set; }
    }
}
