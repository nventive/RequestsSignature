using System.Threading.Tasks;

namespace RequestsSignature.Core
{
    /// <summary>
    /// Builds the source for the signature body.
    /// </summary>
    public interface ISignatureBodySourceBuilder
    {
        /// <summary>
        /// Builds the source value for the signature body.
        /// </summary>
        /// <param name="signingRequest">The <see cref="SigningBodyRequest"/>.</param>
        /// <returns>The source value that needs to be signed.</returns>
        Task<byte[]> Build(SigningBodyRequest signingRequest);
    }
}
