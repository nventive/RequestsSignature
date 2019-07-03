using System;
using System.Collections.Generic;
using RequestsSignature.Core;

namespace RequestsSignature.HttpClient
{
    /// <summary>
    /// Options for signing requests.
    /// </summary>
    public class RequestsSignatureOptions
    {
        /// <summary>
        /// Gets or sets the client id.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the client secret.
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// Gets or sets the header name.
        /// </summary>
        public string HeaderName { get; set; } = DefaultConstants.HeaderName;

        /// <summary>
        /// Gets or sets the header signature pattern.
        /// </summary>
        public string SignaturePattern { get; set; } = DefaultConstants.SignaturePattern;

        /// <summary>
        /// Gets or sets the allowed lag of time in either direction (past/future)
        /// where the request is still considered valid.
        /// Defaults to <see cref="DefaultConstants.ClockSkew"/> (5 minutes).
        /// </summary>
        public TimeSpan ClockSkew { get; set; } = DefaultConstants.ClockSkew;

        /// <summary>
        /// Gets or sets a value indicating whether to disable auto-retries on clock skew detection.
        /// </summary>
        public bool DisableAutoRetryOnClockSkew { get; set; } = false;

        /// <summary>
        /// Gets the ordered list of signature body source components used to compute
        /// the value that will be signed and create the signature body.
        /// </summary>
        public IList<string> SignatureBodySourceComponents { get; } = new List<string>();
    }
}
