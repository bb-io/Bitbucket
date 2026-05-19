using Apps.Bitbucket.Models.Identifiers.Optional;
using Apps.Bitbucket.Models.Pagination.User;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Bitbucket.Handlers;

public class UserDataHandler : BitbucketInvocable, IAsyncDataSourceItemHandler
{
    private readonly string _workspaceUuid;
    
    public UserDataHandler(InvocationContext invocationContext, OptionalWorkspaceIdentifier workspaceIdentifier) 
        : base(invocationContext)
    {
        if (string.IsNullOrWhiteSpace(workspaceIdentifier.WorkspaceUuid))
            throw new PluginMisconfigurationException("Please specify the workspace UUID in order to search users");

        _workspaceUuid = workspaceIdentifier.WorkspaceUuid;
    }

    // https://developer.atlassian.com/cloud/bitbucket/rest/api-group-workspaces/#api-workspaces-workspace-members-get
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken ct)
    {
        var request = new RestRequest($"workspaces/{_workspaceUuid}/members");
        
        if (!string.IsNullOrWhiteSpace(context.SearchString))
            request.AddQueryParameter("q", $"user.display_name~\"{context.SearchString}\"");
        
        var result = await Client.PaginateOnce<UserPaginationEntity>(request);
        return result.Select(x => new DataSourceItem(x.UserEntity.Uuid, x.UserEntity.DisplayName)).ToList();
    }
}