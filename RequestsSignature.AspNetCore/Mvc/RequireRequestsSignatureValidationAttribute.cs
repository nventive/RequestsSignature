using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RequestsSignature.AspNetCore.Mvc
{
    /// <summary>
    /// MVC Filter that Requires Requests Signature validation.
    /// </summary>
    public class RequireRequestsSignatureValidationAttribute : ActionFilterAttribute
    {
        private static readonly SignatureValidationResultStatus[] AcceptedStatus = new[] { SignatureValidationResultStatus.OK, SignatureValidationResultStatus.Disabled };

        /// <summary>
        /// Initializes a new instance of the <see cref="RequireRequestsSignatureValidationAttribute"/> class
        /// that specifies a specific client id to require.
        /// </summary>
        /// <param name="clientIds">The specific client ids to check.</param>
        public RequireRequestsSignatureValidationAttribute(params string[] clientIds)
        {
            ClientIds = clientIds;
        }

        /// <summary>
        /// Gets the list of client ids to check.
        /// </summary>
        public IEnumerable<string> ClientIds { get; }

        /// <summary>
        /// Gets or sets a value indicating whether to throw a <see cref="RequestsSignatureValidationException"/>
        /// if validation failed.
        /// If false, a 401 or 403 response will be returned instead.
        /// Defaults to true.
        /// </summary>
        public bool ThrowsOnValidationError { get; set; } = true;

        /// <inheritdoc />
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.ActionDescriptor.FilterDescriptors.Any(x => x.Filter is IgnoreRequestsSignatureValidationAttribute))
            {
                return;
            }

            var validationResult = context.HttpContext.GetSignatureValidationResult();
            if (validationResult == null || !AcceptedStatus.Contains(validationResult.Status))
            {
                if (ThrowsOnValidationError)
                {
                    throw new RequestsSignatureValidationException(validationResult);
                }

                context.Result = new UnauthorizedResult();
                return;
            }

            if (ClientIds != null && ClientIds.Any())
            {
                if (!ClientIds.Contains(validationResult.ClientId))
                {
                    if (ThrowsOnValidationError)
                    {
                        throw new RequestsSignatureValidationException(validationResult);
                    }

                    context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
                    return;
                }
            }

            base.OnActionExecuting(context);
        }
    }
}
