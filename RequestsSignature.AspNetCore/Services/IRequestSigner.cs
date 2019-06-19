using System.Threading.Tasks;

namespace RequestsSignature.AspNetCore.Services
{
    /// <summary>
    /// Computes signature for requests.
    /// </summary>
    public interface IRequestSigner
    {
        /// <summary>
        /// Creates a signature for the <paramref name="signingRequest"/>.
        /// </summary>
        /// <param name="signingRequest">The <see cref="SigningRequest"/>.</param>
        /// <returns>The created signature.</returns>
        Task<string> CreateSignature(SigningRequest signingRequest);
    }
}
