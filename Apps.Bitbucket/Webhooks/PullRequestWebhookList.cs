using System.Net;
using Apps.Bitbucket.Api.Request;
using Apps.Bitbucket.Extensions;
using Apps.Bitbucket.Helper;
using Apps.Bitbucket.Models.Entities.Diffstat;
using Apps.Bitbucket.Models.Entities.PullRequest;
using Apps.Bitbucket.Models.Identifiers.Optional;
using Apps.Bitbucket.Models.Response.PullRequest;
using Apps.Bitbucket.Webhooks.Handlers;
using Apps.Bitbucket.Webhooks.Helpers;
using Apps.Bitbucket.Webhooks.Models.Payloads.PullRequest;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;
using RestSharp;

namespace Apps.Bitbucket.Webhooks;

[WebhookList("Pull requests")]
public class PullRequestWebhookList(InvocationContext context) : BitbucketInvocable(context)
{
    private readonly IdentifierResolver _resolver = new(context.AuthenticationCredentialsProviders);

    [Webhook("On pull request created", typeof(PullRequestCreatedEventHandler),
        Description = "Triggers when a pull request is created")]
    public async Task<WebhookResponse<PullRequestResponse>> OnPullRequestCreated(
        WebhookRequest webhookRequest,
        [WebhookParameter(true)] OptionalWorkspaceIdentifier workspaceIdentifier,
        [WebhookParameter(true)] OptionalRepositoryIdentifier repositoryIdentifier,
        [WebhookParameter] OptionalBranchIdentifier branchIdentifier,
        [WebhookParameter] OptionalPullRequestFilter pullRequestFilter,
        [WebhookParameter] OptionalFilepath filepath)
    {
        return await ProcessPullRequestWebhook(
            webhookRequest,
            workspaceIdentifier.WorkspaceUuid,
            repositoryIdentifier.RepositoryUuid,
            branchIdentifier,
            pullRequestFilter,
            filepath);
    }

    [Webhook("On pull request created or updated", typeof(PullRequestCreatedOrUpdatedEventHandler),
        Description = "Triggers when a pull request is created or updated")]
    public async Task<WebhookResponse<PullRequestResponse>> OnPullRequestCreatedOrUpdated(
        WebhookRequest webhookRequest,
        [WebhookParameter(true)] OptionalWorkspaceIdentifier workspaceIdentifier,
        [WebhookParameter(true)] OptionalRepositoryIdentifier repositoryIdentifier,
        [WebhookParameter] OptionalBranchIdentifier branchIdentifier,
        [WebhookParameter] OptionalPullRequestFilter pullRequestFilter,
        [WebhookParameter] OptionalFilepath filepath)
    {
        return await ProcessPullRequestWebhook(
            webhookRequest,
            workspaceIdentifier.WorkspaceUuid,
            repositoryIdentifier.RepositoryUuid,
            branchIdentifier,
            pullRequestFilter,
            filepath);
    }

    private async Task<WebhookResponse<PullRequestResponse>> ProcessPullRequestWebhook(
        WebhookRequest webhookRequest,
        string? workspaceIdentifier,
        string? repositoryIdentifier,
        OptionalBranchIdentifier branchIdentifier,
        OptionalPullRequestFilter pullRequestFilter,
        OptionalFilepath filepath)
    {
        var payload = webhookRequest.GetPayload<PullRequestPayload>();
        var pullRequest = payload.PullRequest;

        if (pullRequest is null)
            return await Preflight<PullRequestResponse>();

        if (!PullRequestMatches(pullRequest, branchIdentifier, pullRequestFilter))
            return await Preflight<PullRequestResponse>();

        var sourceBranchName = pullRequest.Source?.Branch?.Name ?? string.Empty;
        var request = CreateDiffstatRequest(pullRequest, workspaceIdentifier, repositoryIdentifier);
        var diffstats = await Client.Paginate<DiffstatEntity>(request);
        var affectedFiles = WebhookFilterHelper.CreateFileResponses(
            diffstats,
            null,
            filepath?.FilePathPatterns,
            sourceBranchName);

        if (HasFilePathPatterns(filepath) && affectedFiles.Count == 0)
            return await Preflight<PullRequestResponse>();

        return await Success(new PullRequestResponse(pullRequest, affectedFiles));
    }

    private static bool HasFilePathPatterns(OptionalFilepath? filepath)
    {
        return filepath?.FilePathPatterns?.Any(value => !string.IsNullOrWhiteSpace(value)) == true;
    }

    private RestRequest CreateDiffstatRequest(
        PullRequestEntity pullRequest,
        string? workspaceIdentifier,
        string? repositoryIdentifier)
    {
        if (!string.IsNullOrWhiteSpace(pullRequest.Links?.Diffstat?.Href))
            return new RestRequest(pullRequest.Links.Diffstat.Href);

        var workspaceUuid = _resolver.ResolveWorkspaceUuid(workspaceIdentifier);
        var repositoryUuid = _resolver.ResolveRepositoryUuid(repositoryIdentifier);

        return new BitbucketCloudRequest(
            $"repositories/{workspaceUuid}/{repositoryUuid}/pullrequests/{pullRequest.Id}/diffstat");
    }

    public static bool PullRequestMatches(
        PullRequestEntity pullRequest,
        OptionalBranchIdentifier? branchIdentifier,
        OptionalPullRequestFilter? pullRequestFilter)
    {
        var sourceBranchName = pullRequest.Source?.Branch?.Name ?? string.Empty;
        if (!WebhookFilterHelper.BranchMatches(sourceBranchName, branchIdentifier))
            return false;

        if (pullRequestFilter is null)
            return true;

        return TextMatches(
                   pullRequest.Title,
                   pullRequestFilter.TitleContains,
                   pullRequestFilter.TitleDoesntContain)
               && TextMatches(
                   pullRequest.Description,
                   pullRequestFilter.DescriptionContains,
                   pullRequestFilter.DescriptionDoesntContain);
    }

    private static bool TextMatches(
        string? value,
        IEnumerable<string>? contains,
        IEnumerable<string>? doesntContain)
    {
        var actualValue = value ?? string.Empty;
        var containsValues = Normalize(contains);

        if (containsValues.Count > 0 &&
            !containsValues.Any(x => actualValue.Contains(x, StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        var doesntContainValues = Normalize(doesntContain);

        return doesntContainValues.Count == 0 ||
               !doesntContainValues.Any(x => actualValue.Contains(x, StringComparison.OrdinalIgnoreCase));
    }

    private static List<string> Normalize(IEnumerable<string>? values)
    {
        return values?
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value.Trim())
            .ToList() ?? [];
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
