using System;
using System.Collections.Generic;
using System.Globalization;
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
            var method = "POST";
            var uri = new Uri("https://example.org/api/users?search=foo");
            var headers = new Dictionary<string, string> { { "Header1", "Value1" } };
            var nonce = Guid.NewGuid().ToString();
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var clientId = "ClientId";
            var key = "Key";
            var body = Encoding.UTF8.GetBytes("FOO");

            yield return new object[]
            {
                new SigningBodyRequest(
                    method,
                    uri,
                    headers,
                    nonce,
                    timestamp,
                    clientId,
                    key,
                    new List<string>()),
                Array.Empty<byte>(),
            };

            yield return new object[]
            {
                new SigningBodyRequest(
                    method,
                    uri,
                    headers,
                    nonce,
                    timestamp,
                    clientId,
                    key,
                    new List<string> { SignatureBodySourceComponents.Nonce }),
                Encoding.UTF8.GetBytes(nonce),
            };

            yield return new object[]
            {
                new SigningBodyRequest(
                    method,
                    uri,
                    headers,
                    nonce,
                    timestamp,
                    clientId,
                    key,
                    new List<string> { SignatureBodySourceComponents.Nonce, SignatureBodySourceComponents.Timestamp }),
                Encoding.UTF8.GetBytes(nonce).Concat(Encoding.UTF8.GetBytes(timestamp.ToString(CultureInfo.InvariantCulture))).ToArray(),
            };

            yield return new object[]
            {
                new SigningBodyRequest(
                    method,
                    uri,
                    headers,
                    nonce,
                    timestamp,
                    clientId,
                    key,
                    new List<string> { SignatureBodySourceComponents.Method, SignatureBodySourceComponents.Scheme, SignatureBodySourceComponents.Host, SignatureBodySourceComponents.LocalPath, SignatureBodySourceComponents.QueryString }),
                Encoding.UTF8.GetBytes("POST")
                    .Concat(Encoding.UTF8.GetBytes(uri.Scheme))
                    .Concat(Encoding.UTF8.GetBytes(uri.Host))
                    .Concat(Encoding.UTF8.GetBytes(uri.LocalPath))
                    .Concat(Encoding.UTF8.GetBytes(uri.Query))
                    .ToArray(),
            };

            yield return new object[]
            {
                new SigningBodyRequest(
                    method,
                    uri,
                    headers,
                    nonce,
                    timestamp,
                    clientId,
                    key,
                    new List<string> { SignatureBodySourceComponents.Body },
                    body),
                body,
            };

            yield return new object[]
            {
                new SigningBodyRequest(
                    method,
                    uri,
                    headers,
                    nonce,
                    timestamp,
                    clientId,
                    key,
                    new List<string> { SignatureBodySourceComponents.Header("Header1") }),
                Encoding.UTF8.GetBytes("Value1"),
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
