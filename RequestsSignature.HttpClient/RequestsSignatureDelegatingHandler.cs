using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Net;
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
        /// <summary>
        /// Gets the name of the property in <see cref="HttpRequestMessage.Properties"/>
        /// that can contain a Func{HttpRequestMessage, long} that returns the current UTC time as Unix Epoch Seconds.
        /// </summary>
        public const string TimestampClockProperty = "TimestampClock";

        private readonly RequestsSignatureOptions _options;
        private readonly ISignatureBodySourceBuilder _signatureBodySourceBuilder;
        private readonly ISignatureBodySigner _signatureBodySigner;

        private long _clockSkew = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestsSignatureDelegatingHandler"/> class
        /// with a specific inner handler.
        /// </summary>
        /// <param name="options">The signature options.</param>
        /// <param name="signatureBodySourceBuilder">The <see cref="ISignatureBodySourceBuilder"/>.</param>
        /// <param name="signatureBodySigner">The <see cref="ISignatureBodySigner"/>.</param>
        /// <param name="innerHandler">The inner handler which is responsible for processing the HTTP response messages.</param>
        [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Not needed for HttpClient.")]
        public RequestsSignatureDelegatingHandler(
            RequestsSignatureOptions options,
            ISignatureBodySourceBuilder signatureBodySourceBuilder = null,
            ISignatureBodySigner signatureBodySigner = null,
            HttpMessageHandler innerHandler = null)
            : base(innerHandler ?? new HttpClientHandler())
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _signatureBodySourceBuilder = signatureBodySourceBuilder ?? new SignatureBodySourceBuilder();
            _signatureBodySigner = signatureBodySigner ?? new HashAlgorithmSignatureBodySigner();
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

            var response = await base.SendAsync(request, cancellationToken);

            if (!_options.DisableAutoRetryOnClockSkew
                && ((response.StatusCode == HttpStatusCode.Unauthorized) || (response.StatusCode == HttpStatusCode.Forbidden))
                && response.Headers.Date.HasValue)
            {
                var serverDate = response.Headers.Date.Value.ToUnixTimeSeconds();
                var now = GetTime(request);
                if (Math.Abs(serverDate - now) > _options.ClockSkew.TotalSeconds)
                {
                    _clockSkew = serverDate - now;
                    await SignRequest(request);
                    response = await base.SendAsync(request, cancellationToken);
                }
            }

            return response;
        }

        /// <summary>
        /// Returns what the client think is the current time.
        /// </summary>
        private static long GetTime(HttpRequestMessage request)
        {
            return request.Properties.ContainsKey(TimestampClockProperty) && request.Properties[TimestampClockProperty] is Func<HttpRequestMessage, long>
                ? ((Func<HttpRequestMessage, long>)request.Properties[TimestampClockProperty])(request)
                : DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        private async Task SignRequest(HttpRequestMessage request)
        {
            request.Headers.Remove(_options.HeaderName);

            var signatureBodySourceComponents = _options.SignatureBodySourceComponents.Any()
                ? _options.SignatureBodySourceComponents
                : DefaultConstants.SignatureBodySourceComponents;

            byte[] body = null;
            if (request.Content != null && signatureBodySourceComponents.Contains(SignatureBodySourceComponents.Body))
            {
                body = await request.Content.ReadAsByteArrayAsync();
            }

            var signatureBodySourceParameters = new SignatureBodySourceParameters(
                request.Method.ToString(),
                request.RequestUri,
                request.Headers.ToDictionary(x => x.Key, x => x.Value.ToString()),
                Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture),
                GetTimestamp(request),
                _options.ClientId,
                signatureBodySourceComponents,
                body);

            var signatureBodySource = await _signatureBodySourceBuilder.Build(signatureBodySourceParameters);

            var signatureBodyParameters = new SignatureBodyParameters(signatureBodySource, _options.ClientSecret);
            var signatureBody = await _signatureBodySigner.Sign(signatureBodyParameters);

            var signature = _options.SignaturePattern
                .Replace("{ClientId}", _options.ClientId)
                .Replace("{Nonce}", signatureBodySourceParameters.Nonce)
                .Replace("{Timestamp}", signatureBodySourceParameters.Timestamp.ToString(CultureInfo.InvariantCulture))
                .Replace("{SignatureBody}", signatureBody);

            request.Headers.TryAddWithoutValidation(_options.HeaderName, signature);
        }

        /// <summary>
        /// Returns the timestamp, accounting for the perceived clock skrew.
        /// </summary>
        private long GetTimestamp(HttpRequestMessage request) => GetTime(request) + _clockSkew;

        private void ValidateOptions()
        {
            if (string.IsNullOrWhiteSpace(_options.ClientId))
            {
                throw new RequestsSignatureException($"Missing {nameof(_options.ClientId)} in {nameof(RequestsSignatureDelegatingHandler)} options.");
            }

            if (string.IsNullOrWhiteSpace(_options.ClientSecret))
            {
                throw new RequestsSignatureException($"Missing {nameof(_options.ClientSecret)} in {nameof(RequestsSignatureDelegatingHandler)} options.");
            }

            if (string.IsNullOrWhiteSpace(_options.HeaderName))
            {
                throw new RequestsSignatureException($"Missing {nameof(_options.HeaderName)} in {nameof(RequestsSignatureDelegatingHandler)} options.");
            }

            if (string.IsNullOrWhiteSpace(_options.SignaturePattern))
            {
                throw new RequestsSignatureException($"Missing {nameof(_options.SignaturePattern)} in {nameof(RequestsSignatureDelegatingHandler)} options.");
            }
        }
    }
}
