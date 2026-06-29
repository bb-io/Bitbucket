using System.Net;
using System.Text.RegularExpressions;
using Apps.Bitbucket.Api.Request;
using Apps.Bitbucket.Extensions;
using Apps.Bitbucket.Helper;
using Apps.Bitbucket.Models.Entities.Diffstat;
using Apps.Bitbucket.Models.Entities.File;
using Apps.Bitbucket.Models.Identifiers.Optional;
using Apps.Bitbucket.Models.Response.File;
using Apps.Bitbucket.Webhooks.Handlers;
using Apps.Bitbucket.Webhooks.Models.Entity.Push;
using Apps.Bitbucket.Webhooks.Models.Payloads.Push;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Webhooks;

namespace Apps.Bitbucket.Webhooks;

[WebhookList("Files")]
public class FilesWebhookList(InvocationContext context) : BitbucketInvocable(context)
{
    private readonly IdentifierResolver _resolver = new(context.AuthenticationCredentialsProviders);
    
    [Webhook("On files added", typeof(PushEventHandler), 
        Description = "Triggers for each new file added to the repository")]
    public async Task<WebhookResponse<SearchFileWebhookResponse>> OnFilesAdded(
        WebhookRequest webhookRequest,
        [WebhookParameter(true)] OptionalWorkspaceIdentifier workspaceIdentifier,
        [WebhookParameter(true)] OptionalRepositoryIdentifier repositoryIdentifier,
        [WebhookParameter] OptionalBranchIdentifier branchIdentifier,
        [WebhookParameter] OptionalFilepath filepath)
    {
        return await ProcessFileWebhook(
            webhookRequest, 
            workspaceIdentifier.WorkspaceUuid, 
            repositoryIdentifier.RepositoryUuid,
            branchIdentifier,
            filepath,
            ["added"]);
    }
    
    [Webhook("On files added or modified", typeof(PushEventHandler), 
        Description = "Triggers for each new file added to the repository or when an existing file is modified")]
    public async Task<WebhookResponse<SearchFileWebhookResponse>> OnFilesAddedOrModified(
        WebhookRequest webhookRequest,
        [WebhookParameter(true)] OptionalWorkspaceIdentifier workspaceIdentifier,
        [WebhookParameter(true)] OptionalRepositoryIdentifier repositoryIdentifier,
        [WebhookParameter] OptionalBranchIdentifier branchIdentifier,
        [WebhookParameter] OptionalFilepath filepath)
    {
        return await ProcessFileWebhook(
            webhookRequest, 
            workspaceIdentifier.WorkspaceUuid, 
            repositoryIdentifier.RepositoryUuid,
            branchIdentifier,
            filepath,
            ["added", "modified"]);
    }
    
    [Webhook("On files modified", typeof(PushEventHandler), 
        Description = "Triggers when an existing file is modified")]
    public async Task<WebhookResponse<SearchFileWebhookResponse>> OnFilesModified(
        WebhookRequest webhookRequest,
        [WebhookParameter(true)] OptionalWorkspaceIdentifier workspaceIdentifier,
        [WebhookParameter(true)] OptionalRepositoryIdentifier repositoryIdentifier,
        [WebhookParameter] OptionalBranchIdentifier branchIdentifier,
        [WebhookParameter] OptionalFilepath filepath)
    {
        return await ProcessFileWebhook(
            webhookRequest, 
            workspaceIdentifier.WorkspaceUuid, 
            repositoryIdentifier.RepositoryUuid,
            branchIdentifier,
            filepath,
            ["modified"]);
    }
    
    [Webhook("On files removed", typeof(PushEventHandler), 
        Description = "Triggers when an existing file is removed")]
    public async Task<WebhookResponse<SearchFileWebhookResponse>> OnFilesRemoved(
        WebhookRequest webhookRequest,
        [WebhookParameter(true)] OptionalWorkspaceIdentifier workspaceIdentifier,
        [WebhookParameter(true)] OptionalRepositoryIdentifier repositoryIdentifier,
        [WebhookParameter] OptionalBranchIdentifier branchIdentifier,
        [WebhookParameter] OptionalFilepath filepath)
    {
        return await ProcessFileWebhook(
            webhookRequest, 
            workspaceIdentifier.WorkspaceUuid, 
            repositoryIdentifier.RepositoryUuid,
            branchIdentifier,
            filepath,
            ["removed"]);
    }

    private async Task<WebhookResponse<SearchFileWebhookResponse>> ProcessFileWebhook(
        WebhookRequest webhookRequest,
        string? workspaceIdentifier,
        string? repositoryIdentifier,
        OptionalBranchIdentifier branchIdentifier,
        OptionalFilepath filepath,
        List<string> fileStatuses)
    {
        var payload = webhookRequest.GetPayload<PushPayload>();
        
        var latestChange = payload.Push.Changes.FirstOrDefault();
        if (latestChange is null)
            return await Preflight<SearchFileWebhookResponse>();
        
        string? newHash = latestChange.New?.Target?.Hash;
        if (string.IsNullOrEmpty(newHash))
            return await Preflight<SearchFileWebhookResponse>();

        string actualBranchName = latestChange.New?.Name ?? string.Empty;
        if (!BranchMatches(actualBranchName, branchIdentifier))
            return await Preflight<SearchFileWebhookResponse>();

        var commitMessage = GetCommitMessage(latestChange);
        
        string workspaceUuid = _resolver.ResolveWorkspaceUuid(workspaceIdentifier);
        string repositoryUuid = _resolver.ResolveRepositoryUuid(repositoryIdentifier);
        
        var request = new BitbucketCloudRequest($"repositories/{workspaceUuid}/{repositoryUuid}/diffstat/{newHash}");
        var response = await Client.Paginate<DiffstatEntity>(request);

        var targetFiles = response
            .Where(x => fileStatuses.Contains(x.Status))
            .Select(x => x.NewFile ?? x.OldFile)
            .Where(file => file is not null)
            .Select(file => CreateFileResponse(file!, filepath?.FilePathPatterns, actualBranchName, commitMessage))
            .Where(file => file is not null)
            .Select(file => file!)
            .ToList();
    
        if (targetFiles.Count == 0)
            return await Preflight<SearchFileWebhookResponse>();
    
        return await Success(new SearchFileWebhookResponse(targetFiles));
    }

    public static bool BranchMatches(string actualBranchName, OptionalBranchIdentifier? branchIdentifier)
    {
        if (branchIdentifier is null)
            return true;

        if (!string.IsNullOrWhiteSpace(branchIdentifier.BranchName) &&
            !string.Equals(branchIdentifier.BranchName, actualBranchName, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var contains = branchIdentifier.BranchNameContains?
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value.Trim())
            .ToList() ?? [];

        if (contains.Count > 0 &&
            !contains.Any(value => actualBranchName.Contains(value, StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        var doesntContain = branchIdentifier.BranchNameDoesntContain?
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value.Trim())
            .ToList() ?? [];

        return doesntContain.Count == 0 ||
               !doesntContain.Any(value => actualBranchName.Contains(value, StringComparison.OrdinalIgnoreCase));
    }

    public static FileWebhookResponse? CreateFileResponse(
        FileEntity file,
        IEnumerable<string>? rawPatterns,
        string branchName,
        string? commitMessage = null)
    {
        var patterns = rawPatterns?
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value.Trim())
            .ToList() ?? [];

        if (patterns.Count == 0)
            return new(file, branchName, null, commitMessage);

        foreach (var pattern in patterns)
        {
            Regex regex;
            try
            {
                regex = new Regex(pattern);
            }
            catch (ArgumentException exception)
            {
                throw new PluginMisconfigurationException(
                    $"Invalid file path regex pattern '{pattern}': {exception.Message}");
            }

            var groupNames = regex.GetGroupNames()
                .Where(name => name != "0")
                .ToList();

            if (groupNames.Count > 1)
            {
                throw new PluginMisconfigurationException(
                    $"File path regex pattern '{pattern}' must contain no more than one capture group.");
            }

            var match = regex.Match(file.Path);
            if (!match.Success)
                continue;

            var extractedPart = groupNames.Count == 1
                ? match.Groups[groupNames[0]].Value
                : null;

            return new(file, branchName, extractedPart, commitMessage);
        }

        return null;
    }

    public static string? GetCommitMessage(Change change)
    {
        var messages = change.Commits
            .Select(commit => commit.Message?.Trim())
            .Where(message => !string.IsNullOrWhiteSpace(message))
            .ToList();

        return messages.Count == 0
            ? null
            : string.Join(Environment.NewLine, messages);
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
