using System;
using System.Linq;
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
            ServerWebHost = Host
                .CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<TStartup>();
                    webBuilder.UseUrls("http://127.0.0.1:0");
                })
                .Build();

            ServerWebHost.Start();
        }

        public IHost ServerWebHost { get; }

        public Uri ServerUri
        {
            get
            {
                var serverAddressesFeature = ServerWebHost
                    .Services.GetRequiredService<Microsoft.AspNetCore.Hosting.Server.IServer>()
                    .Features.Get<IServerAddressesFeature>();

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
