using Apps.Bitbucket.Handlers;
using Apps.Bitbucket.Models.Identifiers.Optional;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Tests.Bitbucket.Base;

namespace Tests.Bitbucket;

[TestClass]
public class AuthenticatorTests : TestBase
{
    // No token caching: 1.8 sec
    // With token caching: 1.3-1.4 sec
    [TestMethod]
    public async Task OAuthAuthenticator_WithTwoSubsequentRequests_IsSuccess()
    {
        // Arrange
        var workspaceIdentifier = new OptionalWorkspaceIdentifier
        {
            WorkspaceUuid = "{c025e168-8bea-4666-8685-03f1c5f61503}"
        };
        var handler = new UserDataHandler(InvocationContext, workspaceIdentifier);

        // Act
        await handler.GetDataAsync(new DataSourceContext(), CancellationToken.None);
        await handler.GetDataAsync(new DataSourceContext(), CancellationToken.None);
    }
}