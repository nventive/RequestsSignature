using RequestsSignature.AspNetCore;

namespace Microsoft.AspNetCore.Http
{
    /// <summary>
    /// <see cref="HttpContext"/> extension methods.
    /// </summary>
    public static class HttpContextExtensions
    {
        /// <summary>
        /// Gets the <see cref="SignatureValidationResult"/>, if any.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/>.</param>
        /// <returns>The <see cref="SignatureValidationResult"/>, if any.</returns>
        public static SignatureValidationResult GetSignatureValidationResult(this HttpContext context)
            => context?.Features?.Get<IRequestsSignatureFeature>()?.ValidationResult;
    }
}
