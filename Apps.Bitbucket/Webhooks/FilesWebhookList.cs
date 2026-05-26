using System.Net;
using Apps.Bitbucket.Api.Request;
using Apps.Bitbucket.Extensions;
using Apps.Bitbucket.Helper;
using Apps.Bitbucket.Models.Entities.Diffstat;
using Apps.Bitbucket.Models.Identifiers.Optional;
using Apps.Bitbucket.Models.Response.File;
using Apps.Bitbucket.Webhooks.Handlers;
using Apps.Bitbucket.Webhooks.Models.Payloads.Push;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;

namespace Apps.Bitbucket.Webhooks;

[WebhookList("Files")]
public class FilesWebhookList(InvocationContext context) : BitbucketInvocable(context)
{
    private readonly IdentifierResolver _resolver = new(context.AuthenticationCredentialsProviders);
    
    [Webhook("On files added", typeof(PushEventHandler), 
        Description = "Triggers for each new file added to the repository")]
    public async Task<WebhookResponse<SearchFilesResponse>> OnFilesAdded(
        WebhookRequest webhookRequest,
        [WebhookParameter(true)] OptionalWorkspaceIdentifier workspaceIdentifier,
        [WebhookParameter(true)] OptionalRepositoryIdentifier repositoryIdentifier,
        [WebhookParameter] OptionalBranchIdentifier branchIdentifier)
    {
        return await ProcessFileWebhook(
            webhookRequest, 
            workspaceIdentifier.WorkspaceUuid, 
            repositoryIdentifier.RepositoryUuid,
            branchIdentifier.BranchName,
            ["added"]);
    }
    
    [Webhook("On files added or modified", typeof(PushEventHandler), 
        Description = "Triggers for each new file added to the repository or when an existing file is modified")]
    public async Task<WebhookResponse<SearchFilesResponse>> OnFilesAddedOrModified(
        WebhookRequest webhookRequest,
        [WebhookParameter(true)] OptionalWorkspaceIdentifier workspaceIdentifier,
        [WebhookParameter(true)] OptionalRepositoryIdentifier repositoryIdentifier,
        [WebhookParameter] OptionalBranchIdentifier branchIdentifier)
    {
        return await ProcessFileWebhook(
            webhookRequest, 
            workspaceIdentifier.WorkspaceUuid, 
            repositoryIdentifier.RepositoryUuid,
            branchIdentifier.BranchName,
            ["added", "modified"]);
    }

    private async Task<WebhookResponse<SearchFilesResponse>> ProcessFileWebhook(
        WebhookRequest webhookRequest,
        string? workspaceIdentifier,
        string? repositoryIdentifier,
        string? branchName,
        List<string> fileStatuses)
    {
        var payload = webhookRequest.GetPayload<PushPayload>();
        
        var latestChange = payload.Push.Changes.FirstOrDefault();
        if (latestChange is null)
            return await Preflight<SearchFilesResponse>();
        
        string? newHash = latestChange.New?.Target?.Hash;
        if (string.IsNullOrEmpty(newHash))
            return await Preflight<SearchFilesResponse>();

        string actualBranchName = latestChange.New?.Name ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(branchName) && 
            !string.Equals(branchName, actualBranchName, StringComparison.OrdinalIgnoreCase))
        {
            return await Preflight<SearchFilesResponse>();
        }
        
        string workspaceUuid = _resolver.ResolveWorkspaceUuid(workspaceIdentifier);
        string repositoryUuid = _resolver.ResolveRepositoryUuid(repositoryIdentifier);
        
        var request = new BitbucketCloudRequest($"repositories/{workspaceUuid}/{repositoryUuid}/diffstat/{newHash}");
        var response = await Client.Paginate<DiffstatEntity>(request);

        var newFiles = response
            .Where(x => fileStatuses.Contains(x.Status) && x.NewFile is not null)
            .Select(x => new FileResponse(x.NewFile!))
            .ToList();
        
        if (newFiles.Count == 0)
            return await Preflight<SearchFilesResponse>();
        
        return await Success<SearchFilesResponse>(new(newFiles));
    }
    
    private static Task<WebhookResponse<T>> Preflight<T>() where T : class
    {
        return Task.FromResult(new WebhookResponse<T>
        {
            HttpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK),
            ReceivedWebhookRequestType = WebhookRequestType.Preflight
        });
    }

    private static Task<WebhookResponse<T>> Success<T>(T data) where T : class
    {
        return Task.FromResult<WebhookResponse<T>>(new()
        {
            HttpResponseMessage = null,
            Result = data
        });
    }
}