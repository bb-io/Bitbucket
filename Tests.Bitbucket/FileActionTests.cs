using Apps.Bitbucket.Actions;
using Apps.Bitbucket.Models.Identifiers;
using Apps.Bitbucket.Models.Identifiers.Optional;
using Tests.Bitbucket.Base;

namespace Tests.Bitbucket;

[TestClass]
public class FileActionTests : TestBase
{
    [TestMethod]
    public async Task DownloadFile_IsSuccess()
    {
        // Arrange
        var actions = new FileActions(InvocationContext, FileManager);
        var workspaceRequest = new WorkspaceIdentifier { WorkspaceUuid = "{c025e168-8bea-4666-8685-03f1c5f61503}" };
        var repositoryRequest = new RepositoryIdentifier { RepositoryUuid = "{06eefb5d-2f7b-4677-add7-c308b406155d}" };
        var branchRequest = new OptionalBranchIdentifier { BranchName = "dev" };
        var filePathRequest = new FilePathIdentifier { FilePath = "123/hi.txt" };

        // Act
        var result = await actions.DownloadFile(
            workspaceRequest, 
            repositoryRequest, 
            branchRequest,
            filePathRequest);

        // Assert
        Console.WriteLine(result.File.Name);
        Assert.IsNotNull(result.File);
    }

    [TestMethod]
    public async Task DownloadRepositoryZip_IsSuccess()
    {
        // Arrange
        var actions = new FileActions(InvocationContext, FileManager);
        var workspaceRequest = new WorkspaceIdentifier { WorkspaceUuid = "{c025e168-8bea-4666-8685-03f1c5f61503}" };
        var repositoryRequest = new RepositoryIdentifier { RepositoryUuid = "{06eefb5d-2f7b-4677-add7-c308b406155d}" };
        var branchRequest = new OptionalBranchIdentifier { };

        // Act
        var result = await actions.DownloadRepositoryZip(workspaceRequest, repositoryRequest, branchRequest);

        // Assert
        Console.WriteLine(result.File.Name);
        Assert.IsNotNull(result.File);
    }
}