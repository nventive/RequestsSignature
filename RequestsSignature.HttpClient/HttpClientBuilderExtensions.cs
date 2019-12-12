using RequestsSignature.Core;
using RequestsSignature.HttpClient;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// <see cref="IHttpClientBuilder"/> extension methods.
    /// </summary>
    public static class HttpClientBuilderExtensions
    {
        /// <summary>
        /// Adds <see cref="RequestsSignatureDelegatingHandler"/> as a HttpMessageHandler in the client pipeline.
        /// </summary>
        /// <param name="httpClientBuilder">The <see cref="IHttpClientBuilder"/>.</param>
        /// <param name="options">The <see cref="RequestsSignatureOptions"/> to use. If not provided will retrieve from the container.</param>
        /// <param name="signatureBodySourceBuilder">The <see cref="ISignatureBodySourceBuilder"/>. If not provided will try to retrieve from the container.</param>
        /// <param name="signatureBodySigner">The <see cref="ISignatureBodySigner"/>. If not provided will try to retrieve from the container.</param>
        /// <returns>The updated <see cref="IHttpClientBuilder"/>.</returns>
        public static IHttpClientBuilder AddRequestsSignature(
            this IHttpClientBuilder httpClientBuilder,
            RequestsSignatureOptions options = null,
            ISignatureBodySourceBuilder signatureBodySourceBuilder = null,
            ISignatureBodySigner signatureBodySigner = null)
                => httpClientBuilder.AddHttpMessageHandler(
                    (sp) => new RequestsSignatureDelegatingHandler(
                        options ?? sp.GetRequiredService<RequestsSignatureOptions>(),
                        signatureBodySourceBuilder: signatureBodySourceBuilder ?? sp.GetService<ISignatureBodySourceBuilder>(),
                        signatureBodySigner: signatureBodySigner ?? sp.GetService<ISignatureBodySigner>()));
    }
}
