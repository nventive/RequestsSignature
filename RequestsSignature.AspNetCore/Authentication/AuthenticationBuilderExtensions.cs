using System;
using Microsoft.AspNetCore.Authentication;
using RequestsSignature.AspNetCore.Authentication;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// <see cref="AuthenticationBuilder"/> extension methods.
    /// </summary>
    public static class AuthenticationBuilderExtensions
    {
        /// <summary>
        /// Adds requests signature authentication scheme.
        /// </summary>
        /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
        /// <returns>The configured <see cref="AuthenticationBuilder"/>.</returns>
        public static AuthenticationBuilder AddRequestsSignature(this AuthenticationBuilder builder)
                => builder?.AddScheme<RequestsSignatureAuthenticationOptions, RequestsSignatureAuthenticationHandler>(
                    RequestsSignatureAuthenticationConstants.AuthenticationScheme, RequestsSignatureAuthenticationConstants.DisplayName, options => { });

        /// <summary>
        /// Adds requests signature authentication scheme.
        /// </summary>
        /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
        /// <param name="authenticationScheme">The name of this scheme.</param>
        /// <param name="displayName">The display name of this scheme.</param>
        /// <param name="configureOptions">Used to configure the scheme options.</param>
        /// <returns>The configured <see cref="AuthenticationBuilder"/>.</returns>
        public static AuthenticationBuilder AddRequestsSignature(
            this AuthenticationBuilder builder,
            string authenticationScheme,
            string displayName,
            Action<RequestsSignatureAuthenticationOptions> configureOptions)
                => builder?.AddScheme<RequestsSignatureAuthenticationOptions, RequestsSignatureAuthenticationHandler>(authenticationScheme, displayName, configureOptions);
    }
}
