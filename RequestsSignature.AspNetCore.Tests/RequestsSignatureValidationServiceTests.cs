using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Options;
using Moq;
using RequestsSignature.Core;
using Xunit;

namespace RequestsSignature.AspNetCore.Tests
{
    public class RequestsSignatureValidationServiceTests
    {
        private const string ClientId = "ClientId";
        private const string ClientKey = "ClientKey";

        [Fact]
        public async Task ItShouldHonorDisabledOptions()
        {
            var optionsMonitor = CreateOptionsMonitor(options =>
            {
                options.Disabled = true;
            });
            var signatureBodySourceBuilderMock = new Mock<ISignatureBodySourceBuilder>();
            var signatureBodySignerMock = new Mock<ISignatureBodySigner>();
            var service = new RequestsSignatureValidationService(
                optionsMonitor,
                signatureBodySourceBuilderMock.Object,
                signatureBodySignerMock.Object);

            var httpRequest = new DefaultHttpRequest(new DefaultHttpContext());
            var result = await service.Validate(httpRequest);

            result.Status.Should().Be(SignatureValidationResultStatus.Disabled);
        }

        [Fact]
        public async Task ItShouldValidateHeaderPresence()
        {
            var optionsMonitor = CreateOptionsMonitor();
            var signatureBodySourceBuilderMock = new Mock<ISignatureBodySourceBuilder>();
            var signatureBodySignerMock = new Mock<ISignatureBodySigner>();
            var service = new RequestsSignatureValidationService(
                optionsMonitor,
                signatureBodySourceBuilderMock.Object,
                signatureBodySignerMock.Object);

            var httpRequest = new DefaultHttpRequest(new DefaultHttpContext());
            var result = await service.Validate(httpRequest);

            result.Status.Should().Be(SignatureValidationResultStatus.HeaderNotFound);
        }

        [Fact]
        public async Task ItShouldValidateHeaderFormat()
        {
            var optionsMonitor = CreateOptionsMonitor();
            var signatureBodySourceBuilderMock = new Mock<ISignatureBodySourceBuilder>();
            var signatureBodySignerMock = new Mock<ISignatureBodySigner>();
            var service = new RequestsSignatureValidationService(
                optionsMonitor,
                signatureBodySourceBuilderMock.Object,
                signatureBodySignerMock.Object);

            var httpRequest = new DefaultHttpRequest(new DefaultHttpContext());
            httpRequest.Headers[optionsMonitor.CurrentValue.HeaderName] = "foo";
            var result = await service.Validate(httpRequest);

            result.Status.Should().Be(SignatureValidationResultStatus.HeaderParseError);
            result.SignatureValue.Should().Be("foo");
        }

        [Fact]
        public async Task ItShouldValidateClientId()
        {
            var optionsMonitor = CreateOptionsMonitor();
            var signatureBodySourceBuilderMock = new Mock<ISignatureBodySourceBuilder>();
            var signatureBodySignerMock = new Mock<ISignatureBodySigner>();
            var service = new RequestsSignatureValidationService(
                optionsMonitor,
                signatureBodySourceBuilderMock.Object,
                signatureBodySignerMock.Object);

            var httpRequest = new DefaultHttpRequest(new DefaultHttpContext());
            httpRequest.Headers[optionsMonitor.CurrentValue.HeaderName] = "WrongClientId:asdf:0:asdf";
            var result = await service.Validate(httpRequest);

            result.Status.Should().Be(SignatureValidationResultStatus.ClientIdNotFound);
            result.SignatureValue.Should().Be(httpRequest.Headers[optionsMonitor.CurrentValue.HeaderName]);
            result.ClientId.Should().Be("WrongClientId");
        }

        [Fact]
        public async Task ItShouldValidateNonce()
        {
            var nonce = Guid.NewGuid().ToString();
            var optionsMonitor = CreateOptionsMonitor();
            var signatureBodySourceBuilderMock = new Mock<ISignatureBodySourceBuilder>();
            var signatureBodySignerMock = new Mock<ISignatureBodySigner>();
            var nonceRepository = new Mock<INonceRepository>();
            nonceRepository.Setup(x => x.Exists(nonce))
                .ReturnsAsync(true);
            var service = new RequestsSignatureValidationService(
                optionsMonitor,
                signatureBodySourceBuilderMock.Object,
                signatureBodySignerMock.Object,
                nonceRepository: nonceRepository.Object);

            var httpRequest = new DefaultHttpRequest(new DefaultHttpContext());
            httpRequest.Headers[optionsMonitor.CurrentValue.HeaderName] = $"{ClientId}:{nonce}:0:asdf";
            var result = await service.Validate(httpRequest);

            result.Status.Should().Be(SignatureValidationResultStatus.NonceHasBeenUsedBefore);
            result.SignatureValue.Should().Be(httpRequest.Headers[optionsMonitor.CurrentValue.HeaderName]);
            result.ClientId.Should().Be(ClientId);
        }

        [Fact]
        public async Task ItShouldValidateTimestamp()
        {
            var optionsMonitor = CreateOptionsMonitor();
            var signatureBodySourceBuilderMock = new Mock<ISignatureBodySourceBuilder>();
            var signatureBodySignerMock = new Mock<ISignatureBodySigner>();
            var service = new RequestsSignatureValidationService(
                optionsMonitor,
                signatureBodySourceBuilderMock.Object,
                signatureBodySignerMock.Object);

            var now = new SystemClock().UtcNow;
            var timestamp = now.Add(optionsMonitor.CurrentValue.ClockSkew).AddMinutes(1);
            var httpRequest = new DefaultHttpRequest(new DefaultHttpContext());
            httpRequest.Headers[optionsMonitor.CurrentValue.HeaderName] = $"{ClientId}:{Guid.NewGuid()}:{timestamp.ToUnixTimeSeconds()}:asdf";
            var result = await service.Validate(httpRequest);

            result.Status.Should().Be(SignatureValidationResultStatus.TimestampIsOff);
            result.SignatureValue.Should().Be(httpRequest.Headers[optionsMonitor.CurrentValue.HeaderName]);
            result.ClientId.Should().Be(ClientId);
        }

        [Fact]
        public async Task ItShouldValidateSignatureWhenNoMatch()
        {
            var expectedSignature = "expectedSignature";
            var optionsMonitor = CreateOptionsMonitor();
            var signatureBodySourceBuilderMock = new Mock<ISignatureBodySourceBuilder>();
            var signatureBodySignerMock = new Mock<ISignatureBodySigner>();
            signatureBodySignerMock.Setup(x => x.Sign(It.IsAny<SignatureBodyParameters>()))
                .ReturnsAsync(expectedSignature);
            var service = new RequestsSignatureValidationService(
                optionsMonitor,
                signatureBodySourceBuilderMock.Object,
                signatureBodySignerMock.Object);

            var timestamp = new SystemClock().UtcNow.ToUnixTimeSeconds();
            var httpRequest = new DefaultHttpRequest(new DefaultHttpContext())
            {
                Scheme = "https",
                Host = new HostString("example.org"),
                Path = "/",
            };
            httpRequest.Headers[optionsMonitor.CurrentValue.HeaderName] = $"{ClientId}:{Guid.NewGuid()}:{timestamp}:asdf";
            var result = await service.Validate(httpRequest);

            result.Status.Should().Be(SignatureValidationResultStatus.SignatureDoesntMatch);
            result.SignatureValue.Should().Be(httpRequest.Headers[optionsMonitor.CurrentValue.HeaderName]);
            result.ClientId.Should().Be(ClientId);
            result.ComputedSignature.Should().Be(expectedSignature);
        }

        [Fact]
        public async Task ItShouldValidateSignatureWhenMatch()
        {
            var expectedSignature = "expectedSignature";
            var optionsMonitor = CreateOptionsMonitor();
            var signatureBodySourceBuilderMock = new Mock<ISignatureBodySourceBuilder>();
            var signatureBodySignerMock = new Mock<ISignatureBodySigner>();
            signatureBodySignerMock.Setup(x => x.Sign(It.IsAny<SignatureBodyParameters>()))
                .ReturnsAsync(expectedSignature);
            var service = new RequestsSignatureValidationService(
                optionsMonitor,
                signatureBodySourceBuilderMock.Object,
                signatureBodySignerMock.Object);

            var timestamp = new SystemClock().UtcNow.ToUnixTimeSeconds();
            var httpRequest = new DefaultHttpRequest(new DefaultHttpContext())
            {
                Scheme = "https",
                Host = new HostString("example.org"),
                Path = "/",
            };
            httpRequest.Headers[optionsMonitor.CurrentValue.HeaderName] = $"{ClientId}:{Guid.NewGuid()}:{timestamp}:{expectedSignature}";
            var result = await service.Validate(httpRequest);

            result.Status.Should().Be(SignatureValidationResultStatus.OK);
            result.SignatureValue.Should().Be(httpRequest.Headers[optionsMonitor.CurrentValue.HeaderName]);
            result.ClientId.Should().Be(ClientId);
            result.ComputedSignature.Should().Be(expectedSignature);
        }

        private IOptionsMonitor<RequestsSignatureOptions> CreateOptionsMonitor(Action<RequestsSignatureOptions> configure = null)
        {
            var configureOptions = new ConfigureOptions<RequestsSignatureOptions>(configure ?? (options =>
            {
                options.Clients.Add(new RequestsSignatureClientOptions
                {
                    ClientId = ClientId,
                    Key = ClientKey,
                });
            }));
            var optionsFactory = new OptionsFactory<RequestsSignatureOptions>(new[] { configureOptions }, Enumerable.Empty<IPostConfigureOptions<RequestsSignatureOptions>>());
            return new OptionsMonitor<RequestsSignatureOptions>(optionsFactory, Enumerable.Empty<IOptionsChangeTokenSource<RequestsSignatureOptions>>(), new OptionsCache<RequestsSignatureOptions>());
        }
    }
}
