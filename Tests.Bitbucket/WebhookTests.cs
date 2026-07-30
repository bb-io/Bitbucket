using Apps.Bitbucket.Constants;
using Apps.Bitbucket.Models.Identifiers.Optional;
using Apps.Bitbucket.Webhooks;
using Apps.Bitbucket.Webhooks.Models.Request.File;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Tests.Bitbucket.Base;

namespace Tests.Bitbucket;

[TestClass]
public class WebhookTests : TestBaseMultipleConnections
{
    [TestMethod, TargetConnections(ConnectionTypes.ApiToken)]
    public async Task OnFilesAddedOrModified_IsSuccess(InvocationContext context)
    {
        // Arrange
        var webhookList = new FilesWebhookList(context);
        var webhookRequest = CreateWebhookRequest("push_payload.json");
        var workspaceRequest = new OptionalWorkspaceIdentifier { WorkspaceUuid = "{83c14ef0-d1a9-41c3-90ae-0d601a700a22}" };
        var repositoryRequest = new OptionalRepositoryIdentifier { RepositoryUuid = "{6c4ba8d8-2fc6-4b10-89ee-c26274a11852}" };
        var branchRequest = new OptionalBranchIdentifier { BranchNameContains = ["feature/"] };
        var filepath = new OptionalFilepath() { FilePathPatterns = ["locales/([a-zA-z]{2}-[a-zA-z]{2}).json"] };
        var input = new OnFilesAddedOrModifiedRequest { FileExtensions = [""] };
        
        // Act
        var result = await webhookList.OnFilesAddedOrModified(
            webhookRequest,
            workspaceRequest, 
            repositoryRequest,
            branchRequest, 
            filepath,
            input);

        // Assert
        PrintResult(result);
        Assert.IsNotNull(result.Result, "The result is null");
        Assert.IsNotEmpty(result.Result.Files, "No files returned");
    }

    [TestMethod, TargetConnections(ConnectionTypes.ApiToken)]
    public async Task OnFilesAddedOrModified_NoCaptureGroup_IsSuccess(InvocationContext context)
    {
        // Arrange
        var webhookList = new FilesWebhookList(context);
        var webhookRequest = CreateWebhookRequest("push_payload.json");
        var workspaceRequest = new OptionalWorkspaceIdentifier { WorkspaceUuid = "{83c14ef0-d1a9-41c3-90ae-0d601a700a22}" };
        var repositoryRequest = new OptionalRepositoryIdentifier { RepositoryUuid = "{6c4ba8d8-2fc6-4b10-89ee-c26274a11852}" };
        var branchRequest = new OptionalBranchIdentifier { BranchNameContains = ["feature/"] };
        var filepath = new OptionalFilepath() { FilePathPatterns = ["locales/en-US.json"] };
        var input = new OnFilesAddedOrModifiedRequest { FileExtensions = [""] };

        // Act
        var result = await webhookList.OnFilesAddedOrModified(
            webhookRequest,
            workspaceRequest, 
            repositoryRequest,
            branchRequest, 
            filepath,
            input);
        
        // Assert
        PrintResult(result);
        Assert.IsNotNull(result.Result, "The result is null");
        Assert.IsNotEmpty(result.Result.Files, "No files returned");
    }

    [TestMethod, TargetConnections(ConnectionTypes.ApiToken)]
    public async Task OnPullRequestCreated_IsSuccess(InvocationContext context)
    {
        // Arrange
        var webhookList = new PullRequestWebhookList(context);
        var webhookRequest = CreateWebhookRequest("pull_request_created.json");
        var workspaceRequest = new OptionalWorkspaceIdentifier { WorkspaceUuid = "{83c14ef0-d1a9-41c3-90ae-0d601a700a22}" };
        var repositoryRequest = new OptionalRepositoryIdentifier { RepositoryUuid = "{6c4ba8d8-2fc6-4b10-89ee-c26274a11852}" };
        var branchRequest = new OptionalBranchIdentifier { BranchNameContains = ["feature/"] };
        var filter = new OptionalPullRequestFilter { TitleContains = ["Sample"] };
        var filepath = new OptionalFilepath();

        // Act
        var result = await webhookList.OnPullRequestCreated(
            webhookRequest,
            workspaceRequest,
            repositoryRequest,
            branchRequest,
            filter,
            filepath);

        // Assert
        PrintResult(result);
        Assert.IsNotNull(result.Result, "The result is null");
        Assert.AreEqual("Sample PR", result.Result.Title);
        Assert.IsNotNull(result.Result.AffectedFiles, "Affected files list is null");
    }

    [TestMethod, TargetConnections(ConnectionTypes.ApiToken)]
    public async Task OnPullRequestCreatedOrUpdated_IsSuccess(InvocationContext context)
    {
        // Arrange
        var webhookList = new PullRequestWebhookList(context);
        var webhookRequest = CreateWebhookRequest("pull_request_updated.json");
        var workspaceRequest = new OptionalWorkspaceIdentifier { WorkspaceUuid = "{83c14ef0-d1a9-41c3-90ae-0d601a700a22}" };
        var repositoryRequest = new OptionalRepositoryIdentifier { RepositoryUuid = "{6c4ba8d8-2fc6-4b10-89ee-c26274a11852}" };
        var branchRequest = new OptionalBranchIdentifier { BranchNameContains = ["feature/"] };
        var filter = new OptionalPullRequestFilter { DescriptionContains = ["Sample commit"] };
        var filepath = new OptionalFilepath();

        // Act
        var result = await webhookList.OnPullRequestCreatedOrUpdated(
            webhookRequest,
            workspaceRequest,
            repositoryRequest,
            branchRequest,
            filter,
            filepath);

        // Assert
        PrintResult(result);
        Assert.IsNotNull(result.Result, "The result is null");
        Assert.AreEqual("feature/demoing", result.Result.SourceBranchName);
        Assert.IsNotNull(result.Result.AffectedFiles, "Affected files list is null");
    }

    [TestMethod, TargetConnections(ConnectionTypes.ApiToken)]
    public async Task OnPullRequestCreatedOrUpdated_FilePatternWithoutAffectedFile_ReturnsPreflight(InvocationContext context)
    {
        // Arrange
        var webhookList = new PullRequestWebhookList(context);
        var webhookRequest = CreateWebhookRequest("pull_request_updated.json");
        var workspaceRequest = new OptionalWorkspaceIdentifier { WorkspaceUuid = "{83c14ef0-d1a9-41c3-90ae-0d601a700a22}" };
        var repositoryRequest = new OptionalRepositoryIdentifier { RepositoryUuid = "{6c4ba8d8-2fc6-4b10-89ee-c26274a11852}" };
        var branchRequest = new OptionalBranchIdentifier { BranchNameContains = ["feature/"] };
        var filter = new OptionalPullRequestFilter { DescriptionContains = ["Sample commit"] };
        var filepath = new OptionalFilepath { FilePathPatterns = ["^does-not-exist/"] };

        // Act
        var result = await webhookList.OnPullRequestCreatedOrUpdated(
            webhookRequest,
            workspaceRequest,
            repositoryRequest,
            branchRequest,
            filter,
            filepath);

        // Assert
        PrintResult(result);
        Assert.IsNull(result.Result, "The result should be null");
        Assert.AreEqual(WebhookRequestType.Preflight, result.ReceivedWebhookRequestType);
    }

    private static WebhookRequest CreateWebhookRequest(string fileName)
    {
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var projectDirectory = Directory.GetParent(baseDirectory)?.Parent?.Parent?.Parent?.FullName;
        var body = File.ReadAllText($"{projectDirectory}/TestFiles/Input/{fileName}");
        return new WebhookRequest { Body = body };
    }
}
