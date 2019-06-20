using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using RequestsSignature.AspNetCore;
using RequestsSignature.Core;
using RequestsSignature.HttpClient.Tests.Server;
using Xunit;

namespace RequestsSignature.HttpClient.Tests
{
    [Collection(nameof(ServerWithMiddlewareCollection))]
    public class HttpMiddlewareIntegrationTests
    {
        private readonly ServerFixture<StartupWithMiddleware> _fixture;

        public HttpMiddlewareIntegrationTests(ServerFixture<StartupWithMiddleware> fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task ItShouldSignAndValidateGetRequest()
        {
            var client = new System.Net.Http.HttpClient(
                new RequestsSignatureDelegatingHandler(
                    new RequestsSignatureOptions
                    {
                        ClientId = StartupWithMiddleware.DefaultClientId,
                        Key = StartupWithMiddleware.DefaultKey,
                    }))
            {
                BaseAddress = _fixture.ServerUri,
            };

            var response = await client.GetAsync(ApiController.GetSignatureValidationResultGetUri);

            var result = await response.Content.ReadAsAsync<SignatureValidationResult>();

            result.Status.Should().Be(SignatureValidationResultStatus.OK);
            result.ClientId.Should().Be(StartupWithMiddleware.DefaultClientId);
        }

        [Fact]
        public async Task ItShouldSignAndValidateGetRequestWithCustomConfig()
        {
            var client = new System.Net.Http.HttpClient(
                new RequestsSignatureDelegatingHandler(
                    new RequestsSignatureOptions
                    {
                        ClientId = StartupWithMiddleware.CustomClientId,
                        Key = StartupWithMiddleware.CustomKey,
                        SignatureBodySourceComponents = StartupWithMiddleware.CustomSignatureBodySourceComponents,
                    }))
            {
                BaseAddress = _fixture.ServerUri,
            };

            var response = await client.GetAsync(ApiController.GetSignatureValidationResultGetUri);

            var result = await response.Content.ReadAsAsync<SignatureValidationResult>();

            result.Status.Should().Be(SignatureValidationResultStatus.OK);
            result.ClientId.Should().Be(StartupWithMiddleware.CustomClientId);
        }

        [Fact]
        public async Task ItShouldSignAndValidateGetRequestWithEncodedQueryString()
        {
            var client = new System.Net.Http.HttpClient(
                new RequestsSignatureDelegatingHandler(
                    new RequestsSignatureOptions
                    {
                        ClientId = StartupWithMiddleware.DefaultClientId,
                        Key = StartupWithMiddleware.DefaultKey,
                    }))
            {
                BaseAddress = _fixture.ServerUri,
            };

            var response = await client.GetAsync($"{ApiController.GetSignatureValidationResultGetUri}?search={WebUtility.UrlEncode("value with spaces")}");

            var result = await response.Content.ReadAsAsync<SignatureValidationResult>();

            result.Status.Should().Be(SignatureValidationResultStatus.OK);
            result.ClientId.Should().Be(StartupWithMiddleware.DefaultClientId);
        }

        [Fact]
        public async Task ItShouldNotAllowSameNonceTwiceIfConfigured()
        {
            var client = new System.Net.Http.HttpClient();
            var requestSigner = new HashAlgorithmRequestSigner(new SignatureBodySourceBuilder());
            var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_fixture.ServerUri, ApiController.GetSignatureValidationResultGetUri));
            var nonce = Guid.NewGuid().ToString();
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            var signatureBodyRequest = new SigningBodyRequest(
                request.Method.ToString(),
                request.RequestUri,
                new Dictionary<string, string>(),
                nonce,
                timestamp,
                StartupWithMiddleware.DefaultClientId,
                StartupWithMiddleware.DefaultKey,
                DefaultConstants.SignatureBodySourceComponents);
            var signatureBody = await requestSigner.CreateSignatureBody(signatureBodyRequest);
            var signature = $"{StartupWithMiddleware.DefaultClientId}:{nonce}:{timestamp}:{signatureBody}";

            request.Headers.TryAddWithoutValidation(DefaultConstants.HeaderName, signature);

            var response = await client.SendAsync(request);

            var result = await response.Content.ReadAsAsync<SignatureValidationResult>();

            result.Status.Should().Be(SignatureValidationResultStatus.OK);
            result.ClientId.Should().Be(StartupWithMiddleware.DefaultClientId);

            timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            request = new HttpRequestMessage(HttpMethod.Get, new Uri(_fixture.ServerUri, ApiController.GetSignatureValidationResultGetUri));
            signatureBodyRequest = new SigningBodyRequest(
                request.Method.ToString(),
                request.RequestUri,
                new Dictionary<string, string>(),
                nonce,
                timestamp,
                StartupWithMiddleware.DefaultClientId,
                StartupWithMiddleware.DefaultKey,
                DefaultConstants.SignatureBodySourceComponents);
            signatureBody = await requestSigner.CreateSignatureBody(signatureBodyRequest);
            signature = $"{StartupWithMiddleware.DefaultClientId}:{nonce}:{timestamp}:{signatureBody}";
            request.Headers.TryAddWithoutValidation(DefaultConstants.HeaderName, signature);

            response = await client.SendAsync(request);

            result = await response.Content.ReadAsAsync<SignatureValidationResult>();
            result.Status.Should().Be(SignatureValidationResultStatus.NonceHasBeenUsedBefore);
        }
    }
}
