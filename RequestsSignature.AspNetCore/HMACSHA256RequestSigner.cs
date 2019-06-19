using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace RequestsSignature.AspNetCore
{
    /// <summary>
    /// <see cref="IRequestSigner"/> implementation that uses <see cref="HMAC"/>.
    /// </summary>
    public class HMACSHA256RequestSigner : IRequestSigner
    {
        /// <inheritdoc />
        public async Task<string> CreateSignature(HttpRequest request, RequestsSignatureClientOptions clientOptions)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (clientOptions == null)
            {
                throw new ArgumentNullException(nameof(clientOptions));
            }

            var valueToSign = Encoding.UTF8.GetBytes(
                await RequestValuePatternExtractor.ExtractFromPattern(request, clientOptions.SignatureBodyPattern));
            using (var hash = new HMACSHA256(Encoding.UTF8.GetBytes(clientOptions.Key)))
            {
                return Convert.ToBase64String(hash.ComputeHash(valueToSign));
            }
        }
    }
}
