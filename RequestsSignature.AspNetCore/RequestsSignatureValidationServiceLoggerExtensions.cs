using System;
using Microsoft.Extensions.Logging;

namespace RequestsSignature.AspNetCore
{
    /// <summary>
    /// Helper class that manages <see cref="ILogger"/> events for <see cref="RequestsSignatureValidationService"/>.
    /// </summary>
    internal static class RequestsSignatureValidationServiceLoggerExtensions
    {
        private const string SignatureValidationFailedMessageFormat = @"Request Signature Failed: {Status}
ServerTimestamp: {ServerTimestamp}
Signature: {SignatureValue}
ClientId: {ClientId}
ComputedSignature: {ComputedSignature}";

        private static readonly Action<ILogger, SignatureValidationResultStatus, string, Exception> _signatureValidationSucceeded =
            LoggerMessage.Define<SignatureValidationResultStatus, string>(
                LogLevel.Trace,
                new EventId(500, "SignatureValidationSucceeded"),
                @"Signature Validation Succeeded ({Status}). ClientId: {ClientId}");

        private static readonly Action<ILogger, SignatureValidationResultStatus, Exception> _signatureValidationIgnored =
            LoggerMessage.Define<SignatureValidationResultStatus>(
                LogLevel.Information,
                new EventId(501, "SignatureValidationIgnored"),
                @"Signature validation ignored: {Status}");

        private static readonly Action<ILogger, SignatureValidationResultStatus, long, string, string, string, Exception> _signatureValidationFailed =
            LoggerMessage.Define<SignatureValidationResultStatus, long, string, string, string>(
                LogLevel.Warning,
                new EventId(510, "SignatureValidationFailed"),
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
                result?.Status ?? SignatureValidationResultStatus.OK,
                result?.ClientId,
                ex);
        }

        /// <summary>
        /// Logs an ignored signature validation.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/>.</param>
        /// <param name="result">The <see cref="SignatureValidationResult"/>.</param>
        /// <param name="ex">The <see cref="Exception"/>, if any.</param>
        public static void SignatureValidationIgnored(this ILogger logger, SignatureValidationResult result, Exception ex = null)
        {
            _signatureValidationIgnored(
                logger,
                result?.Status ?? SignatureValidationResultStatus.Disabled,
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
