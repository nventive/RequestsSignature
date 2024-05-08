using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using RequestsSignature.AspNetCore;
using RequestsSignature.AspNetCore.Nonces;
using RequestsSignature.Core;

namespace RequestsSignature.HttpClient.Tests.Server
{
    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Default ASP.Net Core startup class.")]
    public class StartupWithMiddleware
    {
        public const string DefaultClientId = "0ebb2f21169b4c82b1915f0559212a3a";
        public const string DefaultClientSecret = "4b2a708910f74275a4ba46aeb3af346b";

        public const string CustomClientId = "a91946e5329e40678becbdc7b86564ed";
        public const string CustomClientSecret = "020ee7d9657e4d389e21d05bdd619e52";

        public static readonly IList<string> CustomSignatureBodySourceComponents = new List<string>
            { SignatureBodySourceComponents.LocalPath, SignatureBodySourceComponents.Timestamp };

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.Configure<AspNetCore.RequestsSignatureOptions>(options =>
            {
                options.Clients.Add(new RequestsSignatureClientOptions
                {
                    ClientId = DefaultClientId,
                    ClientSecret = DefaultClientSecret,
                });
                var customClient = new RequestsSignatureClientOptions
                {
                    ClientId = CustomClientId,
                    ClientSecret = CustomClientSecret,
                };
                foreach (var componentSource in CustomSignatureBodySourceComponents)
                {
                    customClient.SignatureBodySourceComponents.Add(componentSource);
                }

                options.Clients.Add(customClient);
            });
            services.AddMemoryCache();
            services.AddSingleton<INonceRepository, MemoryCacheNonceRepository>();
            services.AddRequestsSignatureValidation();

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRequestsSignatureValidation();
            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
