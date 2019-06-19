using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace RequestsSignature.Core
{
    /// <summary>
    /// Holds parameters for signing a request.
    /// </summary>
    [SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Internal usage.")]
    public class SigningBodyRequest
    {
        /// <summary>
        /// Gets or sets the Request Method.
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// Gets or sets the Uri Scheme.
        /// </summary>
        public string Scheme { get; set; }

        /// <summary>
        /// Gets or sets the Uri Scheme.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Gets or sets the Uri Scheme.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the Uri QueryString.
        /// </summary>
        public string QueryString { get; set; }

        /// <summary>
        /// Gets or sets the Request Method.
        /// </summary>
        public IReadOnlyDictionary<string, string> Headers { get; set; }

        /// <summary>
        /// Gets or sets the Request body.
        /// </summary>
        [SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "OK in that case.")]
        public byte[] Body { get; set; }

        /// <summary>
        /// Gets or sets the nonce.
        /// </summary>
        public string Nonce { get; set; }

        /// <summary>
        /// Gets or sets the timestamp.
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the client id.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the signature key.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the ordered list of singature body source components used to compute
        /// the value that will be signed and create the signature body.
        /// </summary>
        public IList<string> SignatureBodySourceComponents { get; set; }

        /// <inheritdoc />
        public override string ToString() => $"{Method} {Scheme}://{Host}/{Path}{QueryString} {Nonce} {Timestamp}";
    }
}
