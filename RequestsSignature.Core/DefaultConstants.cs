using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SC = RequestsSignature.Core.SignatureBodySourceComponents;

namespace RequestsSignature.Core
{
    /// <summary>
    /// Holds common constants.
    /// </summary>
    public static class DefaultConstants
    {
        /// <summary>
        /// Gets the default header name (X-RequestSignature).
        /// </summary>
        public const string HeaderName = "X-RequestSignature";

        /// <summary>
        /// Gets the default pattern for the signature header ({ClientId}:{Nonce}:{Timestamp}:{SignatureBody}).
        /// </summary>
        public const string SignaturePattern = "{ClientId}:{Nonce}:{Timestamp}:{SignatureBody}";

        /// <summary>
        /// Gets the default source components for signature body.
        /// </summary>
        public static readonly IList<string> SignatureBodySourceComponents = new List<string>
        { SC.Nonce, SC.Timestamp, SC.Method, SC.Scheme, SC.Host, SC.LocalPath, SC.QueryString, SC.Body };

        /// <summary>
        /// Gets the default clock skew (5 minutes).
        /// </summary>
        public static readonly TimeSpan ClockSkew = TimeSpan.FromMinutes(5);
    }
}
