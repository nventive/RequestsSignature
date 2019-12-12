using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
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
            var client = CreateClientWithSignature(StartupWithMiddleware.DefaultClientId, StartupWithMiddleware.DefaultClientSecret);

            var response = await client.GetAsync(ApiController.GetSignatureValidationResultGetUri);

            var result = await response.Content.ReadAsAsync<SignatureValidationResult>();

            result.Status.Should().Be(SignatureValidationResultStatus.OK);
            result.ClientId.Should().Be(StartupWithMiddleware.DefaultClientId);
        }

        [Fact]
        public async Task ItShouldSignAndValidateGetRequestWithCustomConfig()
        {
            var requestsSignatureOptions = new RequestsSignatureOptions
            {
                ClientId = StartupWithMiddleware.CustomClientId,
                ClientSecret = StartupWithMiddleware.CustomClientSecret,
            };
            foreach (var sourceComponent in StartupWithMiddleware.CustomSignatureBodySourceComponents)
            {
                requestsSignatureOptions.SignatureBodySourceComponents.Add(sourceComponent);
            }

            var client = CreateClientWithSignature(requestsSignatureOptions);

            var response = await client.GetAsync(ApiController.GetSignatureValidationResultGetUri);

            var result = await response.Content.ReadAsAsync<SignatureValidationResult>();

            result.Status.Should().Be(SignatureValidationResultStatus.OK);
            result.ClientId.Should().Be(StartupWithMiddleware.CustomClientId);
        }

        [Fact]
        public async Task ItShouldSignAndValidateGetRequestWithEncodedQueryString()
        {
            var client = CreateClientWithSignature(StartupWithMiddleware.DefaultClientId, StartupWithMiddleware.DefaultClientSecret);

            var response = await client.GetAsync($"{ApiController.GetSignatureValidationResultGetUri}?search={WebUtility.UrlEncode("value with spaces")}");

            var result = await response.Content.ReadAsAsync<SignatureValidationResult>();

            result.Status.Should().Be(SignatureValidationResultStatus.OK);
            result.ClientId.Should().Be(StartupWithMiddleware.DefaultClientId);
        }

        [Fact]
        public async Task ItShouldNotAllowSameNonceTwiceIfConfigured()
        {
            var client = new System.Net.Http.HttpClient();
            var signatureBodySourceBuilder = new SignatureBodySourceBuilder();
            var signatureBodySigner = new HashAlgorithmSignatureBodySigner();
            var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_fixture.ServerUri, ApiController.GetSignatureValidationResultGetUri));
            var nonce = Guid.NewGuid().ToString();
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            var signatureBodySourceParameters = new SignatureBodySourceParameters(
                request.Method.ToString(),
                request.RequestUri,
                new Dictionary<string, string>(),
                nonce,
                timestamp,
                StartupWithMiddleware.DefaultClientId,
                DefaultConstants.SignatureBodySourceComponents);
            var signatureBodySource = await signatureBodySourceBuilder.Build(signatureBodySourceParameters);
            var signatureBody = await signatureBodySigner.Sign(new SignatureBodyParameters(signatureBodySource, StartupWithMiddleware.DefaultClientSecret));
            var signature = $"{StartupWithMiddleware.DefaultClientId}:{nonce}:{timestamp}:{signatureBody}";

            request.Headers.TryAddWithoutValidation(DefaultConstants.HeaderName, signature);

            var response = await client.SendAsync(request);

            var result = await response.Content.ReadAsAsync<SignatureValidationResult>();

            result.Status.Should().Be(SignatureValidationResultStatus.OK);
            result.ClientId.Should().Be(StartupWithMiddleware.DefaultClientId);

            timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            request = new HttpRequestMessage(HttpMethod.Get, new Uri(_fixture.ServerUri, ApiController.GetSignatureValidationResultGetUri));
            signatureBodySourceParameters = new SignatureBodySourceParameters(
                request.Method.ToString(),
                request.RequestUri,
                new Dictionary<string, string>(),
                nonce,
                timestamp,
                StartupWithMiddleware.DefaultClientId,
                DefaultConstants.SignatureBodySourceComponents);

            signatureBodySource = await signatureBodySourceBuilder.Build(signatureBodySourceParameters);
            signatureBody = await signatureBodySigner.Sign(new SignatureBodyParameters(signatureBodySource, StartupWithMiddleware.DefaultClientSecret));
            signature = $"{StartupWithMiddleware.DefaultClientId}:{nonce}:{timestamp}:{signatureBody}";
            request.Headers.TryAddWithoutValidation(DefaultConstants.HeaderName, signature);

            response = await client.SendAsync(request);

            result = await response.Content.ReadAsAsync<SignatureValidationResult>();
            result.Status.Should().Be(SignatureValidationResultStatus.NonceHasBeenUsedBefore);
        }

        [Fact]
        public async Task ItShouldRequireRequestsSignatureValidationWithFilter()
        {
            var requestsSignatureOptions = new RequestsSignatureOptions
            {
                ClientId = StartupWithMiddleware.CustomClientId,
                ClientSecret = StartupWithMiddleware.CustomClientSecret,
            };
            foreach (var sourceComponent in StartupWithMiddleware.CustomSignatureBodySourceComponents)
            {
                requestsSignatureOptions.SignatureBodySourceComponents.Add(sourceComponent);
            }

            var client = CreateClientWithSignature(requestsSignatureOptions);

            var response = await client.GetAsync(ApiController.GetSignatureValidationResultWithAttributeUri);

            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }

        [Fact]
        public async Task ItShouldRequireRequestsSignatureValidationWithFilterAndNoException()
        {
            var requestsSignatureOptions = new RequestsSignatureOptions
            {
                ClientId = StartupWithMiddleware.CustomClientId,
                ClientSecret = StartupWithMiddleware.CustomClientSecret,
            };
            foreach (var sourceComponent in StartupWithMiddleware.CustomSignatureBodySourceComponents)
            {
                requestsSignatureOptions.SignatureBodySourceComponents.Add(sourceComponent);
            }

            var client = CreateClientWithSignature(requestsSignatureOptions);

            var response = await client.GetAsync(ApiController.GetSignatureValidationResultWithAttributeNoExceptionUri);

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task ItShouldRequireRequestsSignatureValidationWithFilterDisabled()
        {
            var client = new System.Net.Http.HttpClient
            {
                BaseAddress = _fixture.ServerUri,
            };

            var response = await client.GetAsync(ApiController.GetSignatureValidationResultWithAttributeDisabledUri);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        private System.Net.Http.HttpClient CreateClientWithSignature(string clientId, string clientSecret)
            => CreateClientWithSignature(
                new RequestsSignatureOptions
                {
                    ClientId = clientId,
                    ClientSecret = clientSecret,
                });

        private System.Net.Http.HttpClient CreateClientWithSignature(RequestsSignatureOptions options)
        {
            return new System.Net.Http.HttpClient(
                new RequestsSignatureDelegatingHandler(options)
                {
                    InnerHandler = new HttpClientHandler(),
                })
            {
                BaseAddress = _fixture.ServerUri,
            };
        }
    }
}
