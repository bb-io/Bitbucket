using Apps.Bitbucket.Actions;
using Apps.Bitbucket.Models.Identifiers;
using Apps.Bitbucket.Models.Request.PullRequest;
using Tests.Bitbucket.Base;

namespace Tests.Bitbucket;

[TestClass]
public class PullRequestActionTests : TestBase
{
    [TestMethod]
    public async Task CreatePullRequest_ReturnsCreatedPullRequest()
    {
        // Arrange
        var actions = new PullRequestActions(InvocationContext);
        var workspaceRequest = new WorkspaceIdentifier { WorkspaceUuid = "{c025e168-8bea-4666-8685-03f1c5f61503}" };
        var repositoryRequest = new RepositoryIdentifier { RepositoryUuid = "{06eefb5d-2f7b-4677-add7-c308b406155d}" };
        var input = new CreatePullRequestRequest
        {
            Title = "test from bird",
            Description = "12345",
            SourceBranchName = "dev",
            TargetBranchName = "main"
        };

        // Act
        var result = await actions.CreatePullRequest(workspaceRequest, repositoryRequest, input);

        // Assert
        PrintResult(result);
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task MergePullRequest_ReturnsMergedPullRequest()
    {
        // Arrange
        var actions = new PullRequestActions(InvocationContext);
        var workspaceRequest = new WorkspaceIdentifier { WorkspaceUuid = "{c025e168-8bea-4666-8685-03f1c5f61503}" };
        var repositoryRequest = new RepositoryIdentifier { RepositoryUuid = "{06eefb5d-2f7b-4677-add7-c308b406155d}" };
        var pullRequestRequest = new PullRequestIdentifier { PullRequestId = "3" };
        var input = new MergePullRequestRequest
        {
            CommitMessage = "test from tests"
        };

        // Act
        var result = await actions.MergePullRequest(workspaceRequest, repositoryRequest, pullRequestRequest, input);

        // Assert
        PrintResult(result);
        Assert.IsNotNull(result);
    }
}