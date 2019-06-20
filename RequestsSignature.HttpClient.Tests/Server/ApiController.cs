using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RequestsSignature.AspNetCore.Authentication;

namespace RequestsSignature.HttpClient.Tests.Server
{
    [ApiController]
    public class ApiController : ControllerBase
    {
        public const string GetSignatureValidationResultGetUri = "";
        public const string GetSignatureValidationResultWithAuthenticationUri = "with-auth";

        [HttpGet(GetSignatureValidationResultGetUri)]
        public IActionResult GetSignatureValidationResultGet()
            => Ok(HttpContext.GetSignatureValidationResult());

        [HttpGet(GetSignatureValidationResultWithAuthenticationUri)]
        [Authorize(AuthenticationSchemes = RequestsSignatureAuthenticationConstants.AuthenticationScheme)]
        public IActionResult GetSignatureValidationResultWithAuthentication()
            => Ok(HttpContext.GetSignatureValidationResult());
    }
}
