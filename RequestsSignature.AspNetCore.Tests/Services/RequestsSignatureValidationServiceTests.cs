using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Options;
using Moq;
using NodaTime;
using RequestsSignature.AspNetCore.Services;
using RequestsSignature.Core;
using Xunit;

namespace RequestsSignature.AspNetCore.Tests.Services
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
            var requestSignerMock = new Mock<IRequestSigner>();
            var service = new RequestsSignatureValidationService(
                optionsMonitor,
                requestSignerMock.Object);

            var httpRequest = new DefaultHttpRequest(new DefaultHttpContext());
            var result = await service.Validate(httpRequest);

            result.Status.Should().Be(SignatureValidationResultStatus.Disabled);
        }

        [Fact]
        public async Task ItShouldValidateHeaderPresence()
        {
            var optionsMonitor = CreateOptionsMonitor();
            var requestSignerMock = new Mock<IRequestSigner>();
            var service = new RequestsSignatureValidationService(
                optionsMonitor,
                requestSignerMock.Object);

            var httpRequest = new DefaultHttpRequest(new DefaultHttpContext());
            var result = await service.Validate(httpRequest);

            result.Status.Should().Be(SignatureValidationResultStatus.HeaderNotFound);
        }

        [Fact]
        public async Task ItShouldValidateHeaderFormat()
        {
            var optionsMonitor = CreateOptionsMonitor();
            var requestSignerMock = new Mock<IRequestSigner>();
            var service = new RequestsSignatureValidationService(
                optionsMonitor,
                requestSignerMock.Object);

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
            var requestSignerMock = new Mock<IRequestSigner>();
            var service = new RequestsSignatureValidationService(
                optionsMonitor,
                requestSignerMock.Object);

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
            var nonce = "asdf";
            var optionsMonitor = CreateOptionsMonitor();
            var requestSignerMock = new Mock<IRequestSigner>();
            var nonceRepository = new Mock<INonceRepository>();
            nonceRepository.Setup(x => x.Exists(nonce))
                .ReturnsAsync(true);
            var service = new RequestsSignatureValidationService(
                optionsMonitor,
                requestSignerMock.Object,
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
            var requestSignerMock = new Mock<IRequestSigner>();
            var service = new RequestsSignatureValidationService(
                optionsMonitor,
                requestSignerMock.Object);

            var timestamp = SystemClock.Instance.GetCurrentInstant().Plus(Duration.FromTimeSpan(optionsMonitor.CurrentValue.ClockSkew).Plus(Duration.FromMinutes(1)));
            var httpRequest = new DefaultHttpRequest(new DefaultHttpContext());
            httpRequest.Headers[optionsMonitor.CurrentValue.HeaderName] = $"{ClientId}:asdf:{timestamp.ToUnixTimeSeconds()}:asdf";
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
            var requestSignerMock = new Mock<IRequestSigner>();
            requestSignerMock.Setup(x => x.CreateSignatureBody(It.IsAny<SigningBodyRequest>()))
                .ReturnsAsync(expectedSignature);
            var service = new RequestsSignatureValidationService(
                optionsMonitor,
                requestSignerMock.Object);

            var timestamp = SystemClock.Instance.GetCurrentInstant();
            var httpRequest = new DefaultHttpRequest(new DefaultHttpContext());
            httpRequest.Headers[optionsMonitor.CurrentValue.HeaderName] = $"{ClientId}:asdf:{timestamp.ToUnixTimeSeconds()}:asdf";
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
            var requestSignerMock = new Mock<IRequestSigner>();
            requestSignerMock.Setup(x => x.CreateSignatureBody(It.IsAny<SigningBodyRequest>()))
                .ReturnsAsync(expectedSignature);
            var service = new RequestsSignatureValidationService(
                optionsMonitor,
                requestSignerMock.Object);

            var timestamp = SystemClock.Instance.GetCurrentInstant();
            var httpRequest = new DefaultHttpRequest(new DefaultHttpContext());
            httpRequest.Headers[optionsMonitor.CurrentValue.HeaderName] = $"{ClientId}:asdf:{timestamp.ToUnixTimeSeconds()}:{expectedSignature}";
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
                options.Clients = new List<RequestsSignatureClientOptions>
                {
                    new RequestsSignatureClientOptions
                    {
                        ClientId = ClientId,
                        Key = ClientKey,
                    },
                };
            }));
            var optionsFactory = new OptionsFactory<RequestsSignatureOptions>(new[] { configureOptions }, Enumerable.Empty<IPostConfigureOptions<RequestsSignatureOptions>>());
            return new OptionsMonitor<RequestsSignatureOptions>(optionsFactory, Enumerable.Empty<IOptionsChangeTokenSource<RequestsSignatureOptions>>(), new OptionsCache<RequestsSignatureOptions>());
        }
    }
}
