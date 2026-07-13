using Apps.Bitbucket.Actions;
using Apps.Bitbucket.Models.Identifiers;
using Apps.Bitbucket.Models.Identifiers.Optional;
using Apps.Bitbucket.Models.Request.File;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using Tests.Bitbucket.Base;

namespace Tests.Bitbucket;

[TestClass]
public class FileActionTests : TestBaseMultipleConnections 
{
    private const string SlashyBranchWorkspaceUuid = "{83c14ef0-d1a9-41c3-90ae-0d601a700a22}";
    private const string SlashyBranchRepositoryUuid = "{6c4ba8d8-2fc6-4b10-89ee-c26274a11852}";
    private const string SlashyBranchName = "feature/demoing";
    private const string SlashyBranchFilePath = "locales/en-US.json";

    [TestMethod, TargetConnections]
    public async Task DownloadFile_IsSuccess(InvocationContext invocationContext)
    {
        var actions = new FileActions(invocationContext, FileManager);
        var workspaceRequest = new OptionalWorkspaceIdentifier { WorkspaceUuid = SlashyBranchWorkspaceUuid };
        var repositoryRequest = new OptionalRepositoryIdentifier { RepositoryUuid = SlashyBranchRepositoryUuid };
        var branchRequest = new OptionalBranchIdentifier { BranchName = SlashyBranchName };
        var filePathRequest = new FilePathIdentifier { FilePath = SlashyBranchFilePath };
        var downloadRequest = new DownloadFileRequest { };

        var result = await actions.DownloadFile(
            workspaceRequest, 
            repositoryRequest, 
            branchRequest,
            filePathRequest,
            downloadRequest);

        TestContext?.WriteLine(result.File.Name);
        Assert.IsNotNull(result.File);
    }

    [TestMethod, TargetConnections]
    public async Task DownloadRepositoryZip_IsSuccess(InvocationContext invocationContext)
    {
        var actions = new FileActions(invocationContext, FileManager);
        var workspaceRequest = new OptionalWorkspaceIdentifier { WorkspaceUuid = "{c025e168-8bea-4666-8685-03f1c5f61503}" };
        var repositoryRequest = new OptionalRepositoryIdentifier { RepositoryUuid = "{06eefb5d-2f7b-4677-add7-c308b406155d}" };
        var branchRequest = new OptionalBranchIdentifier { };

        var result = await actions.DownloadRepositoryZip(workspaceRequest, repositoryRequest, branchRequest);

        TestContext?.WriteLine(result.File.Name);
        Assert.IsNotNull(result.File);
    }

    [TestMethod, TargetConnections]
    public async Task DeleteFile_IsSuccess(InvocationContext invocationContext)
    {
        var actions = new FileActions(invocationContext, FileManager);
        var workspaceRequest = new OptionalWorkspaceIdentifier { WorkspaceUuid = "{c025e168-8bea-4666-8685-03f1c5f61503}" };
        var repositoryRequest = new OptionalRepositoryIdentifier { RepositoryUuid = "{06eefb5d-2f7b-4677-add7-c308b406155d}" };
        var branchRequest = new OptionalBranchIdentifier { BranchName = "dev" };
        var fileIdentifier = new FilePathIdentifier { FilePath = "hello1.txt" };
        var deleteInput = new DeleteFileRequest { Message = "test msg" };

        await actions.DeleteFile(workspaceRequest, repositoryRequest, branchRequest, fileIdentifier, deleteInput);
    }

    [TestMethod, TargetConnections]
    public async Task FileExists_ExistingFile_ReturnsTrue(InvocationContext invocationContext)
    {
        var actions = new FileActions(invocationContext, FileManager);
        var workspaceRequest = new OptionalWorkspaceIdentifier { WorkspaceUuid = SlashyBranchWorkspaceUuid };
        var repositoryRequest = new OptionalRepositoryIdentifier { RepositoryUuid = SlashyBranchRepositoryUuid };
        var branchRequest = new OptionalBranchIdentifier { BranchName = SlashyBranchName };
        var fileIdentifier = new FilePathIdentifier { FilePath = SlashyBranchFilePath };

        var result = await actions.FileExists(workspaceRequest, repositoryRequest, branchRequest, fileIdentifier);

        Assert.IsTrue(result);
    }

    [TestMethod, TargetConnections]
    public async Task FileExists_NonExistingFile_ReturnsFalse(InvocationContext invocationContext)
    {
        var actions = new FileActions(invocationContext, FileManager);
        var workspaceRequest = new OptionalWorkspaceIdentifier { WorkspaceUuid = SlashyBranchWorkspaceUuid };
        var repositoryRequest = new OptionalRepositoryIdentifier { RepositoryUuid = SlashyBranchRepositoryUuid };
        var branchRequest = new OptionalBranchIdentifier { BranchName = SlashyBranchName };
        var fileIdentifier = new FilePathIdentifier { FilePath = "locales/not-found.json" };

        var result = await actions.FileExists(workspaceRequest, repositoryRequest, branchRequest, fileIdentifier);

        Assert.IsFalse(result);
    }

    [TestMethod, TargetConnections]
    public async Task UploadFile_IsSuccess(InvocationContext invocationContext)
    {
        var actions = new FileActions(invocationContext, FileManager);
        var workspaceRequest = new OptionalWorkspaceIdentifier { WorkspaceUuid = "{c025e168-8bea-4666-8685-03f1c5f61503}" };
        var repositoryRequest = new OptionalRepositoryIdentifier { RepositoryUuid = "{06eefb5d-2f7b-4677-add7-c308b406155d}" };
        var branchRequest = new OptionalBranchIdentifier { BranchName = "dev" };
        var optionalFolderPathIdentifier = new OptionalFolderPathIdentifier { FolderPath = "123/456" };
        var uploadRequest = new UploadFileRequest
        {
            File = new FileReference { Name = "helloworld.txt" },
            FileName = "test.txt"
        };

        await actions.UploadFile(
            workspaceRequest, 
            repositoryRequest, 
            branchRequest,
            optionalFolderPathIdentifier,
            uploadRequest);
    }

    [TestMethod, TargetConnections]
    public async Task SearchFiles_ReturnsFiles(InvocationContext invocationContext)
    {
        var actions = new FileActions(invocationContext, FileManager);
        var workspaceRequest = new OptionalWorkspaceIdentifier { WorkspaceUuid = SlashyBranchWorkspaceUuid };
        var repositoryRequest = new OptionalRepositoryIdentifier { RepositoryUuid = SlashyBranchRepositoryUuid };
        var branchRequest = new OptionalBranchIdentifier { BranchName = SlashyBranchName };
        var optionalFolderPathIdentifier = new OptionalFolderPathIdentifier { FolderPath = "locales" };
        var searchFilesRequest = new SearchFilesRequest
        {
            IncludeSubfolders = true,
        };

        var result = await actions.SearchFiles(
            workspaceRequest, 
            repositoryRequest,
            branchRequest,
            optionalFolderPathIdentifier,
            searchFilesRequest);

        PrintResult(result);
        Assert.IsNotNull(result);
        Assert.IsNotEmpty(result.Files, "No files returned");
    }
}
