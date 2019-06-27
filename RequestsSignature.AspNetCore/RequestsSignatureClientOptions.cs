using System.Collections.Generic;

namespace RequestsSignature.AspNetCore
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
        /// Gets or sets the signature key.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets the ordered list of signature body source components used to compute
        /// the value that will be signed and create the signature body.
        /// </summary>
        public IList<string> SignatureBodySourceComponents { get; } = new List<string>();
    }
}
