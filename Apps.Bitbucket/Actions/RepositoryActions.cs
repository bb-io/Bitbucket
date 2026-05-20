using Apps.Bitbucket.Api.Request;
using Apps.Bitbucket.Extensions;
using Apps.Bitbucket.Models.Entities.Repository;
using Apps.Bitbucket.Models.Identifiers;
using Apps.Bitbucket.Models.Request.Repository;
using Apps.Bitbucket.Models.Response.Repository;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Bitbucket.Actions;

[ActionList("Repository")]
public class RepositoryActions(InvocationContext invocationContext) : BitbucketInvocable(invocationContext)
{
    // https://developer.atlassian.com/cloud/bitbucket/rest/api-group-repositories/#api-repositories-workspace-get
    [Action("Search repositories", Description = "Search repositories in a workspace")]
    public async Task<SearchRepositoriesResponse> SearchRepositories(
        [ActionParameter] WorkspaceIdentifier workspaceIdentifier,
        [ActionParameter] SearchRepositoriesRequest input)
    {
        var request = new BitbucketCloudRequest($"repositories/{workspaceIdentifier.WorkspaceUuid}")
            .AddBitbucketQuery(q =>
            {
                q.Contains("name", input.NameContains);
                q.EqualsExact("language", input.Language);
            });
        
        var result = await Client.Paginate<RepositoryEntity>(request);
        return new(result.Select(x => new RepositoryResponse(x)).ToList());
    }

    // https://developer.atlassian.com/cloud/bitbucket/rest/api-group-repositories/#api-repositories-workspace-repo-slug-get
    [Action("Get repository", Description = "Get information about a repository")]
    public async Task<RepositoryResponse> GetRepository(
        [ActionParameter] WorkspaceIdentifier workspaceIdentifier,
        [ActionParameter] RepositoryIdentifier repositoryIdentifier)
    {
        string endpoint = $"repositories/{workspaceIdentifier.WorkspaceUuid}/{repositoryIdentifier.RepositoryUuid}";
        var request = new BitbucketCloudRequest(endpoint);
        var result = await Client.ExecuteWithErrorHandling<RepositoryEntity>(request);
        return new(result);
    }
}