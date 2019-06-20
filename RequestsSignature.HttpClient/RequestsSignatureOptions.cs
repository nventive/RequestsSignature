using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using RequestsSignature.Core;

namespace RequestsSignature.HttpClient
{
    /// <summary>
    /// Options for signing requests.
    /// </summary>
    public class RequestsSignatureOptions
    {
        private IList<string> _signatureBodySourceComponents;

        /// <summary>
        /// Gets or sets the client id.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the key signature.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the header name.
        /// </summary>
        public string HeaderName { get; set; } = DefaultConstants.HeaderName;

        /// <summary>
        /// Gets or sets the header signature pattern.
        /// </summary>
        public string SignaturePatternBuilder { get; set; } = DefaultConstants.SignaturePatternBuilder;

        /// <summary>
        /// Gets or sets the ordered list of singature body source components used to compute
        /// the value that will be signed and create the signature body.
        /// </summary>
        [SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Allow easy binding.")]
        public IList<string> SignatureBodySourceComponents
        {
            get => _signatureBodySourceComponents ?? (_signatureBodySourceComponents = DefaultConstants.SignatureBodySourceComponents);
            set => _signatureBodySourceComponents = value;
        }
    }
}
