using Apps.Bitbucket.Api.Request;
using Apps.Bitbucket.Constants;
using Apps.Bitbucket.Extensions;
using Apps.Bitbucket.Models.Pagination.Workspace;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Bitbucket.Handlers;

public class WorkspaceDataHandler : BitbucketInvocable, IAsyncDataSourceItemHandler
{
    public WorkspaceDataHandler(InvocationContext invocationContext) : base(invocationContext)
    {
        if (Creds.HasConnectionType(ConnectionTypes.RepoAccessToken))
            throw new PluginMisconfigurationException(
                "'Repository access token' connection type is scoped to a single workspace, " +
                "which is already set in your connection");
    }

    // https://developer.atlassian.com/cloud/bitbucket/rest/api-group-workspaces/#api-user-workspaces-get
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken ct)
    {
        var request = new BitbucketCloudRequest("user/workspaces")
            .AddBitbucketQuery(q =>
            {
                q.Contains("slug", context.SearchString);
            });
        var result = await Client.PaginateOnce<WorkspacePaginationEntity>(request);
        return result.Select(x => new DataSourceItem(x.WorkspaceEntity.Uuid, x.WorkspaceEntity.Slug)).ToList();
    }
}
