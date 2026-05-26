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
    public async Task OnFilesAdded_IsSuccess(InvocationContext context)
    {
        // Arrange
        var webhookList = new FilesWebhookList(context);
        var webhookRequest = CreateWebhookRequest("push_payload.json");
        var workspaceRequest = new OptionalWorkspaceIdentifier { WorkspaceUuid = "{c025e168-8bea-4666-8685-03f1c5f61503}" };
        var repositoryRequest = new OptionalRepositoryIdentifier { RepositoryUuid = "{06eefb5d-2f7b-4677-add7-c308b406155d}" };
        var branchRequest = new OptionalBranchIdentifier { BranchName = "main" };
        
        // Act
        var result = await webhookList.OnFilesAdded(webhookRequest, workspaceRequest, repositoryRequest, branchRequest);

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