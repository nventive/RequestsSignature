using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RequestsSignature.AspNetCore.Authentication;
using RequestsSignature.AspNetCore.Mvc;

namespace RequestsSignature.HttpClient.Tests.Server
{
    [ApiController]
    public class ApiController : ControllerBase
    {
        public const string GetSignatureValidationResultGetUri = "";
        public const string GetSignatureValidationResultWithAuthenticationUri = "with-auth";
        public const string GetSignatureValidationResultWithAttributeUri = "with-attribute";
        public const string GetSignatureValidationResultWithAttributeNoExceptionUri = "with-attribute-no-exception";
        public const string GetSignatureValidationResultWithAttributeDisabledUri = "with-attribute-disabled";

        [HttpGet(GetSignatureValidationResultGetUri)]
        public IActionResult GetSignatureValidationResultGet()
            => Ok(HttpContext.GetSignatureValidationResult());

        [HttpGet(GetSignatureValidationResultWithAuthenticationUri)]
        [Authorize(AuthenticationSchemes = RequestsSignatureAuthenticationConstants.AuthenticationScheme)]
        public IActionResult GetSignatureValidationResultWithAuthentication()
            => Ok(HttpContext.GetSignatureValidationResult());

        [HttpGet(GetSignatureValidationResultWithAttributeUri)]
        [RequireRequestsSignatureValidation(StartupWithMiddleware.DefaultClientId)]
        public IActionResult GetSignatureValidationResultWithAttribute()
            => Ok(HttpContext.GetSignatureValidationResult());

        [HttpGet(GetSignatureValidationResultWithAttributeNoExceptionUri)]
        [RequireRequestsSignatureValidation(StartupWithMiddleware.DefaultClientId, ThrowsOnValidationError = false)]
        public IActionResult GetSignatureValidationResultWithAttributeNoException()
            => Ok(HttpContext.GetSignatureValidationResult());

        [HttpGet(GetSignatureValidationResultWithAttributeDisabledUri)]
        [RequireRequestsSignatureValidation]
        [IgnoreRequestsSignatureValidation]
        public IActionResult GetSignatureValidationResultWithAttributeDisabled()
            => Ok(HttpContext.GetSignatureValidationResult());
    }
}
