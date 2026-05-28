using Apps.Bitbucket.Api.Request;
using Apps.Bitbucket.Helper;
using Apps.Bitbucket.Webhooks.Models.Entity;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using RestSharp;

namespace Apps.Bitbucket.Webhooks.Handlers;

public abstract class BaseEventHandler(
    InvocationContext context,
    string? workspaceUuid,
    string? repositoryUuid) 
    : BitbucketInvocable(context), IWebhookEventHandler
{
    protected abstract string EventName { get; }
    
    // https://developer.atlassian.com/cloud/bitbucket/rest/api-group-repositories/#api-repositories-workspace-repo-slug-hooks-post
    public async Task SubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> creds, Dictionary<string, string> values)
    {
        var resolver = new IdentifierResolver(creds);

        string resolvedWorkspaceUuid = resolver.ResolveWorkspaceUuid(workspaceUuid);
        string resolvedRepositoryUuid = resolver.ResolveRepositoryUuid(repositoryUuid);
        string payloadUrl = values["payloadUrl"];

        var endpoint = $"repositories/{resolvedWorkspaceUuid}/{resolvedRepositoryUuid}/hooks";
        var payload = new
        {
            description = $"Blackbird {EventName} Webhook",
            url = payloadUrl,
            active = true,
            events = new[] { EventName }
        };

        var request = new BitbucketCloudRequest(endpoint, Method.Post).WithJsonBody(payload);
        await Client.ExecuteWithErrorHandling(request);
    }
    
    // https://developer.atlassian.com/cloud/bitbucket/rest/api-group-repositories/#api-repositories-workspace-repo-slug-hooks-uid-delete
    public async Task UnsubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> creds, Dictionary<string, string> values)
    {
        var resolver = new IdentifierResolver(creds);
        
        string resolvedWorkspaceUuid = resolver.ResolveWorkspaceUuid(workspaceUuid);
        string resolvedRepositoryUuid = resolver.ResolveRepositoryUuid(repositoryUuid);
        string payloadUrl = values["payloadUrl"];
        
        var getRequest = new BitbucketCloudRequest($"repositories/{resolvedWorkspaceUuid}/{resolvedRepositoryUuid}/hooks");
        var existingHooks = await Client.Paginate<WebhookEntity>(getRequest);

        var hookToDelete = existingHooks.FirstOrDefault(h => h.Url == payloadUrl);
        if (hookToDelete != null)
        {
            string endpoint = $"repositories/{resolvedWorkspaceUuid}/{resolvedRepositoryUuid}/hooks/{hookToDelete.Uuid}";
            var deleteRequest = new BitbucketCloudRequest(endpoint, Method.Delete);
            await Client.ExecuteWithErrorHandling(deleteRequest);
        }
    }
}