using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace RequestsSignature.Core
{
    /// <summary>
    /// <see cref="ISignatureBodySourceBuilder"/> default implementation.
    /// </summary>
    public class SignatureBodySourceBuilder : ISignatureBodySourceBuilder
    {
        /// <inheritdoc />
        public async Task<byte[]> Build(SigningBodyRequest signingRequest)
        {
            if (signingRequest == null)
            {
                throw new ArgumentNullException(nameof(signingRequest));
            }

            var result = new List<byte>();

            if (signingRequest.SignatureBodySourceComponents != null)
            {
                foreach (var component in signingRequest.SignatureBodySourceComponents)
                {
                    switch (component)
                    {
                        case SignatureBodySourceComponents.Method:
                            result.AddRange(Encoding.UTF8.GetBytes(signingRequest.Method.ToUpperInvariant()));
                            break;
                        case SignatureBodySourceComponents.Scheme:
                            result.AddRange(Encoding.UTF8.GetBytes(signingRequest.Uri.Scheme));
                            break;
                        case SignatureBodySourceComponents.Host:
                            result.AddRange(Encoding.UTF8.GetBytes(signingRequest.Uri.Host));
                            break;
                        case SignatureBodySourceComponents.Port:
                            result.AddRange(Encoding.UTF8.GetBytes(signingRequest.Uri.Port.ToString(CultureInfo.InvariantCulture)));
                            break;
                        case SignatureBodySourceComponents.LocalPath:
                            result.AddRange(Encoding.UTF8.GetBytes(signingRequest.Uri.LocalPath));
                            break;
                        case SignatureBodySourceComponents.QueryString:
                            result.AddRange(Encoding.UTF8.GetBytes(signingRequest.Uri.Query));
                            break;
                        case SignatureBodySourceComponents.Body:
                            result.AddRange(signingRequest.Body ?? Array.Empty<byte>());
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
                                if (signingRequest.Headers.ContainsKey(headerName))
                                {
                                    result.AddRange(Encoding.UTF8.GetBytes(signingRequest.Headers[headerName]));
                                }
                            }
                            else
                            {
                                throw new RequestsSignatureException($"Unknown component source {component}.");
                            }

                            break;
                    }
                }
            }

            return result.ToArray();
        }
    }
}
