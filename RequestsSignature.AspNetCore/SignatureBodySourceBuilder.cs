using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.IO;

namespace RequestsSignature.AspNetCore
{
    /// <summary>
    /// <see cref="ISignatureBodySourceBuilder"/> default implementation.
    /// </summary>
    internal class SignatureBodySourceBuilder : ISignatureBodySourceBuilder
    {
        private readonly RecyclableMemoryStreamManager _memoryStreamManager = new RecyclableMemoryStreamManager();

        /// <inheritdoc />
        public async Task<byte[]> Build(SigningRequest signingRequest)
        {
            var result = new List<byte>();

            foreach (var component in signingRequest.Options.SignatureBodySourceComponents)
            {
                switch (component)
                {
                    case SignatureBodySourceComponents.Method:
                        result.AddRange(Encoding.UTF8.GetBytes(signingRequest.Request.Method));
                        break;
                    case SignatureBodySourceComponents.Scheme:
                        result.AddRange(Encoding.UTF8.GetBytes(signingRequest.Request.Scheme));
                        break;
                    case SignatureBodySourceComponents.Host:
                        result.AddRange(Encoding.UTF8.GetBytes(signingRequest.Request.Host.ToString()));
                        break;
                    case SignatureBodySourceComponents.Path:
                        result.AddRange(Encoding.UTF8.GetBytes(signingRequest.Request.Path));
                        break;
                    case SignatureBodySourceComponents.QueryString:
                        result.AddRange(Encoding.UTF8.GetBytes(signingRequest.Request.QueryString.ToString()));
                        break;
                    case SignatureBodySourceComponents.Body:
                        if (signingRequest.Request.Body != null)
                        {
                            signingRequest.Request.EnableRewind();
                            signingRequest.Request.Body.Seek(0, SeekOrigin.Begin);
                            using (var memoryStream = _memoryStreamManager.GetStream())
                            {
                                await signingRequest.Request.Body.CopyToAsync(memoryStream);
                                result.AddRange(memoryStream.ToArray());
                            }

                            signingRequest.Request.Body.Seek(0, SeekOrigin.Begin);
                        }

                        break;
                    case SignatureBodySourceComponents.Timestamp:
                        result.AddRange(Encoding.UTF8.GetBytes(signingRequest.Timestamp.ToString(CultureInfo.InvariantCulture)));
                        break;
                    case SignatureBodySourceComponents.Nonce:
                        result.AddRange(Encoding.UTF8.GetBytes(signingRequest.Nonce));
                        break;
                    default:
                        if (SignatureBodySourceComponents.IsHeader(component, out var headerName))
                        {
                            if (signingRequest.Request.Headers.ContainsKey(headerName))
                            {
                                result.AddRange(Encoding.UTF8.GetBytes(signingRequest.Request.Headers[headerName]));
                            }
                        }
                        else
                        {
                            throw new RequestsSignatureException($"Unknown component source {component}.");
                        }

                        break;
                }
            }

            return result.ToArray();
        }
    }
}
