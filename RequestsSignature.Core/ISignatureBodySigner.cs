using System.Threading.Tasks;

namespace RequestsSignature.Core
{
    /// <summary>
    /// Computes signature for requests.
    /// </summary>
    public interface ISignatureBodySigner
    {
        /// <summary>
        /// Creates a signature.
        /// </summary>
        /// <param name="parameters">The <see cref="SignatureBodyParameters"/>.</param>
        /// <returns>The created signature.</returns>
        Task<string> Sign(SignatureBodyParameters parameters);
    }
}
