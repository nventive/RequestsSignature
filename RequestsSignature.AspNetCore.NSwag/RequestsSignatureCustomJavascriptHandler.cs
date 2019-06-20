using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;

namespace RequestsSignature.AspNetCore.NSwag
{
    /// <summary>
    /// handler for custom Nswag javascript.
    /// </summary>
    public static class RequestsSignatureCustomJavascriptHandler
    {
        /// <summary>
        /// The path at which the attributions.txt is exposed.
        /// </summary>
        public const string Path = "/add-request-signature.js";

        /// <summary>
        /// Configures the pipeline to return the content of the ATTRIBUTIONS.txt file
        /// at the <see cref="Path"/> (/attributions.txt).
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/>.</param>
        public static void UseSwaggerUiRequestsSignatureValidation(this IApplicationBuilder app)
        {
            app.Map(Path, Handle);
        }

        /// <summary>
        /// Handler that returns the content of the ATTRIBUTIONS.txt file.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/>.</param>
        public static void Handle(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                var fileProvider = new EmbeddedFileProvider(typeof(RequestsSignatureCustomJavascriptHandler).Assembly);
                var attributions = fileProvider.GetFileInfo("add-request-signature.js");
                context.Response.ContentType = "text/javascript";
                using (var stream = attributions.CreateReadStream())
                {
                    await stream.CopyToAsync(context.Response.Body);
                }
            });
        }
    }
}
