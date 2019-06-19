using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using RequestsSignature.AspNetCore;
using RequestsSignature.HttpClient.Tests.Server;
using Xunit;

namespace RequestsSignature.HttpClient.Tests
{
    [Collection(ServerCollection.Name)]
    public class HttpIntegrationTests
    {
        private readonly ServerFixture _fixture;

        public HttpIntegrationTests(ServerFixture fixture)
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
                        ClientId = Startup.DefaultClientId,
                        Key = Startup.DefaultKey,
                    }))
            {
                BaseAddress = _fixture.ServerUri,
            };

            var response = await client.GetAsync(ApiController.GetSignatureValidationResultGetUri);

            var result = await response.Content.ReadAsAsync<SignatureValidationResult>();

            result.Status.Should().Be(SignatureValidationResultStatus.OK);
            result.ClientId.Should().Be(Startup.DefaultClientId);
        }

        [Fact]
        public async Task ItShouldSignAndValidateGetRequestWithCustomConfig()
        {
            var client = new System.Net.Http.HttpClient(
                new RequestsSignatureDelegatingHandler(
                    new RequestsSignatureOptions
                    {
                        ClientId = Startup.CustomClientId,
                        Key = Startup.CustomKey,
                        SignatureBodySourceComponents = Startup.CustomSignatureBodySourceComponents,
                    }))
            {
                BaseAddress = _fixture.ServerUri,
            };

            var response = await client.GetAsync(ApiController.GetSignatureValidationResultGetUri);

            var result = await response.Content.ReadAsAsync<SignatureValidationResult>();

            result.Status.Should().Be(SignatureValidationResultStatus.OK);
            result.ClientId.Should().Be(Startup.CustomClientId);
        }
    }
}
