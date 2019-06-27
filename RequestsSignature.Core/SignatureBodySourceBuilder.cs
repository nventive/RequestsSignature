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
        public async Task<byte[]> Build(SignatureBodySourceParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            var result = new List<byte>();

            if (parameters.SignatureBodySourceComponents != null)
            {
                foreach (var component in parameters.SignatureBodySourceComponents)
                {
                    switch (component)
                    {
                        case SignatureBodySourceComponents.Method:
                            if (parameters.Method != null)
                            {
                                result.AddRange(Encoding.UTF8.GetBytes(parameters.Method.ToUpperInvariant()));
                            }

                            break;
                        case SignatureBodySourceComponents.Scheme:
                            if (parameters.Uri != null && parameters.Uri.IsAbsoluteUri)
                            {
                                result.AddRange(Encoding.UTF8.GetBytes(parameters.Uri.Scheme));
                            }

                            break;
                        case SignatureBodySourceComponents.Host:
                            if (parameters.Uri != null && parameters.Uri.IsAbsoluteUri)
                            {
                                result.AddRange(Encoding.UTF8.GetBytes(parameters.Uri.Host));
                            }

                            break;
                        case SignatureBodySourceComponents.Port:
                            if (parameters.Uri != null && parameters.Uri.IsAbsoluteUri)
                            {
                                result.AddRange(Encoding.UTF8.GetBytes(parameters.Uri.Port.ToString(CultureInfo.InvariantCulture)));
                            }

                            break;
                        case SignatureBodySourceComponents.LocalPath:
                            if (parameters.Uri != null)
                            {
                                result.AddRange(Encoding.UTF8.GetBytes(parameters.Uri.LocalPath));
                            }

                            break;
                        case SignatureBodySourceComponents.QueryString:
                            if (parameters.Uri != null)
                            {
                                result.AddRange(Encoding.UTF8.GetBytes(parameters.Uri.Query));
                            }

                            break;
                        case SignatureBodySourceComponents.Body:
                            result.AddRange(parameters.Body ?? Array.Empty<byte>());
                            break;
                        case SignatureBodySourceComponents.Timestamp:
                            result.AddRange(Encoding.UTF8.GetBytes(parameters.Timestamp.ToString(CultureInfo.InvariantCulture)));
                            break;
                        case SignatureBodySourceComponents.Nonce:
                            result.AddRange(Encoding.UTF8.GetBytes(parameters.Nonce));
                            break;
                        default:
                            if (SignatureBodySourceComponents.IsHeader(component, out var headerName))
                            {
                                if (parameters.Headers.ContainsKey(headerName))
                                {
                                    result.AddRange(Encoding.UTF8.GetBytes(parameters.Headers[headerName]));
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
