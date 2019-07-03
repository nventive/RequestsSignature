using System;
using System.Collections.Generic;
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

        /// <summary>
        /// Gets or sets a value indicating whether the requests signature validation is disabled.
        /// </summary>
        public bool Disabled { get; set; } = DefaultDisabled;

        /// <summary>
        /// Gets individual <see cref="RequestsSignatureClientOptions"/>.
        /// </summary>
        public IList<RequestsSignatureClientOptions> Clients { get; } = new List<RequestsSignatureClientOptions>();

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
        /// Gets or sets the header signature pattern,
        /// Defaults to <see cref="DefaultConstants.SignaturePattern"/> ({ClientId}:{Nonce}:{Timestamp}:{SignatureBody}).
        /// </summary>
        public string SignaturePattern { get; set; } = DefaultConstants.SignaturePattern;
    }
}
