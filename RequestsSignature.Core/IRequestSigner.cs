using System.Threading.Tasks;

namespace RequestsSignature.Core
{
    /// <summary>
    /// Computes signature for requests.
    /// </summary>
    public interface IRequestSigner
    {
        /// <summary>
        /// Creates a signature for the <paramref name="signingRequest"/>.
        /// </summary>
        /// <param name="signingRequest">The <see cref="SigningBodyRequest"/>.</param>
        /// <returns>The created signature.</returns>
        Task<string> CreateSignatureBody(SigningBodyRequest signingRequest);
    }
}
