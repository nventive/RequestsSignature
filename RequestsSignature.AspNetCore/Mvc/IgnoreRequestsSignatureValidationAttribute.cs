using Microsoft.AspNetCore.Mvc.Filters;

namespace RequestsSignature.AspNetCore.Mvc
{
    /// <summary>
    /// Allows to bypass any <see cref="RequireRequestsSignatureValidationAttribute"/> for specific actions.
    /// </summary>
    public class IgnoreRequestsSignatureValidationAttribute : ActionFilterAttribute
    {
    }
}
