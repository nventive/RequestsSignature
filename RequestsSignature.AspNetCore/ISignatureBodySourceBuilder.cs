using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace RequestsSignature.AspNetCore
{
    /// <summary>
    /// Builds the source for the signature body.
    /// </summary>
    public interface ISignatureBodySourceBuilder
    {
        /// <summary>
        /// Builds the source value for the signature body.
        /// </summary>
        /// <param name="signingRequest">The <see cref="SigningRequest"/>.</param>
        /// <returns>The source value that needs to be signed.</returns>
        Task<byte[]> Build(SigningRequest signingRequest);
    }
}
