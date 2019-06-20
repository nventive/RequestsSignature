using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using RequestsSignature.Core;

namespace RequestsSignature.HttpClient
{
    /// <summary>
    /// <see cref="DelegatingHandler"/> that signs the outgoing requests.
    /// </summary>
    public class RequestsSignatureDelegatingHandler : DelegatingHandler
    {
        private readonly RequestsSignatureOptions _options;
        private readonly IRequestSigner _requestSigner;

        private long _clockSkew = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestsSignatureDelegatingHandler"/> class
        /// with a specific inner handler.
        /// </summary>
        /// <param name="options">The signature options.</param>
        /// <param name="requestSigner">The <see cref="IRequestSigner"/>.</param>
        /// <param name="innerHandler">The inner handler which is responsible for processing the HTTP response messages.</param>
        [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Not needed for HttpClient.")]
        public RequestsSignatureDelegatingHandler(
            RequestsSignatureOptions options,
            IRequestSigner requestSigner = null,
            HttpMessageHandler innerHandler = null)
            : base(innerHandler ?? new HttpClientHandler())
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _requestSigner = requestSigner ?? new HashAlgorithmRequestSigner(new SignatureBodySourceBuilder());
        }

        /// <inheritdoc />
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            ValidateOptions();
            await SignRequest(request);

            return await base.SendAsync(request, cancellationToken);
        }

        private async Task SignRequest(HttpRequestMessage request)
        {
            byte[] body = null;
            if (request.Content != null && _options.SignatureBodySourceComponents.Contains(SignatureBodySourceComponents.Body))
            {
                body = await request.Content.ReadAsByteArrayAsync();
            }

            var signingRequest = new SigningBodyRequest(
                request.Method.ToString(),
                request.RequestUri,
                request.Headers.ToDictionary(x => x.Key, x => x.Value.ToString()),
                Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture),
                GetTimestamp(),
                _options.ClientId,
                _options.Key,
                _options.SignatureBodySourceComponents,
                body);

            var signatureBody = await _requestSigner.CreateSignatureBody(signingRequest);

            var signature = _options.SignaturePatternBuilder
                .Replace("{ClientId}", _options.ClientId)
                .Replace("{Nonce}", signingRequest.Nonce)
                .Replace("{Timestamp}", signingRequest.Timestamp.ToString(CultureInfo.InvariantCulture))
                .Replace("{SignatureBody}", signatureBody);

            request.Headers.TryAddWithoutValidation(_options.HeaderName, signature);
        }

        private long GetTimestamp() => DateTimeOffset.UtcNow.ToUnixTimeSeconds() + _clockSkew;

        private void ValidateOptions()
        {
            if (string.IsNullOrWhiteSpace(_options.ClientId))
            {
                throw new RequestsSignatureException($"Missing {nameof(_options.ClientId)} in {nameof(RequestsSignatureDelegatingHandler)} options.");
            }

            if (string.IsNullOrWhiteSpace(_options.Key))
            {
                throw new RequestsSignatureException($"Missing {nameof(_options.Key)} in {nameof(RequestsSignatureDelegatingHandler)} options.");
            }

            if (string.IsNullOrWhiteSpace(_options.HeaderName))
            {
                throw new RequestsSignatureException($"Missing {nameof(_options.HeaderName)} in {nameof(RequestsSignatureDelegatingHandler)} options.");
            }

            if (string.IsNullOrWhiteSpace(_options.SignaturePatternBuilder))
            {
                throw new RequestsSignatureException($"Missing {nameof(_options.SignaturePatternBuilder)} in {nameof(RequestsSignatureDelegatingHandler)} options.");
            }

            if (!_options.SignatureBodySourceComponents.Any())
            {
                throw new RequestsSignatureException($"No {nameof(_options.SignatureBodySourceComponents)} is defined in {nameof(RequestsSignatureDelegatingHandler)} options.");
            }
        }
    }
}
