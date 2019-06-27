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
        /// <param name="key">The key.</param>
        public SignatureBodyParameters(
            byte[] data,
            string key)
        {
            Data = data;
            Key = key;
        }

        /// <summary>
        /// Gets the source data to sign.
        /// </summary>
        [SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "OK in that case - binary data.")]
        public byte[] Data { get; }

        /// <summary>
        /// Gets the signature key.
        /// </summary>
        public string Key { get; }
    }
}
