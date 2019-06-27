using System;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace RequestsSignature.Core.Tests
{
    public class HashAlgorithmSignatureBodySignerTests
    {
        [Fact]
        public async Task ItShouldSign()
        {
            var parameters = new SignatureBodyParameters(Array.Empty<byte>(), "key");
            var signer = new HashAlgorithmSignatureBodySigner();

            var result = await signer.Sign(parameters);
            result.Should().NotBeEmpty();

            var bytes = Convert.FromBase64String(result);
            bytes.Should().HaveCount(32);
        }
    }
}
