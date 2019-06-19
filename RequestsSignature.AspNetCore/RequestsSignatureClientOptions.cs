using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using SC = RequestsSignature.AspNetCore.SignatureBodySourceComponents;

namespace RequestsSignature.AspNetCore
{
    /// <summary>
    /// Signature options for a specific client.
    /// </summary>
    public class RequestsSignatureClientOptions
    {
        /// <summary>
        /// Gets the default <see cref="SignatureBodySourceComponents"/>.
        /// </summary>
        public static readonly IList<string> DefaultSignatureBodySourceComponents = new List<string>
        { SC.Method, SC.Scheme, SC.Host, SC.Path, SC.QueryString, SC.Body };

        private IList<string> _signatureBodySourceComponents;

        /// <summary>
        /// Gets or sets the client id.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the signature key.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the ordered list of singature body source components used to compute
        /// the value that will be signed and create the signature body.
        /// </summary>
        [SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Allow easy binding.")]
        public IList<string> SignatureBodySourceComponents
        {
            get => _signatureBodySourceComponents ?? (_signatureBodySourceComponents = DefaultSignatureBodySourceComponents);
            set => _signatureBodySourceComponents = value;
        }
    }
}
