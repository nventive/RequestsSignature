using System;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace RequestsSignature.HttpClient.Tests
{
    /// <summary>
    /// xUnit collection fixture that starts an ASP.NET Core server listening to a random port.
    /// <seealso cref="ServerWithMiddlewareCollection" />.
    /// </summary>
    /// <typeparam name="TStartup">The ASP.NET Core Startup class.</typeparam>
    public class ServerFixture<TStartup> : IDisposable
        where TStartup : class
    {
        public ServerFixture()
        {
#if NETCOREAPP2_2
            ServerWebHost = WebHost
                .CreateDefaultBuilder()
                .UseStartup<TStartup>()
                .UseUrls("http://127.0.0.1:0")
                .Build();
#endif
#if NETCOREAPP3_0
            ServerWebHost = Host
                .CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<TStartup>();
                    webBuilder.UseUrls("http://127.0.0.1:0");
                })
                .Build();
#endif
            ServerWebHost.Start();
        }

#if NETCOREAPP2_2
        public IWebHost ServerWebHost { get; }
#endif
#if NETCOREAPP3_0
        public IHost ServerWebHost { get; }
#endif

        public Uri ServerUri
        {
            get
            {
#if NETCOREAPP2_2

                var serverAddressesFeature = ServerWebHost.ServerFeatures.Get<IServerAddressesFeature>();
#endif
#if NETCOREAPP3_0
                var serverAddressesFeature = ServerWebHost
                    .Services.GetRequiredService<Microsoft.AspNetCore.Hosting.Server.IServer>()
                    .Features.Get<IServerAddressesFeature>();
#endif
                return new Uri(serverAddressesFeature.Addresses.First());
            }
        }

        public void Dispose()
        {
            if (ServerWebHost != null)
            {
                ServerWebHost.Dispose();
            }
        }
    }
}
