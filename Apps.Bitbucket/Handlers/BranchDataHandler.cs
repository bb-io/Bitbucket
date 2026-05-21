using Apps.Bitbucket.Api.Request;
using Apps.Bitbucket.Extensions;
using Apps.Bitbucket.Helper;
using Apps.Bitbucket.Models.Entities.Branch;
using Apps.Bitbucket.Models.Identifiers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Bitbucket.Handlers;

public class BranchDataHandler : BitbucketInvocable, IAsyncDataSourceItemHandler
{
    private readonly string _workspaceId;
    private readonly string _repositoryId;
    
    public BranchDataHandler(
        InvocationContext context, 
        [ActionParameter] WorkspaceIdentifier workspaceIdentifier,
        [ActionParameter] RepositoryIdentifier repositoryIdentifier) 
        : base(context)
    {
        InputValidator.ThrowIfMissing(
            (workspaceIdentifier.WorkspaceUuid, "Workspace UUID"),
            (repositoryIdentifier.RepositoryUuid, "Repository UUID"));

        _workspaceId = workspaceIdentifier.WorkspaceUuid;
        _repositoryId = repositoryIdentifier.RepositoryUuid;
    }

    // https://developer.atlassian.com/cloud/bitbucket/rest/api-group-refs/#api-repositories-workspace-repo-slug-refs-branches-get
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken ct)
    {
        var request = new BitbucketCloudRequest($"repositories/{_workspaceId}/{_repositoryId}/refs/branches")
            .AddBitbucketQuery(q =>
            {
                q.Contains("name", context.SearchString);
            });
        
        var result = await Client.PaginateOnce<BranchEntity>(request);
        return result.Select(x => new DataSourceItem(x.Name, x.Name)).ToList();
    }
}