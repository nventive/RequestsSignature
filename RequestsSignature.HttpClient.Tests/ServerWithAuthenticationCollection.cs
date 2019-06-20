using Xunit;

namespace RequestsSignature.HttpClient.Tests
{
    [CollectionDefinition(Name)]
    public class ServerWithAuthenticationCollection : ICollectionFixture<ServerFixtureWithAuthentication>
    {
        public const string Name = nameof(ServerWithAuthenticationCollection);
    }
}
