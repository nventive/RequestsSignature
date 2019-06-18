namespace AspNetCoreRequestsSignature
{
    /// <summary>
    /// Helper class for string patterns substitutions.
    /// </summary>
    public static class PatternTags
    {
        /// <summary>
        /// Gets the pattern tag for client id.
        /// </summary>
        public const string ClientIdTag = "{ClientId}";

        /// <summary>
        /// Gets the pattern tag for the nonce.
        /// </summary>
        public const string NonceTag = "{Nonce}";

        /// <summary>
        /// Gets the pattern tag for the timestamp.
        /// </summary>
        public const string TimestampTag = "{Timestamp}";

        /// <summary>
        /// Gets the pattern tag for the signature.
        /// </summary>
        public const string SignatureTag = "{Signature}";
    }
}
