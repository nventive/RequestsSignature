using Microsoft.Extensions.DependencyInjection.Extensions;
using RequestsSignature.AspNetCore;
using RequestsSignature.Core;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for <see cref="IServiceCollection"/>.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the necessary services to support requests signature validation.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddRequestsSignatureValidation(this IServiceCollection services)
        {
            services.TryAddSingleton<IRequestsSignatureValidationService, RequestsSignatureValidationService>();
            services.TryAddSingleton<IRequestSigner, HashAlgorithmRequestSigner>();
            services.TryAddSingleton<ISignatureBodySourceBuilder, SignatureBodySourceBuilder>();
            return services;
        }
    }
}
