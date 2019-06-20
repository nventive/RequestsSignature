using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace RequestsSignature.Core
{
    /// <summary>
    /// Holds parameters for signing a request.
    /// </summary>
    public class SigningBodyRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SigningBodyRequest"/> class.
        /// </summary>
        /// <param name="method">The request method.</param>
        /// <param name="uri">The request <see cref="Uri"/>.</param>
        /// <param name="headers">The request headers.</param>
        /// <param name="nonce">The nonce.</param>
        /// <param name="timestamp">The timestamp.</param>
        /// <param name="clientId">The client id.</param>
        /// <param name="key">The key.</param>
        /// <param name="signatureBodySourceComponents">The signature body components.</param>
        /// <param name="body">The request body, if any.</param>
        public SigningBodyRequest(
            string method,
            Uri uri,
            IDictionary<string, string> headers,
            string nonce,
            long timestamp,
            string clientId,
            string key,
            IList<string> signatureBodySourceComponents,
            byte[] body = null)
        {
            Method = method;
            Uri = uri;
            Headers = headers;
            Nonce = nonce;
            Timestamp = timestamp;
            ClientId = clientId;
            Key = key;
            SignatureBodySourceComponents = signatureBodySourceComponents;
            Body = body;
        }

        /// <summary>
        /// Gets the Request Method.
        /// </summary>
        public string Method { get; }

        /// <summary>
        /// Gets the <see cref="Uri"/>.
        /// </summary>
        public Uri Uri { get; }

        /// <summary>
        /// Gets the Request Method.
        /// </summary>
        public IDictionary<string, string> Headers { get; }

        /// <summary>
        /// Gets the Request body.
        /// </summary>
        [SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "OK in that case - binary data.")]
        public byte[] Body { get; }

        /// <summary>
        /// Gets the nonce.
        /// </summary>
        public string Nonce { get; }

        /// <summary>
        /// Gets the timestamp.
        /// </summary>
        public long Timestamp { get; }

        /// <summary>
        /// Gets the client id.
        /// </summary>
        public string ClientId { get; }

        /// <summary>
        /// Gets the signature key.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Gets the ordered list of singature body source components used to compute
        /// the value that will be signed and create the signature body.
        /// </summary>
        public IList<string> SignatureBodySourceComponents { get; }

        /// <inheritdoc />
        public override string ToString() => $"{Method} {Uri} {Nonce} {Timestamp}";
    }
}
