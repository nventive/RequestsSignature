using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace AspNetCoreRequestsSignature
{
    /// <summary>
    /// Helper class to extract values from a <see cref="HttpRequest"/> using a pattern.
    /// </summary>
    public static class RequestValuePatternExtractor
    {
        /// <summary>
        /// Construct a specific string using <paramref name="pattern"/> from a supply <paramref name="request"/>.
        /// </summary>
        /// <param name="request">The <see cref="HttpRequest"/> to parse.</param>
        /// <param name="pattern">The pattern to use.</param>
        /// <returns>The constructed string.</returns>
        public static async Task<string> ExtractFromPattern(HttpRequest request, string pattern)
        {
            if (request == null)
            {
                throw new System.ArgumentNullException(nameof(request));
            }

            if (pattern == null)
            {
                throw new System.ArgumentNullException(nameof(pattern));
            }

            var value = pattern
                .Replace("{Method}", request.Method)
                .Replace("{Scheme}", request.Scheme)
                .Replace("{Host}", request.Host.ToString())
                .Replace("{Path}", request.Path)
                .Replace("{QueryString}", request.QueryString.ToString());

            if (pattern.Contains("{Body}"))
            {
                request.Body?.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(request.Body, Encoding.UTF8, false, 1024, true))
                {
                    value = value.Replace("{Body}", request.Body == null ? string.Empty : await reader.ReadToEndAsync());
                }

                request.Body?.Seek(0, SeekOrigin.Begin);
            }

            if (pattern.Contains("{Header"))
            {
                foreach (var headerName in request.Headers.Keys)
                {
                    var headerPattern = $"{{Header{headerName}}}";
                    value = value.Replace(headerPattern, request.Headers[headerName]);
                }
            }

            return value;
        }
    }
}
