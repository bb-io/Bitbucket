using Apps.Bitbucket.Constants;
using Apps.Bitbucket.Models.Identifiers.Optional;
using Apps.Bitbucket.Webhooks;
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
        
        // Act
        var result = await webhookList.OnFilesAddedOrModified(webhookRequest, workspaceRequest, repositoryRequest, branchRequest, filepath);

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

        // Act
        var result = await webhookList.OnFilesAddedOrModified(webhookRequest, workspaceRequest, repositoryRequest, branchRequest, filepath);

        // Assert
        PrintResult(result);
        Assert.IsNotNull(result.Result, "The result is null");
        Assert.IsNotEmpty(result.Result.Files, "No files returned");
    }

    private static WebhookRequest CreateWebhookRequest(string fileName)
    {
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var projectDirectory = Directory.GetParent(baseDirectory)?.Parent?.Parent?.Parent?.FullName;
        var body = File.ReadAllText($"{projectDirectory}/TestFiles/Input/{fileName}");
        return new WebhookRequest { Body = body };
    }
}
