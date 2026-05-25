using Apps.Bitbucket.Handlers;
using Apps.Bitbucket.Handlers.FileFolder;
using Apps.Bitbucket.Models.Identifiers;
using Apps.Bitbucket.Models.Identifiers.Optional;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Tests.Bitbucket.Base;

namespace Tests.Bitbucket;

[TestClass]
public class HandlerTests : TestBaseMultipleConnections
{
    [TestMethod, TargetConnections]
    public async Task WorkspaceDataHandler_ReturnsWorkspacesForCurrentUser(InvocationContext invocationContext)
    {
        var handler = new WorkspaceDataHandler(invocationContext);

        var result = await handler.GetDataAsync(new DataSourceContext { SearchString = "ab" }, CancellationToken.None);

        PrintDataHandlerResult(result);
        Assert.IsNotNull(result);
    }

    [TestMethod, TargetConnections]
    public async Task UserDataHandler_ReturnsUsers(InvocationContext invocationContext)
    {
        var workspaceId = new OptionalWorkspaceIdentifier { WorkspaceUuid = "{c025e168-8bea-4666-8685-03f1c5f61503}" };
        var handler = new UserDataHandler(invocationContext, workspaceId);

        var result = await handler.GetDataAsync(new DataSourceContext { SearchString = "an" }, CancellationToken.None);

        PrintDataHandlerResult(result);
        Assert.IsNotNull(result);
    }

    [TestMethod, TargetConnections]
    public async Task RepositoryDataHandler_ReturnsRepositories(InvocationContext invocationContext)
    {
        var workspaceId = new WorkspaceIdentifier { WorkspaceUuid = "{c025e168-8bea-4666-8685-03f1c5f61503}" };
        var handler = new RepositoryDataHandler(invocationContext, workspaceId);

        var result = await handler.GetDataAsync(new DataSourceContext { }, CancellationToken.None);

        PrintDataHandlerResult(result);
        Assert.IsNotNull(result);
    }

    [TestMethod, TargetConnections]
    public async Task BranchDataHandler_ReturnsBranches(InvocationContext invocationContext)
    {
        var workspaceId = new WorkspaceIdentifier { WorkspaceUuid = "{c025e168-8bea-4666-8685-03f1c5f61503}" };
        var repositoryId = new RepositoryIdentifier { RepositoryUuid = "{06eefb5d-2f7b-4677-add7-c308b406155d}" };
        var handler = new BranchDataHandler(invocationContext, workspaceId, repositoryId);

        var result = await handler.GetDataAsync(new DataSourceContext { }, CancellationToken.None);

        PrintDataHandlerResult(result);
        Assert.IsNotNull(result);
    }

    [TestMethod, TargetConnections]
    public async Task PullRequestDataHandler_ReturnsPullRequests(InvocationContext invocationContext)
    {
        var workspaceId = new WorkspaceIdentifier { WorkspaceUuid = "{c025e168-8bea-4666-8685-03f1c5f61503}" };
        var repositoryId = new RepositoryIdentifier { RepositoryUuid = "{06eefb5d-2f7b-4677-add7-c308b406155d}" };
        var handler = new PullRequestDataHandler(invocationContext, workspaceId, repositoryId);

        var result = await handler.GetDataAsync(new DataSourceContext { }, CancellationToken.None);

        PrintDataHandlerResult(result);
        Assert.IsNotNull(result);
    }

    [TestMethod, TargetConnections]
    public async Task FileDataHandler_ReturnsFiles(InvocationContext invocationContext)
    {
        var workspaceId = new WorkspaceIdentifier { WorkspaceUuid = "{c025e168-8bea-4666-8685-03f1c5f61503}" };
        var repositoryId = new RepositoryIdentifier { RepositoryUuid = "{06eefb5d-2f7b-4677-add7-c308b406155d}" };
        var branchId = new OptionalBranchIdentifier { BranchName = "dev" };
        var handler = new FilePickerDataHandler(invocationContext, workspaceId, repositoryId, branchId);

        var result = await handler.GetFolderContentAsync(new() { FolderId = "" }, CancellationToken.None);

        foreach (var fileDataItem in result)
            TestContext?.WriteLine($"{fileDataItem.Id} - {fileDataItem.DisplayName}");
        Assert.IsNotNull(result);
    }
}