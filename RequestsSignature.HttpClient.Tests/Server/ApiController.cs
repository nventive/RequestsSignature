using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RequestsSignature.HttpClient.Tests.Server
{
    [ApiController]
    public class ApiController : ControllerBase
    {
        public const string GetSignatureValidationResultGetUri = "";

        [HttpGet(GetSignatureValidationResultGetUri)]
        public IActionResult GetSignatureValidationResultGet()
            => Ok(HttpContext.GetSignatureValidationResult());
    }
}
