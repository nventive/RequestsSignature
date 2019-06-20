using RequestsSignature.AspNetCore;
using RequestsSignature.AspNetCore.Middleware;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// <see cref="IApplicationBuilder"/> extension methods.
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds a middleware that performs Requests signature validation (using <see cref="IRequestsSignatureValidationService"/>).
        /// </summary>
        /// <param name="builder">The <see cref="IApplicationBuilder"/>.</param>
        /// <returns>The configured <see cref="IApplicationBuilder"/>.</returns>
        public static IApplicationBuilder UseRequestsSignatureValidation(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<RequestsSignatureMiddleware>();
            return builder;
        }
    }
}
