using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RequestsSignature.Core
{
    /// <summary>
    /// <see cref="ISignatureBodySigner"/> implementation that uses <see cref="HashAlgorithm"/>.
    /// </summary>
    public class HashAlgorithmSignatureBodySigner : ISignatureBodySigner
    {
        private static readonly Func<SignatureBodyParameters, HashAlgorithm> DefaultHashAlgorithmBuilder =
            parameters => new HMACSHA256(Encoding.UTF8.GetBytes(parameters.ClientSecret));

        private readonly Func<SignatureBodyParameters, HashAlgorithm> _hashAlgorithmBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="HashAlgorithmSignatureBodySigner"/> class.
        /// </summary>
        /// <param name="hashAlgorithmBuilder">Allows customization of the hash algorithm used. Defaults to <see cref="HMACSHA256"/>.</param>
        public HashAlgorithmSignatureBodySigner(Func<SignatureBodyParameters, HashAlgorithm> hashAlgorithmBuilder = null)
        {
            _hashAlgorithmBuilder = hashAlgorithmBuilder ?? DefaultHashAlgorithmBuilder;
        }

        /// <inheritdoc />
        public async Task<string> Sign(SignatureBodyParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            using (var hash = _hashAlgorithmBuilder(parameters))
            {
                return Convert.ToBase64String(hash.ComputeHash(parameters.Data));
            }
        }
    }
}
