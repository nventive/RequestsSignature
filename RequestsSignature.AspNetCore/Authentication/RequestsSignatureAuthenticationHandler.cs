using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RequestsSignature.AspNetCore.Services;

namespace RequestsSignature.AspNetCore.Authentication
{
    /// <summary>
    /// <see cref="AuthenticationHandler{TOptions}"/> for requests signature validation.
    /// </summary>
    public class RequestsSignatureAuthenticationHandler : AuthenticationHandler<RequestsSignatureAuthenticationOptions>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestsSignatureAuthenticationHandler"/> class.
        /// </summary>
        /// <param name="options">The <see cref="IOptionsMonitor{RequestsSignatureAuthenticationOptions}"/>.</param>
        /// <param name="logger">The <see cref="ILoggerFactory"/>.</param>
        /// <param name="encoder">The <see cref="UrlEncoder"/>.</param>
        /// <param name="clock">The <see cref="ISystemClock"/>.</param>
        public RequestsSignatureAuthenticationHandler(IOptionsMonitor<RequestsSignatureAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        /// <inheritdoc />
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var service = Context.RequestServices.GetRequiredService<IRequestsSignatureValidationService>();

            var result = await service.Validate(Context.Request);

            Context.Features.Set<IRequestsSignatureFeature>(new RequestsSignatureFeature(result));

            switch (result.Status)
            {
                case SignatureValidationResultStatus.Disabled:
                case SignatureValidationResultStatus.HeaderNotFound:
                    return AuthenticateResult.NoResult();
                case SignatureValidationResultStatus.OK:
                    var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(Options.ClientIdClaimName, result.ClientId) }));
                    return AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, new AuthenticationProperties(), Scheme.Name));
                default:
                    return AuthenticateResult.Fail($"Failed Request Signature: {result.Status}");
            }
        }
    }
}
