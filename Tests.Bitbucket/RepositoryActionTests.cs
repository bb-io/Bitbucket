using Apps.Bitbucket.Actions;
using Apps.Bitbucket.Models.Identifiers;
using Apps.Bitbucket.Models.Request.Repository;
using Tests.Bitbucket.Base;

namespace Tests.Bitbucket;

[TestClass]
public class RepositoryActionTests : TestBase
{
    [TestMethod]
    public async Task SearchRepositories_ReturnsRepositories()
    {
        // Arrange
        var actions = new RepositoryActions(InvocationContext);
        var workspaceRequest = new WorkspaceIdentifier { WorkspaceUuid = "{c025e168-8bea-4666-8685-03f1c5f61503}" };
        var input = new SearchRepositoriesRequest { Language = "c#" };
        
        // Act
        var result = await actions.SearchRepositories(workspaceRequest, input);

        // Assert
        PrintResult(result);
        Assert.IsNotNull(result);
        Assert.IsNotEmpty(result.Repositories, "No repositories returned");
    }

    [TestMethod]
    public async Task GetRepository_ReturnsRepository()
    {
        // Arrange
        var actions = new RepositoryActions(InvocationContext);
        var workspaceRequest = new WorkspaceIdentifier { WorkspaceUuid = "{c025e168-8bea-4666-8685-03f1c5f61503}" };
        var repositoryRequest = new RepositoryIdentifier { RepositoryUuid = "{06eefb5d-2f7b-4677-add7-c308b406155d}" };
        
        // Act
        var result = await actions.GetRepository(workspaceRequest, repositoryRequest);

        // Assert
        PrintResult(result);
        Assert.IsNotNull(result);
    }
}