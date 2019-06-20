using System;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using RequestsSignature.HttpClient.Tests.Server;

namespace RequestsSignature.HttpClient.Tests
{
    /// <summary>
    /// xUnit collection fixture that starts an ASP.NET Core server listening to a random port.
    /// <seealso cref="ServerWithMiddlewareCollection" />.
    /// </summary>
    public class ServerFixtureWithMiddleware : IDisposable
    {
        public ServerFixtureWithMiddleware()
        {
            ServerWebHost = WebHost
                .CreateDefaultBuilder()
                .UseStartup<StartupWithMiddleware>()
                .UseUrls("http://127.0.0.1:0")
                .Build();
            ServerWebHost.Start();
        }

        public IWebHost ServerWebHost { get; }

        public Uri ServerUri
        {
            get
            {
                var serverAddressesFeature = ServerWebHost.ServerFeatures.Get<IServerAddressesFeature>();
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
