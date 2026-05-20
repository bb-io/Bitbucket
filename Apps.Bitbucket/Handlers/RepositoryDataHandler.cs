using Apps.Bitbucket.Api.Request;
using Apps.Bitbucket.Extensions;
using Apps.Bitbucket.Models.Entities.Repository;
using Apps.Bitbucket.Models.Identifiers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Bitbucket.Handlers;

public class RepositoryDataHandler : BitbucketInvocable, IAsyncDataSourceItemHandler
{
    private readonly string _workspaceId;
    
    public RepositoryDataHandler(
        InvocationContext invocationContext,
        [ActionParameter] WorkspaceIdentifier workspaceIdentifier) 
        : base(invocationContext)
    {
        if (string.IsNullOrEmpty(workspaceIdentifier.WorkspaceUuid))
            throw new PluginMisconfigurationException("Please specify the workspace UUID first");

        _workspaceId = workspaceIdentifier.WorkspaceUuid;
    }

    // https://developer.atlassian.com/cloud/bitbucket/rest/api-group-repositories/#api-repositories-workspace-get
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken ct)
    {
        var request = new BitbucketCloudRequest($"repositories/{_workspaceId}")
            .AddBitbucketQuery(q =>
            {
                q.Contains("name", context.SearchString);
            });
        
        var result = await Client.PaginateOnce<RepositoryEntity>(request);
        return result.Select(x => new DataSourceItem(x.Uuid, x.Name)).ToList();
    }
}