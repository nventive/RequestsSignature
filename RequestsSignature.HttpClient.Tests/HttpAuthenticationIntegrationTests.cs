using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using RequestsSignature.AspNetCore;
using RequestsSignature.HttpClient.Tests.Server;
using Xunit;

namespace RequestsSignature.HttpClient.Tests
{
    [Collection(ServerWithAuthenticationCollection.Name)]
    public class HttpAuthenticationIntegrationTests
    {
        private readonly ServerFixture<StartupWithAuthentication> _fixture;

        public HttpAuthenticationIntegrationTests(ServerFixture<StartupWithAuthentication> fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task ItShouldSignAndValidateGetRequest()
        {
            var client = CreateClientWithSignature(StartupWithMiddleware.DefaultClientId, StartupWithMiddleware.DefaultClientSecret);

            var response = await client.GetAsync(ApiController.GetSignatureValidationResultWithAuthenticationUri);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

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

            var response = await client.GetAsync(ApiController.GetSignatureValidationResultWithAuthenticationUri);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadAsAsync<SignatureValidationResult>();

            result.Status.Should().Be(SignatureValidationResultStatus.OK);
            result.ClientId.Should().Be(StartupWithMiddleware.CustomClientId);
        }

        [Fact]
        public async Task ItShouldRefuseAuthenticationIfWrongSignature()
        {
            var client = CreateClientWithSignature(StartupWithMiddleware.DefaultClientId, "wrongkey");

            var response = await client.GetAsync(ApiController.GetSignatureValidationResultWithAuthenticationUri);
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task ItShouldAutoRetryOnClockSkew()
        {
            var client = CreateClientWithSignature(StartupWithMiddleware.DefaultClientId, StartupWithMiddleware.DefaultClientSecret);

            var request = new HttpRequestMessage(HttpMethod.Get, ApiController.GetSignatureValidationResultWithAuthenticationUri);
            Func<HttpRequestMessage, long> timestampClock = (r) => DateTimeOffset.UtcNow.Subtract(TimeSpan.FromHours(1)).ToUnixTimeSeconds();
            request.Options.Set(new HttpRequestOptionsKey<Func<HttpRequestMessage, long>>(RequestsSignatureDelegatingHandler.TimestampClockProperty), timestampClock);
            var response = await client.SendAsync(request);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadAsAsync<SignatureValidationResult>();

            result.Status.Should().Be(SignatureValidationResultStatus.OK);
            result.ClientId.Should().Be(StartupWithMiddleware.DefaultClientId);
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
