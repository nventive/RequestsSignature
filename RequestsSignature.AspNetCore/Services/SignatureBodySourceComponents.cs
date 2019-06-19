using System;
using Microsoft.AspNetCore.Http;

namespace RequestsSignature.AspNetCore.Services
{
    /// <summary>
    /// Constants that represents signature body source components.
    /// </summary>
    public static class SignatureBodySourceComponents
    {
        /// <summary>
        /// The <see cref="HttpRequest.Method"/>.
        /// </summary>
        public const string Method = "Method";

        /// <summary>
        /// The <see cref="HttpRequest.Scheme"/>.
        /// </summary>
        public const string Scheme = "Scheme";

        /// <summary>
        /// The <see cref="HttpRequest.Host"/>.
        /// </summary>
        public const string Host = "Host";

        /// <summary>
        /// The <see cref="HttpRequest.Path"/>.
        /// </summary>
        public const string Path = "Path";

        /// <summary>
        /// The <see cref="HttpRequest.QueryString"/>.
        /// </summary>
        public const string QueryString = "QueryString";

        /// <summary>
        /// The <see cref="HttpRequest.Body"/>.
        /// </summary>
        public const string Body = "Body";

        /// <summary>
        /// The Timestamp.
        /// </summary>
        public const string Timestamp = "Timestamp";

        /// <summary>
        /// The Nonce.
        /// </summary>
        public const string Nonce = "Nonce";

        /// <summary>
        /// A specific header in <see cref="HttpRequest.Headers"/>.
        /// </summary>
        /// <param name="headerName">The name of the header in <see cref="HttpRequest.Headers"/>.</param>
        /// <returns>The source component name.</returns>
        public static string Header(string headerName) => $"Header{headerName}";

        /// <summary>
        /// Indicates whether the component is a header and extracts the header name if it is.
        /// </summary>
        /// <param name="component">The source component.</param>
        /// <param name="headerName">The extracted header name if it is a header component.</param>
        /// <returns>true if it is a header component, false if not.</returns>
        public static bool IsHeader(string component, out string headerName)
        {
            headerName = null;
            if (string.IsNullOrEmpty(component))
            {
                return false;
            }

            if (component.StartsWith("Header", StringComparison.Ordinal))
            {
                headerName = component.Substring("Header".Length);
                return true;
            }

            return false;
        }
    }
}
