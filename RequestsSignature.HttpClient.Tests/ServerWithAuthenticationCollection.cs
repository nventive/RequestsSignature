using RequestsSignature.HttpClient.Tests.Server;
using Xunit;

namespace RequestsSignature.HttpClient.Tests
{
    [CollectionDefinition(Name)]
    public class ServerWithAuthenticationCollection : ICollectionFixture<ServerFixture<StartupWithAuthentication>>
    {
        public const string Name = nameof(ServerWithAuthenticationCollection);
    }
}
