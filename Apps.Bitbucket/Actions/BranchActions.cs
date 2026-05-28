using System.Net;
using Apps.Bitbucket.Api.Request;
using Apps.Bitbucket.Extensions;
using Apps.Bitbucket.Helper;
using Apps.Bitbucket.Models.Entities.Branch;
using Apps.Bitbucket.Models.Identifiers.Optional;
using Apps.Bitbucket.Models.Payloads.Branch.CreateBranch;
using Apps.Bitbucket.Models.Request.Branch;
using Apps.Bitbucket.Models.Response.Branch;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using RestSharp;

namespace Apps.Bitbucket.Actions;

[ActionList("Branches")]
public class BranchActions(InvocationContext invocationContext) : BitbucketInvocable(invocationContext)
{
    private readonly IdentifierResolver _resolver = new(invocationContext.AuthenticationCredentialsProviders);

    // https://developer.atlassian.com/cloud/bitbucket/rest/api-group-refs/#api-repositories-workspace-repo-slug-refs-branches-get
    [Action("Search branches", Description = "Search branches in a specific workspace")]
    public async Task<SearchBranchesResponse> SearchBranches(
        [ActionParameter] OptionalWorkspaceIdentifier workspaceIdentifier,
        [ActionParameter] OptionalRepositoryIdentifier repositoryIdentifier,
        [ActionParameter] SearchBranchesRequest searchInput)
    {
        string workspaceUuid = _resolver.ResolveWorkspaceUuid(workspaceIdentifier.WorkspaceUuid);
        string repositoryUuid = _resolver.ResolveRepositoryUuid(repositoryIdentifier.RepositoryUuid);
        
        var request = new BitbucketCloudRequest($"repositories/{workspaceUuid}/{repositoryUuid}/refs/branches")
            .AddBitbucketQuery(q =>
            {
                q.Contains("name", searchInput.BranchNameContains);
            });
        
        var result = await Client.Paginate<BranchEntity>(request);
        return new(result.Select(x => new BranchResponse(x)).ToList());
    }

    // https://developer.atlassian.com/cloud/bitbucket/rest/api-group-refs/#api-repositories-workspace-repo-slug-refs-branches-name-get
    [Action("Branch exists", Description = "Check if branch exists by name")]
    public async Task<bool> BranchExists(
        [ActionParameter] OptionalWorkspaceIdentifier workspaceIdentifier,
        [ActionParameter] OptionalRepositoryIdentifier repositoryIdentifier,
        [ActionParameter] BranchExistsRequest existsInput)
    {
        string workspaceUuid = _resolver.ResolveWorkspaceUuid(workspaceIdentifier.WorkspaceUuid);
        string repositoryUuid = _resolver.ResolveRepositoryUuid(repositoryIdentifier.RepositoryUuid);
        string branchName = Uri.EscapeDataString(existsInput.Name);
        
        string endpoint = $"repositories/{workspaceUuid}/{repositoryUuid}/refs/branches/{branchName}";
        var request = new BitbucketCloudRequest(endpoint);
        
        var result = await Client.ExecuteWithExpectedStatuses(request, HttpStatusCode.NotFound);
        return result.IsSuccessStatusCode;
    }

    // https://developer.atlassian.com/cloud/bitbucket/rest/api-group-refs/#api-repositories-workspace-repo-slug-refs-branches-post
    [Action("Create branch", Description = "Create a new branch in a repository")]
    public async Task<BranchResponse> CreateBranch(
        [ActionParameter] OptionalWorkspaceIdentifier workspaceIdentifier,
        [ActionParameter] OptionalRepositoryIdentifier repositoryIdentifier,
        [ActionParameter] CreateBranchRequest createInput)
    {
        string workspaceUuid = _resolver.ResolveWorkspaceUuid(workspaceIdentifier.WorkspaceUuid);
        string repositoryUuid = _resolver.ResolveRepositoryUuid(repositoryIdentifier.RepositoryUuid);
        string branchName = Uri.EscapeDataString(createInput.BranchName);
        
        string sourceEndpoint = $"repositories/{workspaceUuid}/{repositoryUuid}/refs/branches/{branchName}";
        var sourceRequest = new BitbucketCloudRequest(sourceEndpoint);
        var sourceBranch = await Client.ExecuteWithErrorHandling<BranchEntity>(sourceRequest);
        
        string endpoint = $"repositories/{workspaceUuid}/{repositoryUuid}/refs/branches";
        var payload = new CreateBranchPayload(createInput.BranchName, sourceBranch.Target.Hash);
        var request = new BitbucketCloudRequest(endpoint, Method.Post).WithJsonBody(payload);

        var result = await Client.ExecuteWithErrorHandling<BranchEntity>(request);
        return new(result);
    }
}