﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using RequestsSignature.AspNetCore.Authentication;
using RequestsSignature.Core;

namespace RequestsSignature.HttpClient.Tests.Server
{
    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Default ASP.Net Core startup class.")]
    public class StartupWithAuthentication
    {
        public const string DefaultClientId = "0ebb2f21169b4c82b1915f0559212a3a";
        public const string DefaultKey = "4b2a708910f74275a4ba46aeb3af346b";

        public const string CustomClientId = "a91946e5329e40678becbdc7b86564ed";
        public const string CustomKey = "020ee7d9657e4d389e21d05bdd619e52";

        public static readonly IList<string> CustomSignatureBodySourceComponents = new List<string>
            { SignatureBodySourceComponents.LocalPath, SignatureBodySourceComponents.Timestamp };

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.Configure<AspNetCore.RequestsSignatureOptions>(options =>
            {
                options.Clients = new[]
                {
                    new AspNetCore.RequestsSignatureClientOptions
                    {
                        ClientId = DefaultClientId,
                        Key = DefaultKey,
                    },
                    new AspNetCore.RequestsSignatureClientOptions
                    {
                        ClientId = CustomClientId,
                        Key = CustomKey,
                        SignatureBodySourceComponents = CustomSignatureBodySourceComponents,
                    },
                };
            });
            services.AddRequestsSignatureValidation();
            services.AddMvc();

            services
                .AddAuthentication(RequestsSignatureAuthenticationConstants.AuthenticationScheme)
                .AddRequestsSignature();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}