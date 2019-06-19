using System;
using Microsoft.Extensions.Logging;

namespace RequestsSignature.AspNetCore
{
    /// <summary>
    /// Helper class that manages <see cref="ILogger"/> events for <see cref="RequestsSignatureValidationService"/>.
    /// </summary>
    public static class RequestsSignatureValidationServiceLoggerExtensions
    {
        private const string SignatureValidationFailedMessageFormat = @"Request Signature Failed: {Status}
ServerTimestamp: {ServerTimestamp}
Signature: {SignatureValue}
ClientId: {ClientId}
ComputedSignature: {ComputedSignature}";

        private static readonly Action<ILogger, string, Exception> _signatureValidationSucceeded =
            LoggerMessage.Define<string>(
                LogLevel.Trace,
                new EventId(500, "SignatureValidationSucceeded"),
                @"Signature Validation Succeeded. ClientId: {ClientId}");

        private static readonly Action<ILogger, SignatureValidationResultStatus, long, string, string, string, Exception> _signatureValidationFailed =
            LoggerMessage.Define<SignatureValidationResultStatus, long, string, string, string>(
                LogLevel.Error,
                new EventId(501, "SignatureValidationFailed"),
                SignatureValidationFailedMessageFormat);

        /// <summary>
        /// Logs a successful signature validation.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/>.</param>
        /// <param name="result">The <see cref="SignatureValidationResult"/>.</param>
        /// <param name="ex">The <see cref="Exception"/>, if any.</param>
        public static void SignatureValidationSucceeded(this ILogger logger, SignatureValidationResult result, Exception ex = null)
        {
            _signatureValidationSucceeded(
                logger,
                result?.ClientId,
                ex);
        }

        /// <summary>
        /// Logs a failed signature validation.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/>.</param>
        /// <param name="result">The <see cref="SignatureValidationResult"/>.</param>
        /// <param name="ex">The <see cref="Exception"/>, if any.</param>
        public static void SignatureValidationFailed(this ILogger logger, SignatureValidationResult result, Exception ex = null)
        {
            _signatureValidationFailed(
                logger,
                result?.Status ?? SignatureValidationResultStatus.InternalError,
                result?.ServerTimestamp ?? 0,
                result?.SignatureValue,
                result?.ClientId,
                result?.ComputedSignature,
                ex);
        }
    }
}
