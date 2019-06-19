using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;

namespace RequestsSignature.Core.Tests
{
    public class HashAlgorithmRequestSignerTests
    {
        [Fact]
        public async Task ItShouldComputeHash()
        {
            var signingRequest = new SigningBodyRequest
            {
                Key = "Key",
            };
            var signatureBodySourceBuilder = new Mock<ISignatureBodySourceBuilder>();
            signatureBodySourceBuilder.Setup(x => x.Build(signingRequest))
                .ReturnsAsync(new byte[] { 1 });
            var signer = new HashAlgorithmRequestSigner(signatureBodySourceBuilder.Object);

            var result = await signer.CreateSignatureBody(signingRequest);
            result.Should().NotBeEmpty();

            var bytes = Convert.FromBase64String(result);
            bytes.Should().HaveCount(32);
        }
    }
}
