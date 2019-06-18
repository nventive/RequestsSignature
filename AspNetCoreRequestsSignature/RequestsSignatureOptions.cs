﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace AspNetCoreRequestsSignature
{
    /// <summary>
    /// Options for requests signatures validation.
    /// </summary>
    public class RequestsSignatureOptions
    {
        /// <summary>
        /// Gets the default <see cref="HeaderName"/> (X-RequestSignature).
        /// </summary>
        public const string DefaultHeaderName = "X-RequestSignature";

        /// <summary>
        /// Gets the default <see cref="SignaturePattern"/> ({ClientId}:{Nonce}:{Timestamp}:{Signature}).
        /// </summary>
        public const string DefaultSignaturePattern =
            PatternTags.ClientIdTag + ":" + PatternTags.NonceTag + ":" + PatternTags.TimestampTag + ":" + PatternTags.SignatureTag;

        /// <summary>
        /// Gets the default <see cref="ClockSkew"/> (5 minutes).
        /// </summary>
        public static readonly TimeSpan DefaultClockSkew = TimeSpan.FromMinutes(5);

        private IEnumerable<RequestsSignatureClientOptions> _clients;

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
        /// Gets or sets the overral pattern for the signature.
        /// Defaults to <see cref="DefaultSignaturePattern"/> ({ClientId}:{Nonce}:{Timestamp}:{Signature}).
        /// </summary>
        public string SignaturePattern { get; set; } = DefaultSignaturePattern;
    }
}
