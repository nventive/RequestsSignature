using Xunit;

namespace RequestsSignature.HttpClient.Tests
{
    [CollectionDefinition(Name)]
    public class ServerWithMiddlewareCollection : ICollectionFixture<ServerFixtureWithMiddleware>
    {
        public const string Name = nameof(ServerWithMiddlewareCollection);
    }
}
