using Microsoft.AspNetCore.Authentication;

namespace RequestsSignature.AspNetCore.Authentication
{
    /// <summary>
    /// Options for <see cref="RequestsSignatureAuthenticationHandler"/>.
    /// </summary>
    public class RequestsSignatureAuthenticationOptions : AuthenticationSchemeOptions
    {
        /// <summary>
        /// Gets or sets the name of the claim that contains the client id.
        /// </summary>
        public string ClientIdClaimName { get; set; } = "ClientId";
    }
}
