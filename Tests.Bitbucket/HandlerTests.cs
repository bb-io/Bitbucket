using Apps.Bitbucket.Handlers;
using Apps.Bitbucket.Models.Identifiers.Optional;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Tests.Bitbucket.Base;

namespace Tests.Bitbucket;

[TestClass]
public class HandlerTests : TestBase
{
    [TestMethod]
    public async Task WorkspaceDataHandler_ReturnsWorkspacesForCurrentUser()
    {
        // Arrange
        var handler = new WorkspaceDataHandler(InvocationContext);

        // Act
        var result = await handler.GetDataAsync(new DataSourceContext { }, CancellationToken.None);

        // Assert
        PrintDataHandlerResult(result);
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task UserDataHandler_ReturnsUsers()
    {
        // Arrange
        var workspaceId = new OptionalWorkspaceIdentifier { WorkspaceUuid = "{c025e168-8bea-4666-8685-03f1c5f61503}" };
        var handler = new UserDataHandler(InvocationContext, workspaceId);

        // Act
        var result = await handler.GetDataAsync(new DataSourceContext { }, CancellationToken.None);

        // Assert
        PrintDataHandlerResult(result);
        Assert.IsNotNull(result);
    }
}
