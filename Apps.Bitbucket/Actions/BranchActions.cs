using System.Net;
using Apps.Bitbucket.Api.Request;
using Apps.Bitbucket.Extensions;
using Apps.Bitbucket.Models.Entities.Branch;
using Apps.Bitbucket.Models.Identifiers;
using Apps.Bitbucket.Models.Request.Branch;
using Apps.Bitbucket.Models.Response.Branch;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Bitbucket.Actions;

[ActionList("Branches")]
public class BranchActions(InvocationContext invocationContext) : BitbucketInvocable(invocationContext)
{
    // https://developer.atlassian.com/cloud/bitbucket/rest/api-group-refs/#api-repositories-workspace-repo-slug-refs-branches-get
    [Action("Search branches", Description = "Search branches in a specific workspace")]
    public async Task<SearchBranchesResponse> SearchBranches(
        [ActionParameter] WorkspaceIdentifier workspaceIdentifier,
        [ActionParameter] RepositoryIdentifier repositoryIdentifier,
        [ActionParameter] SearchBranchesRequest searchInput)
    {
        string endpoint =
            $"repositories/{workspaceIdentifier.WorkspaceUuid}/{repositoryIdentifier.RepositoryUuid}/refs/branches";
        var request = new BitbucketCloudRequest(endpoint)
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
        [ActionParameter] WorkspaceIdentifier workspaceIdentifier,
        [ActionParameter] RepositoryIdentifier repositoryIdentifier,
        [ActionParameter] BranchExistsRequest existsInput)
    {
        string endpoint = $"repositories/{workspaceIdentifier.WorkspaceUuid}/{repositoryIdentifier.RepositoryUuid}" +
                          $"/refs/branches/{existsInput.Name}";
        var request = new BitbucketCloudRequest(endpoint);
        
        var result = await Client.ExecuteWithExpectedStatuses(request, HttpStatusCode.NotFound);
        return result.IsSuccessStatusCode;
    }

    // https://developer.atlassian.com/cloud/bitbucket/rest/api-group-refs/#api-repositories-workspace-repo-slug-refs-branches-post
    [Action("Create branch", Description = "Create a new branch in a repository")]
    public async Task<BranchResponse> CreateBranch(
        [ActionParameter] WorkspaceIdentifier workspaceIdentifier,
        [ActionParameter] RepositoryIdentifier repositoryIdentifier,
        [ActionParameter] CreateBranchRequest createInput)
    {
        string sourceEndpoint = 
            $"repositories/{workspaceIdentifier.WorkspaceUuid}/{repositoryIdentifier.RepositoryUuid}" +
            $"/refs/branches/{createInput.SourceBranchName}";
        var sourceRequest = new BitbucketCloudRequest(sourceEndpoint);
        var sourceBranch = await Client.ExecuteWithErrorHandling<BranchEntity>(sourceRequest);
        
        string endpoint = 
            $"repositories/{workspaceIdentifier.WorkspaceUuid}/{repositoryIdentifier.RepositoryUuid}/refs/branches";
        var request = new BitbucketCloudRequest(endpoint, Method.Post);

        var body = new
        {
            name = createInput.BranchName,
            target = new { hash = sourceBranch.Target.Hash }
        };
        request.AddJsonBody(body);

        var result = await Client.ExecuteWithErrorHandling<BranchEntity>(request);
        return new(result);
    }
}