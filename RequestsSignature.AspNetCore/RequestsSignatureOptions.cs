using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using RequestsSignature.Core;

namespace RequestsSignature.AspNetCore
{
    /// <summary>
    /// Options for requests signatures validation.
    /// </summary>
    public class RequestsSignatureOptions
    {
        /// <summary>
        /// Gets the default <see cref="Disabled"/> value (false).
        /// </summary>
        public const bool DefaultDisabled = false;

        private IEnumerable<RequestsSignatureClientOptions> _clients;

        /// <summary>
        /// Gets or sets a value indicating whether the requests signature validation is disabled.
        /// </summary>
        public bool Disabled { get; set; } = DefaultDisabled;

        /// <summary>
        /// Gets or sets individual <see cref="RequestsSignatureClientOptions"/>.
        /// </summary>
        public IEnumerable<RequestsSignatureClientOptions> Clients
        {
            get => _clients ?? (_clients = Enumerable.Empty<RequestsSignatureClientOptions>());
            set => _clients = value;
        }

        /// <summary>
        /// Gets or sets the allowed lag of time in either direction (past/future)
        /// where the request is still configured valid.
        /// Defaults to <see cref="DefaultConstants.ClockSkew"/> (5 minutes).
        /// </summary>
        public TimeSpan ClockSkew { get; set; } = DefaultConstants.ClockSkew;

        /// <summary>
        /// Gets or sets the name of the header that carries out signature information.
        /// Default to <see cref="DefaultConstants.HeaderName"/> (X-RequestSignature).
        /// </summary>
        public string HeaderName { get; set; } = DefaultConstants.HeaderName;

        /// <summary>
        /// Gets or sets the overral <see cref="Regex"/> pattern for the signature.
        /// Defaults to <see cref="DefaultConstants.SignaturePatternParser"/> ({ClientId}:{Nonce}:{Timestamp}:{SignatureBody}).
        /// </summary>
        public Regex SignaturePattern { get; set; } = DefaultConstants.SignaturePatternParser;
    }
}
