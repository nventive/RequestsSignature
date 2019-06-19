namespace RequestsSignature.AspNetCore
{
    /// <summary>
    /// The status for a <see cref="SignatureValidationResult"/>.
    /// </summary>
    public enum SignatureValidationResultStatus
    {
        /// <summary>
        /// The signature validation is disabled.
        /// </summary>
        Disabled,

        /// <summary>
        /// The signature is valid.
        /// </summary>
        OK,

        /// <summary>
        /// No header was present in the request.
        /// </summary>
        HeaderNotFound,

        /// <summary>
        /// The header could not be parsed according to the <see cref="RequestsSignatureOptions.SignaturePattern"/>.
        /// </summary>
        HeaderParseError,

        /// <summary>
        /// The client id does not exist in the <see cref="RequestsSignatureOptions.Clients"/> collection.
        /// </summary>
        ClientIdNotFound,

        /// <summary>
        /// The nonce has already been used before. This might indicate a replay attack.
        /// </summary>
        NonceHasBeenUsedBefore,

        /// <summary>
        /// The timestamp is too far away from the current system time (outside the range of <see cref="RequestsSignatureOptions.ClockSkew"/>).
        /// </summary>
        TimestampIsOff,

        /// <summary>
        /// The computed signature does not match the request signature.
        /// </summary>
        SignatureDoesntMatch,

        /// <summary>
        /// Unknown error internal to the component.
        /// </summary>
        InternalError,
    }
}
