using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace RequestsSignature.AspNetCore.Tests
{
    public class RequestsSignatureOptionsTests
    {
        [Fact]
        public void ItShouldLoadFromFile()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var options = new RequestsSignatureOptions();
            configuration.GetSection(nameof(RequestsSignatureOptions)).Bind(options);

            options.Clients.Should().HaveCount(2);
            options.Clients.First().SignatureBodySourceComponents.Should().HaveCount(1);
        }
    }
}
