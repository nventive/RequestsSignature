using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using RequestsSignature.AspNetCore;
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
    }
}
