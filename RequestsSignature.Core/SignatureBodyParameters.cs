using System.Diagnostics.CodeAnalysis;

namespace RequestsSignature.Core
{
    /// <summary>
    /// Holds parameters for signing a request.
    /// </summary>
    public class SignatureBodyParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SignatureBodyParameters"/> class.
        /// </summary>
        /// <param name="data">The data to sign.</param>
        /// <param name="clientSecret">The client secret.</param>
        public SignatureBodyParameters(
            byte[] data,
            string clientSecret)
        {
            Data = data;
            ClientSecret = clientSecret;
        }

        /// <summary>
        /// Gets the source data to sign.
        /// </summary>
        [SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "OK in that case - binary data.")]
        public byte[] Data { get; }

        /// <summary>
        /// Gets the client secret.
        /// </summary>
        public string ClientSecret { get; }
    }
}
