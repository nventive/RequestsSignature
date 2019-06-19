using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace RequestsSignature.Core.Tests
{
    public class SignatureBodySourceBuilderTests
    {
        public static IEnumerable<object[]> ItShouldBuildSourceValueFromComponentsData()
        {
            yield return new object[]
            {
                new SigningBodyRequest { SignatureBodySourceComponents = new List<string>() },
                Array.Empty<byte>(),
            };

            yield return new object[]
            {
                new SigningBodyRequest
                {
                    Nonce = "nonce",
                    SignatureBodySourceComponents = new List<string> { SignatureBodySourceComponents.Nonce },
                },
                Encoding.UTF8.GetBytes("nonce"),
            };

            yield return new object[]
            {
                new SigningBodyRequest
                {
                    Nonce = "nonce",
                    Timestamp = 0,
                    SignatureBodySourceComponents = new List<string> { SignatureBodySourceComponents.Nonce, SignatureBodySourceComponents.Timestamp },
                },
                Encoding.UTF8.GetBytes("nonce").Concat(Encoding.UTF8.GetBytes("0")).ToArray(),
            };

            yield return new object[]
            {
                new SigningBodyRequest
                {
                    Method = "POST",
                    Scheme = "https",
                    Host = "example.org",
                    Path = "/api/users",
                    QueryString = "search=foo",
                    SignatureBodySourceComponents = new List<string> { SignatureBodySourceComponents.Method, SignatureBodySourceComponents.Scheme, SignatureBodySourceComponents.Host, SignatureBodySourceComponents.Path, SignatureBodySourceComponents.QueryString },
                },
                Encoding.UTF8.GetBytes("POST")
                    .Concat(Encoding.UTF8.GetBytes("https"))
                    .Concat(Encoding.UTF8.GetBytes("example.org"))
                    .Concat(Encoding.UTF8.GetBytes("/api/users"))
                    .Concat(Encoding.UTF8.GetBytes("search=foo"))
                    .ToArray(),
            };

            yield return new object[]
            {
                new SigningBodyRequest
                {
                    Body = Encoding.UTF8.GetBytes("FOO"),
                    SignatureBodySourceComponents = new List<string> { SignatureBodySourceComponents.Body },
                },
                Encoding.UTF8.GetBytes("FOO"),
            };

            yield return new object[]
            {
                new SigningBodyRequest
                {
                    Headers = new Dictionary<string, string> { { "Header2", "Value2" } },
                    SignatureBodySourceComponents = new List<string> { SignatureBodySourceComponents.Header("Header2") },
                },
                Encoding.UTF8.GetBytes("Value2"),
            };
        }

        [Theory]
        [MemberData(nameof(ItShouldBuildSourceValueFromComponentsData))]
        public async Task ItShouldBuildSourceValueFromComponents(SigningBodyRequest signingRequest, byte[] expected)
        {
            var builder = new SignatureBodySourceBuilder();
            var result = await builder.Build(signingRequest);
            result.Should().BeEquivalentTo(expected);
        }
    }
}
