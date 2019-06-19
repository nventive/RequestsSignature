using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RequestsSignature.Core
{
    /// <summary>
    /// <see cref="IRequestSigner"/> implementation that uses <see cref="HashAlgorithm"/>.
    /// </summary>
    public class HashAlgorithmRequestSigner : IRequestSigner
    {
        private static readonly Func<SigningBodyRequest, HashAlgorithm> DefaultHashAlgorithmBuilder =
            signingRequest => new HMACSHA256(Encoding.UTF8.GetBytes(signingRequest.Key));

        private readonly ISignatureBodySourceBuilder _signatureBodySourceBuilder;
        private readonly Func<SigningBodyRequest, HashAlgorithm> _hashAlgorithmBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="HashAlgorithmRequestSigner"/> class.
        /// </summary>
        /// <param name="signatureBodySourceBuilder">The <see cref="ISignatureBodySourceBuilder"/>.</param>
        /// <param name="hashAlgorithmBuilder">Allows customization of the hash algorithm used. Defaults to <see cref="HMACSHA256"/>.</param>
        public HashAlgorithmRequestSigner(
            ISignatureBodySourceBuilder signatureBodySourceBuilder,
            Func<SigningBodyRequest, HashAlgorithm> hashAlgorithmBuilder = null)
        {
            _signatureBodySourceBuilder = signatureBodySourceBuilder ?? throw new ArgumentNullException(nameof(signatureBodySourceBuilder));
            _hashAlgorithmBuilder = hashAlgorithmBuilder ?? DefaultHashAlgorithmBuilder;
        }

        /// <inheritdoc />
        public async Task<string> CreateSignatureBody(SigningBodyRequest signingRequest)
        {
            if (signingRequest == null)
            {
                throw new ArgumentNullException(nameof(signingRequest));
            }

            using (var hash = _hashAlgorithmBuilder(signingRequest))
            {
                return Convert.ToBase64String(hash.ComputeHash(await _signatureBodySourceBuilder.Build(signingRequest)));
            }
        }
    }
}
