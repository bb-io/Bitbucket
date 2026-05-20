using Apps.Bitbucket.Api.Request;
using Apps.Bitbucket.Extensions;
using Apps.Bitbucket.Models.Pagination.Workspace;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Bitbucket.Handlers;

public class WorkspaceDataHandler(InvocationContext invocationContext)
    : BitbucketInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    // https://developer.atlassian.com/cloud/bitbucket/rest/api-group-workspaces/#api-user-workspaces-get
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken ct)
    {
        var request = new BitbucketCloudRequest("user/workspaces")
            .AddQueryParameterIfNotNull("sort", context.SearchString);
        var result = await Client.PaginateOnce<WorkspacePaginationEntity>(request);
        return result.Select(x => new DataSourceItem(x.WorkspaceEntity.Uuid, x.WorkspaceEntity.Slug)).ToList();
    }
}
