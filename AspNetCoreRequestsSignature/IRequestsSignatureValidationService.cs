using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace AspNetCoreRequestsSignature
{
    /// <summary>
    /// Validates signature of <see cref="HttpRequest"/>.
    /// </summary>
    public interface IRequestsSignatureValidationService
    {
        /// <summary>
        /// Validates a <paramref name="request"/>.
        /// </summary>
        /// <param name="request">The <see cref="HttpRequest"/> to validate.</param>
        /// <returns>The <see cref="SignatureValidationResult"/>.</returns>
        Task<SignatureValidationResult> Validate(HttpRequest request);
    }
}
