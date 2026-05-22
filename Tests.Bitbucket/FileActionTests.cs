using Apps.Bitbucket.Actions;
using Apps.Bitbucket.Models.Identifiers;
using Apps.Bitbucket.Models.Identifiers.Optional;
using Apps.Bitbucket.Models.Request.File;
using Blackbird.Applications.Sdk.Common.Files;
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

    [TestMethod]
    public async Task DeleteFile_IsSuccess()
    {
        // Arrange
        var actions = new FileActions(InvocationContext, FileManager);
        var workspaceRequest = new WorkspaceIdentifier { WorkspaceUuid = "{c025e168-8bea-4666-8685-03f1c5f61503}" };
        var repositoryRequest = new RepositoryIdentifier { RepositoryUuid = "{06eefb5d-2f7b-4677-add7-c308b406155d}" };
        var branchRequest = new OptionalBranchIdentifier { BranchName = "dev" };
        var fileIdentifier = new FilePathIdentifier { FilePath = "hello1.txt" };
        var deleteInput = new DeleteFileRequest { Message = "test msg" };

        // Act
        await actions.DeleteFile(workspaceRequest, repositoryRequest, branchRequest, fileIdentifier, deleteInput);
    }

    [TestMethod]
    public async Task FileExists_ExistingFile_ReturnsTrue()
    {
        // Arrange
        var actions = new FileActions(InvocationContext, FileManager);
        var workspaceRequest = new WorkspaceIdentifier { WorkspaceUuid = "{c025e168-8bea-4666-8685-03f1c5f61503}" };
        var repositoryRequest = new RepositoryIdentifier { RepositoryUuid = "{06eefb5d-2f7b-4677-add7-c308b406155d}" };
        var branchRequest = new OptionalBranchIdentifier { BranchName = "dev" };
        var fileIdentifier = new FilePathIdentifier { FilePath = "hello.txt" };

        // Act
        var result = await actions.FileExists(workspaceRequest, repositoryRequest, branchRequest, fileIdentifier);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task FileExists_NonExistingFile_ReturnsFalse()
    {
        // Arrange
        var actions = new FileActions(InvocationContext, FileManager);
        var workspaceRequest = new WorkspaceIdentifier { WorkspaceUuid = "{c025e168-8bea-4666-8685-03f1c5f61503}" };
        var repositoryRequest = new RepositoryIdentifier { RepositoryUuid = "{06eefb5d-2f7b-4677-add7-c308b406155d}" };
        var branchRequest = new OptionalBranchIdentifier { BranchName = "dev" };
        var fileIdentifier = new FilePathIdentifier { FilePath = "hello1.txt" };

        // Act
        var result = await actions.FileExists(workspaceRequest, repositoryRequest, branchRequest, fileIdentifier);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task UploadFile_IsSuccess()
    {
        // Arrange
        var actions = new FileActions(InvocationContext, FileManager);
        var workspaceRequest = new WorkspaceIdentifier { WorkspaceUuid = "{c025e168-8bea-4666-8685-03f1c5f61503}" };
        var repositoryRequest = new RepositoryIdentifier { RepositoryUuid = "{06eefb5d-2f7b-4677-add7-c308b406155d}" };
        var branchRequest = new OptionalBranchIdentifier { BranchName = "dev" };
        var optionalFolderPathIdentifier = new OptionalFolderPathIdentifier { FolderPath = "123/456" };
        var uploadRequest = new UploadFileRequest
        {
            File = new FileReference { Name = "helloworld.txt" },
            FileName = "test.txt"
        };

        // Act
        await actions.UploadFile(
            workspaceRequest, 
            repositoryRequest, 
            branchRequest,
            optionalFolderPathIdentifier,
            uploadRequest);
    }

    [TestMethod]
    public async Task SearchFiles_ReturnsFiles()
    {
        // Arrange
        var actions = new FileActions(InvocationContext, FileManager);
        var workspaceRequest = new WorkspaceIdentifier { WorkspaceUuid = "{c025e168-8bea-4666-8685-03f1c5f61503}" };
        var repositoryRequest = new RepositoryIdentifier { RepositoryUuid = "{06eefb5d-2f7b-4677-add7-c308b406155d}" };
        var branchRequest = new OptionalBranchIdentifier { BranchName = "dev" };
        var optionalFolderPathIdentifier = new OptionalFolderPathIdentifier { FolderPath = "123" };
        var searchFilesRequest = new SearchFilesRequest
        {
            IncludeSubfolders = true,
        };

        // Act
        var result = await actions.SearchFiles(
            workspaceRequest, 
            repositoryRequest,
            branchRequest,
            optionalFolderPathIdentifier,
            searchFilesRequest);

        // Assert
        PrintResult(result);
        Assert.IsNotNull(result);
        Assert.IsNotEmpty(result.Files, "No files returned");
    }
}