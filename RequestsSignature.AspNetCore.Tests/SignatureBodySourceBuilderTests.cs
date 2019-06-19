using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Xunit;

namespace RequestsSignature.AspNetCore.Tests
{
    public class SignatureBodySourceBuilderTests
    {
        public static IEnumerable<object[]> ItShouldBuildSourceValueFromComponentsData()
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
                "nonce",
                0,
                new List<string>(),
                Array.Empty<byte>(),
            };

            yield return new object[]
            {
                request,
                "nonce",
                0,
                new List<string> { SignatureBodySourceComponents.Nonce },
                Encoding.UTF8.GetBytes("nonce"),
            };

            yield return new object[]
            {
                request,
                "nonce",
                0,
                new List<string> { SignatureBodySourceComponents.Nonce, SignatureBodySourceComponents.Timestamp },
                Encoding.UTF8.GetBytes("nonce").Concat(Encoding.UTF8.GetBytes("0")).ToArray(),
            };

            yield return new object[]
            {
                request,
                "nonce",
                0,
                new List<string> { SignatureBodySourceComponents.Method, SignatureBodySourceComponents.Scheme, SignatureBodySourceComponents.Host, SignatureBodySourceComponents.Path, SignatureBodySourceComponents.QueryString },
                Encoding.UTF8.GetBytes(request.Method)
                    .Concat(Encoding.UTF8.GetBytes(request.Scheme))
                    .Concat(Encoding.UTF8.GetBytes(request.Host.ToString()))
                    .Concat(Encoding.UTF8.GetBytes(request.Path))
                    .Concat(Encoding.UTF8.GetBytes(request.QueryString.ToString()))
                    .ToArray(),
            };

            yield return new object[]
            {
                request,
                "nonce",
                0,
                new List<string> { SignatureBodySourceComponents.Body },
                Encoding.UTF8.GetBytes("FOO"),
            };

            yield return new object[]
            {
                request,
                "nonce",
                0,
                new List<string> { SignatureBodySourceComponents.Header("Header2") },
                Encoding.UTF8.GetBytes("Value2"),
            };
        }

        [Theory]
        [MemberData(nameof(ItShouldBuildSourceValueFromComponentsData))]
        public async Task ItShouldBuildSourceValueFromComponents(
            HttpRequest request,
            string nonce,
            long timestamp,
            IList<string> components,
            byte[] expected)
        {
            var builder = new SignatureBodySourceBuilder();

            var signingRequest = new SigningRequest
            {
                Request = request,
                Nonce = nonce,
                Timestamp = timestamp,
                Options = new RequestsSignatureClientOptions { SignatureBodySourceComponents = components },
            };
            var result = await builder.Build(signingRequest);

            result.Should().BeEquivalentTo(expected);
        }
    }
}
