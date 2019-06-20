using RequestsSignature.HttpClient.Tests.Server;
using Xunit;

namespace RequestsSignature.HttpClient.Tests
{
    [CollectionDefinition(Name)]
    public class ServerWithMiddlewareCollection : ICollectionFixture<ServerFixture<StartupWithMiddleware>>
    {
        public const string Name = nameof(ServerWithMiddlewareCollection);
    }
}
