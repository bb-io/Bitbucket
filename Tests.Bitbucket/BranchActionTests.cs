using Apps.Bitbucket.Actions;
using Apps.Bitbucket.Models.Identifiers.Optional;
using Apps.Bitbucket.Models.Request.Branch;
using Blackbird.Applications.Sdk.Common.Invocation;
using Tests.Bitbucket.Base;

namespace Tests.Bitbucket;

[TestClass]
public class BranchActionTests : TestBaseMultipleConnections
{
    [TestMethod, TargetConnections]
    public async Task SearchBranches_ReturnsBranches(InvocationContext invocationContext)
    {
        // Arrange
        var actions = new BranchActions(invocationContext);
        var workspaceRequest = new OptionalWorkspaceIdentifier { WorkspaceUuid = "{c025e168-8bea-4666-8685-03f1c5f61503}" };
        var repositoryRequest = new OptionalRepositoryIdentifier { RepositoryUuid = "{06eefb5d-2f7b-4677-add7-c308b406155d}" };
        var searchInput = new SearchBranchesRequest { BranchNameContains = "" };

        // Act
        var result = await actions.SearchBranches(workspaceRequest, repositoryRequest, searchInput);

        // Assert
        PrintResult(result);
        Assert.IsNotNull(result);
        Assert.IsNotEmpty(result.Branches, "No branches returned");
    }

    [TestMethod, TargetConnections]
    public async Task BranchExists_ExistingBranch_ReturnsTrue(InvocationContext invocationContext)
    {
        // Arrange
        var actions = new BranchActions(invocationContext);
        var workspaceRequest = new OptionalWorkspaceIdentifier { WorkspaceUuid = "{c025e168-8bea-4666-8685-03f1c5f61503}" };
        var repositoryRequest = new OptionalRepositoryIdentifier { RepositoryUuid = "{06eefb5d-2f7b-4677-add7-c308b406155d}" };
        var existsInput = new BranchExistsRequest { Name = "dev" };

        // Act
        var result = await actions.BranchExists(workspaceRequest, repositoryRequest, existsInput);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod, TargetConnections]
    public async Task BranchExists_NonExistingBranch_ReturnsFalse(InvocationContext invocationContext)
    {
        // Arrange
        var actions = new BranchActions(invocationContext);
        var workspaceRequest = new OptionalWorkspaceIdentifier { WorkspaceUuid = "{c025e168-8bea-4666-8685-03f1c5f61503}" };
        var repositoryRequest = new OptionalRepositoryIdentifier { RepositoryUuid = "{06eefb5d-2f7b-4677-add7-c308b406155d}" };
        var existsInput = new BranchExistsRequest { Name = "test" };

        // Act
        var result = await actions.BranchExists(workspaceRequest, repositoryRequest, existsInput);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod, TargetConnections]
    public async Task CreateBranch_ReturnsCreatedBranch(InvocationContext invocationContext)
    {
        // Arrange
        var actions = new BranchActions(invocationContext);
        var workspaceRequest = new OptionalWorkspaceIdentifier { WorkspaceUuid = "{c025e168-8bea-4666-8685-03f1c5f61503}" };
        var repositoryRequest = new OptionalRepositoryIdentifier { RepositoryUuid = "{06eefb5d-2f7b-4677-add7-c308b406155d}" };
        var createInput = new CreateBranchRequest
        {
            BranchName = "fromtests1",
            SourceBranchName = "dev"
        };

        // Act
        var result = await actions.CreateBranch(workspaceRequest, repositoryRequest, createInput);

        // Assert
        PrintResult(result);
        Assert.IsNotNull(result);
    }
}