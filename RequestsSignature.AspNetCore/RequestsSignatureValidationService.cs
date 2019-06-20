using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.IO;
using RequestsSignature.AspNetCore.Nonces;
using RequestsSignature.Core;

namespace RequestsSignature.AspNetCore
{
    /// <summary>
    /// <see cref="IRequestsSignatureValidationService"/> default implementation.
    /// </summary>
    internal class RequestsSignatureValidationService : IRequestsSignatureValidationService
    {
        private readonly IOptionsMonitor<RequestsSignatureOptions> _optionsMonitor;
        private readonly IRequestSigner _requestSigner;
        private readonly ISystemClock _clock;
        private readonly INonceRepository _nonceRepository;
        private readonly ILogger _logger;

        private readonly RecyclableMemoryStreamManager _memoryStreamManager = new RecyclableMemoryStreamManager();
        private RequestsSignatureOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestsSignatureValidationService"/> class.
        /// </summary>
        /// <param name="options">The <see cref="RequestsSignatureOptions"/>.</param>
        /// <param name="requestSigner">The <see cref="IRequestSigner"/>.</param>
        /// <param name="clock">The <see cref="ISystemClock"/> instance for time retrieval. Defaults to <see cref="SystemClock"/>.</param>
        /// <param name="nonceRepository">The <see cref="INonceRepository"/> to use. Defaults to <see cref="NullNonceRepository"/>.</param>
        /// <param name="logger">The <see cref="ILogger"/> to use. Defaults to <see cref="NullLogger"/>.</param>
        public RequestsSignatureValidationService(
            IOptionsMonitor<RequestsSignatureOptions> options,
            IRequestSigner requestSigner,
            ISystemClock clock = null,
            INonceRepository nonceRepository = null,
            ILogger<RequestsSignatureValidationService> logger = null)
        {
            _optionsMonitor = options ?? throw new ArgumentNullException(nameof(options));
            _requestSigner = requestSigner ?? throw new ArgumentNullException(nameof(requestSigner));
            _clock = clock ?? new SystemClock();
            _nonceRepository = nonceRepository ?? NullNonceRepository.Instance;
            _logger = (ILogger)logger ?? NullLogger.Instance;
            _optionsMonitor.OnChange(OnOptionsChange);
            OnOptionsChange(_optionsMonitor.CurrentValue);
        }

        /// <inheritdoc />
        public async Task<SignatureValidationResult> Validate(HttpRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var serverTimestamp = _clock.UtcNow.ToUnixTimeSeconds();
            SignatureValidationResult result;

            if (_options.Disabled)
            {
                result = new SignatureValidationResult(
                    SignatureValidationResultStatus.Disabled,
                    serverTimestamp);
                _logger.SignatureValidationIgnored(result);
                return result;
            }

            var headerValue = request.Headers[_options.HeaderName];
            if (string.IsNullOrWhiteSpace(headerValue))
            {
                result = new SignatureValidationResult(
                    SignatureValidationResultStatus.HeaderNotFound,
                    serverTimestamp);
                _logger.SignatureValidationFailed(result);
                return result;
            }

            var signatureValue = headerValue.ToString().Trim();
            var headerMatch = _options.SignaturePattern.Match(signatureValue);
            if (!headerMatch.Success)
            {
                result = new SignatureValidationResult(
                    SignatureValidationResultStatus.HeaderParseError,
                    serverTimestamp,
                    signatureValue: signatureValue);
                _logger.SignatureValidationFailed(result);
                return result;
            }

            var signatureComponents = new SignatureValueComponents
            {
                ClientId = headerMatch.Groups["ClientId"]?.Value,
                Nonce = headerMatch.Groups["Nonce"]?.Value,
                Timestamp = long.Parse(headerMatch.Groups["Timestamp"]?.Value ?? "0", CultureInfo.InvariantCulture),
                SignatureBody = headerMatch.Groups["SignatureBody"]?.Value,
            };

            var clientOptions = _options.Clients.FirstOrDefault(x => string.Equals(x.ClientId, signatureComponents.ClientId, StringComparison.Ordinal));
            if (clientOptions == null)
            {
                result = new SignatureValidationResult(
                    SignatureValidationResultStatus.ClientIdNotFound,
                    serverTimestamp,
                    signatureValue: signatureValue,
                    clientId: signatureComponents.ClientId);
                _logger.SignatureValidationFailed(result);
                return result;
            }

            if (await _nonceRepository.Exists(signatureComponents.Nonce))
            {
                result = new SignatureValidationResult(
                    SignatureValidationResultStatus.NonceHasBeenUsedBefore,
                    serverTimestamp,
                    signatureValue: signatureValue,
                    clientId: clientOptions.ClientId);
                _logger.SignatureValidationFailed(result);
                return result;
            }

            if (Math.Abs(serverTimestamp - signatureComponents.Timestamp) > _options.ClockSkew.TotalSeconds)
            {
                result = new SignatureValidationResult(
                    SignatureValidationResultStatus.TimestampIsOff,
                    serverTimestamp,
                    signatureValue: signatureValue,
                    clientId: clientOptions.ClientId);
                _logger.SignatureValidationFailed(result);
                return result;
            }

            byte[] body = null;
            if (clientOptions.SignatureBodySourceComponents.Contains(SignatureBodySourceComponents.Body))
            {
                request.EnableRewind();
                request.Body.Seek(0, SeekOrigin.Begin);
                using (var memoryStream = _memoryStreamManager.GetStream())
                {
                    await request.Body.CopyToAsync(memoryStream);
                    body = memoryStream.ToArray();
                }

                request.Body.Seek(0, SeekOrigin.Begin);
            }

            var signingRequest = new SigningBodyRequest(
                request.Method,
                new Uri($"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}"),
                request.Headers.ToDictionary(x => x.Key, x => x.Value.ToString()),
                signatureComponents.Nonce,
                signatureComponents.Timestamp,
                clientOptions.ClientId,
                clientOptions.Key,
                clientOptions.SignatureBodySourceComponents,
                body);

            var signature = await _requestSigner.CreateSignatureBody(signingRequest);
            if (!string.Equals(signature, signatureComponents.SignatureBody, StringComparison.Ordinal))
            {
                result = new SignatureValidationResult(
                    SignatureValidationResultStatus.SignatureDoesntMatch,
                    serverTimestamp,
                    signatureValue: signatureValue,
                    clientId: clientOptions.ClientId,
                    computedSignature: signature);
                _logger.SignatureValidationFailed(result);
                return result;
            }

            await _nonceRepository.Add(signatureComponents.Nonce);
            result = new SignatureValidationResult(
                    SignatureValidationResultStatus.OK,
                    serverTimestamp,
                    signatureValue: signatureValue,
                    clientId: clientOptions.ClientId,
                    computedSignature: signature);
            _logger.SignatureValidationSucceeded(result);
            return result;
        }

        private void OnOptionsChange(RequestsSignatureOptions options)
        {
            _options = options;

            if (options.Disabled)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(_options.HeaderName))
            {
                throw new RequestsSignatureException($"Property {nameof(_options.HeaderName)} is null or empty.");
            }

            if (_options.SignaturePattern == null)
            {
                throw new RequestsSignatureException($"Property {nameof(_options.SignaturePattern)} is null.");
            }

            if (!_options.Clients.Any())
            {
                throw new RequestsSignatureException($"No {nameof(_options.Clients)} has been defined.");
            }

            foreach (var clientOptions in _options.Clients)
            {
                if (string.IsNullOrEmpty(clientOptions.ClientId))
                {
                    throw new RequestsSignatureException($"Property {nameof(clientOptions.ClientId)} is null or empty for a client.");
                }

                if (string.IsNullOrEmpty(clientOptions.Key))
                {
                    throw new RequestsSignatureException($"Property {nameof(clientOptions.Key)} is null or empty for client {clientOptions.ClientId}.");
                }

                if (!clientOptions.SignatureBodySourceComponents.Any())
                {
                    throw new RequestsSignatureException($"Property {nameof(clientOptions.SignatureBodySourceComponents)} is empty for client {clientOptions.ClientId}. No signature can be computed.");
                }
            }
        }

        private struct SignatureValueComponents
        {
            public string ClientId;

            public string Nonce;

            public long Timestamp;

            public string SignatureBody;
        }
    }
}
