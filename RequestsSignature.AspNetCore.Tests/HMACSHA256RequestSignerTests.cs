using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Xunit;

namespace RequestsSignature.AspNetCore.Tests
{
    public class HMACSHA256RequestSignerTests
    {
        private const string Key = "foo";

        public static IEnumerable<object[]> ItShouldGetRequestSignatureData()
        {
            var request = new DefaultHttpRequest(new DefaultHttpContext())
            {
                Method = "POST",
                Scheme = "https",
                Host = new HostString("example.org"),
                Path = "/api/users",
                QueryString = QueryString.Create("search", "foo"),
            };
            request.Headers.Add("Header1", "Value1");
            request.Headers.Add("Header2", "Value2");
            request.Body = new MemoryStream(Encoding.UTF8.GetBytes("FOO"));

            yield return new object[]
            {
                request,
                new RequestsSignatureClientOptions { Key = Key, SignatureBodyPattern = string.Empty },
                string.Empty,
            };

            yield return new object[]
            {
                request,
                new RequestsSignatureClientOptions { Key = Key, SignatureBodyPattern = @"{Method} {Scheme}://{Host}{Path}" },
                "POST https://example.org/api/users",
            };

            yield return new object[]
            {
                request,
                new RequestsSignatureClientOptions { Key = Key, SignatureBodyPattern = @"{Method} {Scheme}://{Host}{Path}{QueryString}" },
                "POST https://example.org/api/users?search=foo",
            };

            yield return new object[]
            {
                request,
                new RequestsSignatureClientOptions { Key = Key, SignatureBodyPattern = @"{Method} {Scheme}://{Host}{Path}{QueryString} {HeaderHeader1}" },
                "POST https://example.org/api/users?search=foo Value1",
            };

            yield return new object[]
            {
                request,
                new RequestsSignatureClientOptions { Key = Key, SignatureBodyPattern = @"{Method} {Scheme}://{Host}{Path}{QueryString} {Body}" },
                "POST https://example.org/api/users?search=foo FOO",
            };
        }

        [Theory]
        [MemberData(nameof(ItShouldGetRequestSignatureData))]
        public async Task ItShouldGetRequestSignature(HttpRequest request, RequestsSignatureClientOptions options, string valueToSign)
        {
            var signer = new HMACSHA256RequestSigner();

            var result = await signer.CreateSignature(request, options);

            var computedValueToSign = Convert.ToBase64String(new HMACSHA256(Encoding.UTF8.GetBytes(Key)).ComputeHash(Encoding.UTF8.GetBytes(valueToSign)));

            result.Should().Be(computedValueToSign);
        }
    }
}
