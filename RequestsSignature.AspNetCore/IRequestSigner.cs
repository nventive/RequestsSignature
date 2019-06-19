using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace RequestsSignature.AspNetCore
{
    /// <summary>
    /// Computes signature for requests.
    /// </summary>
    public interface IRequestSigner
    {
        /// <summary>
        /// Creates a signature for the <paramref name="request"/>.
        /// </summary>
        /// <param name="request">The <see cref="HttpRequest"/>.</param>
        /// <param name="clientOptions">The <see cref="RequestsSignatureClientOptions"/>.</param>
        /// <returns>The created signature.</returns>
        Task<string> CreateSignature(HttpRequest request, RequestsSignatureClientOptions clientOptions);
    }
}
