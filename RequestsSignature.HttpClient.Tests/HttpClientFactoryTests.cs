using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using RequestsSignature.AspNetCore;
using RequestsSignature.HttpClient.Tests.Server;
using Xunit;

namespace RequestsSignature.HttpClient.Tests
{
    [Collection(nameof(ServerWithMiddlewareCollection))]
    public class HttpClientFactoryTests
    {
        private readonly ServerFixture<StartupWithMiddleware> _fixture;

        public HttpClientFactoryTests(ServerFixture<StartupWithMiddleware> fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task ItShouldWorkWithHttpClientFactoryWithNoOptions()
        {
            var services = new ServiceCollection();
            services
                .AddSingleton(new RequestsSignatureOptions
                {
                    ClientId = StartupWithMiddleware.DefaultClientId,
                    ClientSecret = StartupWithMiddleware.DefaultClientSecret,
                })
                .AddHttpClient(
                    "TheClient",
                    options =>
                    {
                        options.BaseAddress = _fixture.ServerUri;
                    })
                .AddRequestsSignature();

            var client = services.BuildServiceProvider().GetRequiredService<IHttpClientFactory>().CreateClient("TheClient");
            var response = await client.GetAsync(ApiController.GetSignatureValidationResultGetUri);

            var result = await response.Content.ReadAsAsync<SignatureValidationResult>();

            result.Status.Should().Be(SignatureValidationResultStatus.OK);
            result.ClientId.Should().Be(StartupWithMiddleware.DefaultClientId);
        }

        [Fact]
        public async Task ItShouldWorkWithHttpClientFactoryWithOptions()
        {
            var services = new ServiceCollection();
            services.AddHttpClient(
                "TheClient",
                options =>
                {
                    options.BaseAddress = _fixture.ServerUri;
                })
                .AddRequestsSignature(new RequestsSignatureOptions
                {
                    ClientId = StartupWithMiddleware.DefaultClientId,
                    ClientSecret = StartupWithMiddleware.DefaultClientSecret,
                });

            var client = services.BuildServiceProvider().GetRequiredService<IHttpClientFactory>().CreateClient("TheClient");
            var response = await client.GetAsync(ApiController.GetSignatureValidationResultGetUri);

            var result = await response.Content.ReadAsAsync<SignatureValidationResult>();

            result.Status.Should().Be(SignatureValidationResultStatus.OK);
            result.ClientId.Should().Be(StartupWithMiddleware.DefaultClientId);
        }
    }
}
