using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

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

        /// <summary>
        /// Gets the default <see cref="HeaderName"/> (X-RequestSignature).
        /// </summary>
        public const string DefaultHeaderName = "X-RequestSignature";

        /// <summary>
        /// Gets the default <see cref="SignaturePattern"/> ({ClientId}:{Nonce}:{Timestamp}:{Signature}).
        /// </summary>
        public static readonly Regex DefaultSignaturePattern
            = new Regex(@"^(?<ClientId>[^:]+):(?<Nonce>[^:]+):(?<Timestamp>[\d]+):(?<Signature>[^:]+)$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        /// <summary>
        /// Gets the default <see cref="ClockSkew"/> (5 minutes).
        /// </summary>
        public static readonly TimeSpan DefaultClockSkew = TimeSpan.FromMinutes(5);

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
        /// Defaults to <see cref="DefaultClockSkew"/> (5 minutes).
        /// </summary>
        public TimeSpan ClockSkew { get; set; } = DefaultClockSkew;

        /// <summary>
        /// Gets or sets the name of the header that carries out signature information.
        /// Default to <see cref="DefaultHeaderName"/> (X-RequestSignature).
        /// </summary>
        public string HeaderName { get; set; } = DefaultHeaderName;

        /// <summary>
        /// Gets or sets the overral <see cref="Regex"/> pattern for the signature.
        /// Defaults to <see cref="DefaultSignaturePattern"/> ({ClientId}:{Nonce}:{Timestamp}:{Signature}).
        /// </summary>
        public Regex SignaturePattern { get; set; } = DefaultSignaturePattern;
    }
}
