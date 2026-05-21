using Apps.Bitbucket.Handlers;
using Apps.Bitbucket.Models.Identifiers;
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
        var result = await handler.GetDataAsync(new DataSourceContext { SearchString = "ab" }, CancellationToken.None);

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
        var result = await handler.GetDataAsync(new DataSourceContext { SearchString = "an" }, CancellationToken.None);

        // Assert
        PrintDataHandlerResult(result);
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task RepositoryDataHandler_ReturnsRepositories()
    {
        // Arrange
        var workspaceId = new WorkspaceIdentifier { WorkspaceUuid = "{c025e168-8bea-4666-8685-03f1c5f61503}" };
        var handler = new RepositoryDataHandler(InvocationContext, workspaceId);

        // Act
        var result = await handler.GetDataAsync(new DataSourceContext { }, CancellationToken.None);

        // Assert
        PrintDataHandlerResult(result);
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task BranchDataHandler_ReturnsBranches()
    {
        // Arrange
        var workspaceId = new WorkspaceIdentifier { WorkspaceUuid = "{c025e168-8bea-4666-8685-03f1c5f61503}" };
        var repositoryId = new RepositoryIdentifier { RepositoryUuid = "{06eefb5d-2f7b-4677-add7-c308b406155d}" };
        var handler = new BranchDataHandler(InvocationContext, workspaceId, repositoryId);

        // Act
        var result = await handler.GetDataAsync(new DataSourceContext { }, CancellationToken.None);

        // Assert
        PrintDataHandlerResult(result);
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task PullRequestDataHandler_ReturnsPullRequests()
    {
        // Arrange
        var workspaceId = new WorkspaceIdentifier { WorkspaceUuid = "{c025e168-8bea-4666-8685-03f1c5f61503}" };
        var repositoryId = new RepositoryIdentifier { RepositoryUuid = "{06eefb5d-2f7b-4677-add7-c308b406155d}" };
        var handler = new PullRequestDataHandler(InvocationContext, workspaceId, repositoryId);

        // Act
        var result = await handler.GetDataAsync(new DataSourceContext { }, CancellationToken.None);

        // Assert
        PrintDataHandlerResult(result);
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task FileDataHandler_ReturnsFiles()
    {
        // Arrange
        var workspaceId = new WorkspaceIdentifier { WorkspaceUuid = "{c025e168-8bea-4666-8685-03f1c5f61503}" };
        var repositoryId = new RepositoryIdentifier { RepositoryUuid = "{06eefb5d-2f7b-4677-add7-c308b406155d}" };
        var branchId = new OptionalBranchIdentifier { BranchName = "dev" };
        var handler = new FileDataHandler(InvocationContext, workspaceId, repositoryId, branchId);

        // Act
        var result = await handler.GetDataAsync(new DataSourceContext { }, CancellationToken.None);

        // Assert
        PrintDataHandlerResult(result);
        Assert.IsNotNull(result);
    }
}
