using System;
using Microsoft.Extensions.Options;
using NJsonSchema;
using NSwag;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace RequestsSignature.AspNetCore.NSwag
{
    /// <summary>
    /// <see cref="IOperationProcessor"/> that adds the request signature header.
    /// </summary>
    public class RequestsSignatureOperationProcessor : IOperationProcessor
    {
        private readonly IOptions<RequestsSignatureOptions> _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestsSignatureOperationProcessor"/> class.
        /// </summary>
        /// <param name="options">The <see cref="RequestsSignatureOptions"/>.</param>
        public RequestsSignatureOperationProcessor(IOptions<RequestsSignatureOptions> options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        /// <inheritdoc />
        public bool Process(OperationProcessorContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var parameter = new OpenApiParameter
            {
                Name = _options.Value.HeaderName,
                Kind = OpenApiParameterKind.Header,
                Description = $"Request signature.",
                Schema = new JsonSchema
                {
                    Type = JsonObjectType.String,
                    Pattern = _options.Value.SignaturePattern.ToString(),
                },
            };
            context.OperationDescription.Operation.Parameters.Add(parameter);

            return true;
        }
    }
}
