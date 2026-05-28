using Apps.Bitbucket.Actions;
using Apps.Bitbucket.Constants;
using Apps.Bitbucket.Models.Identifiers.Optional;
using Apps.Bitbucket.Models.Request.Repository;
using Blackbird.Applications.Sdk.Common.Invocation;
using Tests.Bitbucket.Base;

namespace Tests.Bitbucket;

[TestClass]
public class RepositoryActionTests : TestBaseMultipleConnections
{
    [TestMethod, TargetConnections(ConnectionTypes.RepoAccessToken)]
    public async Task SearchRepositories_ReturnsRepositories(InvocationContext invocationContext)
    {
        var actions = new RepositoryActions(invocationContext);
        var workspaceRequest = new OptionalWorkspaceIdentifier { WorkspaceUuid = "{c025e168-8bea-4666-8685-03f1c5f61503}" };
        var input = new SearchRepositoriesRequest { Language = "c#" };
        
        var result = await actions.SearchRepositories(workspaceRequest, input);

        PrintResult(result);
        Assert.IsNotNull(result);
        Assert.IsNotEmpty(result.Repositories, "No repositories returned");
    }

    [TestMethod, TargetConnections]
    public async Task GetRepository_ReturnsRepository(InvocationContext invocationContext)
    {
        var actions = new RepositoryActions(invocationContext);
        var workspaceRequest = new OptionalWorkspaceIdentifier { WorkspaceUuid = "{c025e168-8bea-4666-8685-03f1c5f61503}" };
        var repositoryRequest = new OptionalRepositoryIdentifier { RepositoryUuid = "{06eefb5d-2f7b-4677-add7-c308b406155d}" };
        
        var result = await actions.GetRepository(workspaceRequest, repositoryRequest);

        PrintResult(result);
        Assert.IsNotNull(result);
    }
}