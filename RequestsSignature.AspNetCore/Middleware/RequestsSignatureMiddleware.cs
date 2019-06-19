using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using RequestsSignature.AspNetCore.Services;
using RequestsSignature.Core;

namespace RequestsSignature.AspNetCore.Middleware
{
    /// <summary>
    /// This middleware triggers the requests signature validation feature
    /// and set the <see cref="IRequestsSignatureFeature"/> in the current <see cref="HttpContext.Features"/>.
    /// </summary>
    public class RequestsSignatureMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestsSignatureMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next <see cref="RequestDelegate"/> in the chain.</param>
        public RequestsSignatureMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        /// <summary>
        /// Invoked by ASP.NET.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/>.</param>
        /// <param name="requestsSignatureValidationService">The <see cref="IRequestsSignatureValidationService"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task Invoke(HttpContext context, IRequestsSignatureValidationService requestsSignatureValidationService)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (requestsSignatureValidationService == null)
            {
                throw new RequestsSignatureException($"No {nameof(IRequestsSignatureValidationService)} implementation has been registered. Did you forget to call {nameof(ServiceCollectionExtensions.AddRequestsSignatureValidation)}?");
            }

            var result = await requestsSignatureValidationService.Validate(context.Request);

            context.Features.Set<IRequestsSignatureFeature>(new RequestsSignatureFeature(result));

            await _next(context);
        }
    }
}
