# RequestsSignature

Signs and validates HTTP requests.

This projects can help you implements [HMAC](https://en.wikipedia.org/wiki/HMAC) signature to HTTP requests in .NET.

It consists of .NET Standard 2.0 assemblies to help implement:
- HMAC Signature Validation in a ASP.NET Core project (server-side)
- a HTTP Client Delegating Handler that signs requests (client-side)

Additionally, it provides a [Postman](https://www.getpostman.com/) Pre-request script to help testing APIs with signature validation.

[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](LICENSE)

## Getting Started

### Implementing Requests Signature Validation in ASP.NET Core:

Install the package:

```
Install-Package RequestsSignature.AspNetCore
```

Then in the `Startup` class adds the configuration and the services:

```csharp

using RequestsSignature.AspNetCore;

public class Startup
{
    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
        // ...

        // Configure the RequestTracingMiddlewareOptions. This is one way of doing it.
        services.Configure<RequestsSignatureOptions>(options =>
        {
            // List of client ids and secrets that are accepted and validated.
            options.Clients = new[]
            {
                new RequestsSignatureClientOptions
                {
                    ClientId = "9e616f36fde8424e9f71afa4a31e128a",
                    ClientSecret = "df46ca91155142e99617a5fc5dea1f50",
                },
            };
        });
        
        // Alternatively, options can be loaded from a configuration section.
        services.Configure<RequestsSignatureOptions>(
          _configuration.GetSection(nameof(RequestsSignatureOptions)));

        // Adds the requests signature validation services.
        services.AddRequestsSignatureValidation();
    }
}

```

Then you need to decide whether you want to implement it as a *Middleware* or as part
of *ASP.NET Core Authentication*

### Implement as a middleware

Implementing as a middleware means that you have the most control over the validation process.
A middleware intercepts the request and validates the signature, without taking
any other decision. It is then up to you to handle the result.

To implement as a middleware, us the `Configure` method in the `Startup` class:

```csharp

using RequestsSignature.AspNetCore;

public class Startup
{
    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
        // Make sure this is sufficiently early in the request pipeline.
        app.UseRequestsSignatureValidation();
        // ...
        app.UseMvc();
    }
}

```

This alone only adds the middleware, performs the validation for each incoming request,
and store the result in the `IRequestsSignatureFeature` HTTP Feature.

Results for the validation are accessible through the `HttpContext.GetSignatureValidationResult()` extension method:

```csharp
HttpContext context;
var validationResults = context.GetSignatureValidationResult();

// This is a shortcut for this:
var validationResults = context.Features.Get<IRequestsSignatureFeature>().ValidationResult;

```

#### Integration with ASP.NET Core MVC

2 additional MVC Filters are provided for convenience:

- `RequireRequestsSignatureValidationAttribute`: it is an [Action Filter](https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/filters?view=aspnetcore-2.2#action-filters) that can force the validation to be performed:

```csharp
[HttpGet("my-action")]
[RequireRequestsSignatureValidation]
public IActionResult MyAction()
{
    ...
}
```

This will throw a `RequestsSignatureValidationException` if the requests is not successfully validated. Additional option allow you to specify the list of client ids to accept.

- `IgnoreRequestsSignatureValidationAttribute`: disable the action of the previous filter.
This is useful when you implement the filter globally:

```csharp

// In Startup.cs, adds the filter globally:
services.AddMvc(options =>
{
    // Makes signature validation mandatory for all actions
    options.Filters.Add(new RequireRequestsSignatureValidationAttribute());
});

// Then on specific controller action disable it:

[HttpGet("my-action")]
[IgnoreRequestsSignatureValidation]
public IActionResult MyAction()
{
    // Signature validation is disabled here. 
    ...
}
```

This works in the same way as the `[AllowAnonymous]` filter for the authentication system.


### Implement as part of ASP.NET Core Authentication

Alternatively, you can implement requests signature validation as an ASP.NET Core Authentication scheme. This allows you to consider requests signature as a means to authenticate requests, the same way that [`AddJwtBearer`](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.jwtbearerextensions.addjwtbearer?view=aspnetcore-2.2) authenticates requests using JSON Web Tokens.

To do so, configure the authentication in the `Startup` class:

```csharp

using RequestsSignature.AspNetCore;
using RequestsSignature.AspNetCore.Authentication;

public class Startup
{
    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
        // ...

        // Configure the RequestTracingMiddlewareOptions. This is one way of doing it.
        services.Configure<RequestsSignatureOptions>(options =>
        {
            // List of client ids and secrets that are accepted and validated.
            options.Clients = new[]
            {
                new RequestsSignatureClientOptions
                {
                    ClientId = "9e616f36fde8424e9f71afa4a31e128a",
                    ClientSecret = "df46ca91155142e99617a5fc5dea1f50",
                },
            };
        });
        
        // Alternatively, options can be loaded from a configuration section.
        services.Configure<RequestsSignatureOptions>(
          _configuration.GetSection(nameof(RequestsSignatureOptions)));

        // Adds the requests signature validation services.
        services.AddRequestsSignatureValidation();


        // Configure Authentication with Requests Signature as the Default Scheme.
        services
            .AddAuthentication(RequestsSignatureAuthenticationConstants.AuthenticationScheme)
            .AddRequestsSignature();
    }
}

```

### Implementing Requests Signature in HTTP Client

To implement the creation of signed requests client-side (using `HttpClient`):

Install the package:

```
Install-Package RequestsSignature.HttpClient
```

Configure the `RequestsSignatureDelegatingHandler` with the `HttpClient` instance:

```csharp
// Create the client with the RequestsSignatureDelegatingHandler
var client = new System.Net.Http.HttpClient(
    new RequestsSignature.HttpClient.RequestsSignatureDelegatingHandler(
        new RequestsSignatureOptions
        {
            // These options must be the same as the server-side client options.
            ClientId = "9e616f36fde8424e9f71afa4a31e128a",
            ClientSecret = "df46ca91155142e99617a5fc5dea1f50",
        }));

// Use the client normally
var response = await client.GetAsync("...");
```

### Testing with [Postman](https://www.getpostman.com/)

The repository includes a [Postman Pre-request script](https://learning.getpostman.com/docs/postman/scripts/pre_request_scripts/) that can be used
to sign the requests when using [Postman](https://www.getpostman.com/).

Simply copy the content of the [`Postman.Pre-request Script.js`](Postman.Pre-request%20Script.js) file and configure the following [variables](https://learning.getpostman.com/docs/postman/environments_and_globals/variables/):

- `signatureClientId`
- `signatureClientSecret`

The outgoing requests will then be properly signed.

## Features

### Default Header signature and algorithm

By default, here is how the header is constructed:

The final header has the following specification: `{ClientId}:{Nonce}:{Timestamp}:{SignatureBody}` where:
- `{ClientId}`: is the client id as specified by the configuration
- `{Nonce}`: is a random value unique to each request (a UUID/GUID is perfectly suitable)
- `{Timestamp}`: is the current time when the request is sent, in Unix Epoch time (in seconds)
- `{SignatureBody}`: Is the Base-64 encoded value of the HMAC SHA256 Signature of the signature components

Signature components (the source for the SignatureBody HMAC value) is a binary value composed of the following values sequentially:
- Nonce: UTF-8 encoded binary values of the Nonce
- Timestamp: UTF-8 encoded binary values of the Timestamp (as a string value)
- Request method: UTF-8 encoded binary values of the **uppercase** Request method
- Request scheme: UTF-8 encoded binary values of the Request Uri scheme (e.g. `https`)
- Request host: UTF-8 encoded binary values of the Request Uri host (e.g. `example.org`)
- Request local path: UTF-8 encoded binary values of the Request Uri local path (e.g. `/api/v1/users`)
- Request query string: UTF-8 encoded binary values of the Request Query string, including the leading `?` (e.g. `?q=search`)
- Request body: Raw bytes of the request body

*For more information see the [`SignatureBodySourceBuilder`](RequestsSignature.Core/SignatureBodySourceBuilder.cs) class or the [`Postman.Pre-request Script.js`](Postman.Pre-request%20Script.js) file.*

*See the Configuration section on how to customize the signature.*

### Nonce repository

By default, nonce are not stored and checked, which means that you are vulnerable to
replay attacks for the duration of the clock skew.

To enable nonce check, you must configure a `INonceRepository` that is responsible
for storing and checking the nonce at least for the duration of twice the clock skew.

Two implementations are provided: one that relies on the [`IMemoryCache`](https://docs.microsoft.com/en-us/aspnet/core/performance/caching/memory?view=aspnetcore-2.2), and another that relies on the [`IDistributedCache`](https://docs.microsoft.com/en-us/aspnet/core/performance/caching/distributed?view=aspnetcore-2.2). Consider using one of these
(or your own implementation) to enable nonce management.

```csharp
using RequestsSignature.AspNetCore;
using RequestsSignature.AspNetCore.Nonces;

public class Startup
{
    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
        // ...

        // Enable nonce repository using the Memory Cache
        services
            .AddMemoryCache()
            .AddSingleton<INonceRepository, MemoryCacheNonceRepository>()
            .AddRequestsSignatureValidation();

        // Or using the distributed cache
        services
            .AddDistributedMemoryCache()
            .AddSingleton<INonceRepository, DistributedCacheNonceRepository>()
            .AddRequestsSignatureValidation();
    }
}
```

### Auto retry on clock skew detection (client)

The `RequestsSignatureDelegatingHandler` has a specific features that tries to detect
when a client's clock is not properly synchronized with the server and compensate
for the delta. This is useful when dealing with clients that are not under your control.

The way this work is when the client receives either a 401 or 403 status code and the 
response includes a Date header, it compares the date received from the server and the
client current time. If the difference is more than the configured `ClockSkew`, it
computes the delta, adjust the time based on the computation and automatically re-tries
the request. All subsequent invocation will also apply the same time delta, until another 
potential clock skew is detected.

This behavior can be de-activated using the `DisableAutoRetryOnClockSkew` client option.

### Configuration options

**It is important to properly synchronize the settings between the server and all the clients, otherwise the signature will be improperly computed and compared.**

#### Server-side

The following parameters can be configured client-side:

- `ClockSkew`: The duration of time that a timestamp will still be considered valid when
  comparing with the current time (+/-). Defaults to 5 minutes.
- `HeaderName`: The name of the header that contains the signature. Defaults to `X-RequestSignature`.
- `SignaturePattern`: The pattern that is used to create the final header value.
  Defaults to `{ClientId}:{Nonce}:{Timestamp}:{SignatureBody}`.
- `Disabled`: When set to true, disable the signature validation. Useful when running
  tests or local development environment.
- `SignatureBodySourceComponents`: The list of requests components that is used for signature validation. For example, to only include the Nonce, Timestamp, Host and a custom header (`X-ClientId`) for the signature body, this is how it should be configured:

```csharp
clientOptions.SignatureBodySourceComponents.Add(SignatureBodySourceComponents.Nonce);
clientOptions.SignatureBodySourceComponents.Add(SignatureBodySourceComponents.Timestamp);
clientOptions.SignatureBodySourceComponents.Add(SignatureBodySourceComponents.Host);
clientOptions.SignatureBodySourceComponents.Add(SignatureBodySourceComponents.Header("X-ClientId"));
```

#### Client-side

- `ClockSkew`: The duration of time that a timestamp will still be considered valid when
  comparing with the current time (+/-). Defaults to 5 minutes.
- `HeaderName`: The name of the header that contains the signature. Defaults to `X-RequestSignature`.
- `SignaturePattern`: The pattern that is used to create the final header value.
  Defaults to `{ClientId}:{Nonce}:{Timestamp}:{SignatureBody}`.
- `DisableAutoRetryOnClockSkew`: When set to true, the handler will not attempt to 
  detect clock skew and auto-retry.

#### Postman script

The following variables can be used to configure the Postman Pre-request script:

- `signatureClientId`: The client id
- `signatureClientSecret`: The client secret
- `signatureHeaderName`: The name of the header that contains the signature. Defaults to `X-RequestSignature`.
- `signaturePattern`: The pattern that is used to create the final header value.
  Defaults to `{ClientId}:{Nonce}:{Timestamp}:{SignatureBody}`.
- `signatureBodySourceComponents`: The requests components used to compute the signature;
  If customized, must be a JSON array of `SignatureBodySourceComponents` string values

### Further customization

It is possible to further customize the behavior of the component by providing 
custom implementation of the following interfaces:

- `ISignatureBodySourceBuilder`: Builds the source data for the signature computation
- `ISignatureBodySigner`: Creates the signature body value (from the signature body source)
- `IRequestsSignatureValidationService`: Performs the signature validation

Additionally, the Hash algorithm used can be customized by constructing the 
`HashAlgorithmSignatureBodySigner` using a custom `hashAlgorithmBuilder`:

```csharp
using System.Security.Cryptography;
using System.Text;
using RequestsSignature.AspNetCore;
using RequestsSignature.Core;

public class Startup
{
    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
        // ...
        services
            .AddSingleton<ISignatureBodySigner>(
                sp => new HashAlgorithmSignatureBodySigner(
                    parameters => new HMACSHA512(Encoding.UTF8.GetBytes(parameters.ClientSecret))))
            .AddRequestsSignatureValidation();
    }
}
```

### Diagnose problems

The `RequestsSignatureValidationService` provides extensive logging capabilities
to try to diagnose signature errors.

Enable the logging (with a minimum log level of `Warning`) to see diagnostic information:

```json
{
  "Logging": {
    "LogLevel": {
      "RequestsSignature": "Warning"
    }
  }
}
```

Here is the list of events that are logged:

| Event Id | Event Name                   | Description                                               |
|----------|------------------------------|-----------------------------------------------------------|
| 500      | SignatureValidationSucceeded | When a signature is successfully validated                |
| 501      | SignatureValidationIgnored   | When signature validation is disabled (via configuration) |
| 510      | SignatureValidationFailed    | When signature validation fails                           |

The logged properties and the `SignatureValidationResult` provides a detailed
account as to what exactly failed the validation step, including intermediary
signature body source and expected signature value.

## Changelog

Please consult the [CHANGELOG](CHANGELOG.md) for more information about version
history.

## License

This project is licensed under the Apache 2.0 license - see the
[LICENSE](LICENSE) file for details.

## Contributing

Please read [CONTRIBUTING.md](CONTRIBUTING.md) for details on the process for
contributing to this project.

Be mindful of our [Code of Conduct](CODE_OF_CONDUCT.md).
