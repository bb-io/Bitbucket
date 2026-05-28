using Apps.Bitbucket.Api.Request;
using Apps.Bitbucket.Helper;
using Apps.Bitbucket.Models.Entities.PullRequest;
using Apps.Bitbucket.Models.Identifiers;
using Apps.Bitbucket.Models.Identifiers.Optional;
using Apps.Bitbucket.Models.Payloads.PullRequest.CreatePullRequest;
using Apps.Bitbucket.Models.Payloads.PullRequest.MergePullRequest;
using Apps.Bitbucket.Models.Request.PullRequest;
using Apps.Bitbucket.Models.Response.PullRequest;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using RestSharp;

namespace Apps.Bitbucket.Actions;

[ActionList("Pull requests")]
public class PullRequestActions(InvocationContext context) : BitbucketInvocable(context)
{
    private readonly IdentifierResolver _resolver = new(context.AuthenticationCredentialsProviders);
    
    // https://developer.atlassian.com/cloud/bitbucket/rest/api-group-pullrequests/#api-repositories-workspace-repo-slug-pullrequests-post
    [Action("Create pull request", Description = "Create a new pull request")]
    public async Task<PullRequestResponse> CreatePullRequest(
        [ActionParameter] OptionalWorkspaceIdentifier workspaceIdentifier,
        [ActionParameter] OptionalRepositoryIdentifier repositoryIdentifier,
        [ActionParameter] CreatePullRequestRequest createInput)
    {
        string workspaceUuid = _resolver.ResolveWorkspaceUuid(workspaceIdentifier.WorkspaceUuid);
        string repositoryUuid = _resolver.ResolveRepositoryUuid(repositoryIdentifier.RepositoryUuid);
        
        var payload = new CreatePullRequestPayload(createInput);
        string endpoint = $"repositories/{workspaceUuid}/{repositoryUuid}/pullrequests";
        var request = new BitbucketCloudRequest(endpoint, Method.Post).WithJsonBody(payload);

        var result = await Client.ExecuteWithErrorHandling<PullRequestEntity>(request);
        return new(result);
    }
    
    // https://developer.atlassian.com/cloud/bitbucket/rest/api-group-pullrequests/#api-repositories-workspace-repo-slug-pullrequests-pull-request-id-merge-post
    [Action("Merge pull request", Description = "Merge a pull request")]
    public async Task<PullRequestResponse> MergePullRequest(
        [ActionParameter] OptionalWorkspaceIdentifier workspaceIdentifier,
        [ActionParameter] OptionalRepositoryIdentifier repositoryIdentifier,
        [ActionParameter] PullRequestIdentifier pullRequestIdentifier,
        [ActionParameter] MergePullRequestRequest mergeInput)
    {
        string workspaceUuid = _resolver.ResolveWorkspaceUuid(workspaceIdentifier.WorkspaceUuid);
        string repositoryUuid = _resolver.ResolveRepositoryUuid(repositoryIdentifier.RepositoryUuid);
        
        var payload = new MergePullRequestPayload(mergeInput);
        string endpoint = $"repositories/{workspaceUuid}/{repositoryUuid}/" +
                          $"pullrequests/{pullRequestIdentifier.PullRequestId}/merge";
        var request = new BitbucketCloudRequest(endpoint, Method.Post).WithJsonBody(payload);
        
        var result = await Client.ExecuteWithErrorHandling<PullRequestEntity>(request);
        return new(result);
    }
}