using Apps.Bitbucket.Actions;
using Apps.Bitbucket.Models.Identifiers;
using Apps.Bitbucket.Models.Identifiers.Optional;
using Apps.Bitbucket.Models.Request.PullRequest;
using Blackbird.Applications.Sdk.Common.Invocation;
using Tests.Bitbucket.Base;

namespace Tests.Bitbucket;

[TestClass]
public class PullRequestActionTests : TestBaseMultipleConnections
{
    [TestMethod, TargetConnections]
    public async Task CreatePullRequest_ReturnsCreatedPullRequest(InvocationContext invocationContext)
    {
        var actions = new PullRequestActions(invocationContext);
        var workspaceRequest = new OptionalWorkspaceIdentifier { WorkspaceUuid = "{c025e168-8bea-4666-8685-03f1c5f61503}" };
        var repositoryRequest = new OptionalRepositoryIdentifier { RepositoryUuid = "{06eefb5d-2f7b-4677-add7-c308b406155d}" };
        var input = new CreatePullRequestRequest
        {
            Title = "test from bird",
            Description = "12345",
            SourceBranchName = "dev",
            TargetBranchName = "main"
        };

        var result = await actions.CreatePullRequest(workspaceRequest, repositoryRequest, input);

        PrintResult(result);
        Assert.IsNotNull(result);
    }

    [TestMethod, TargetConnections]
    public async Task MergePullRequest_ReturnsMergedPullRequest(InvocationContext invocationContext)
    {
        var actions = new PullRequestActions(invocationContext);
        var workspaceRequest = new OptionalWorkspaceIdentifier { WorkspaceUuid = "{c025e168-8bea-4666-8685-03f1c5f61503}" };
        var repositoryRequest = new OptionalRepositoryIdentifier { RepositoryUuid = "{06eefb5d-2f7b-4677-add7-c308b406155d}" };
        var pullRequestRequest = new PullRequestIdentifier { PullRequestId = "3" };
        var input = new MergePullRequestRequest
        {
            CommitMessage = "test from tests"
        };

        var result = await actions.MergePullRequest(workspaceRequest, repositoryRequest, pullRequestRequest, input);

        PrintResult(result);
        Assert.IsNotNull(result);
    }
}