using Apps.Bitbucket.Api.Request;
using Apps.Bitbucket.Extensions;
using Apps.Bitbucket.Models.Identifiers.Optional;
using Apps.Bitbucket.Models.Pagination.User;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Bitbucket.Handlers;

public class UserDataHandler : BitbucketInvocable, IAsyncDataSourceItemHandler
{
    private readonly string _workspaceUuid;
    
    public UserDataHandler(
        InvocationContext invocationContext, 
        [ActionParameter] OptionalWorkspaceIdentifier workspaceIdentifier) 
        : base(invocationContext)
    {
        if (string.IsNullOrWhiteSpace(workspaceIdentifier.WorkspaceUuid))
            throw new PluginMisconfigurationException("Please specify the workspace UUID in order to search users");

        _workspaceUuid = workspaceIdentifier.WorkspaceUuid;
    }

    // https://developer.atlassian.com/cloud/bitbucket/rest/api-group-workspaces/#api-workspaces-workspace-members-get
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken ct)
    {
        var request = new BitbucketCloudRequest($"workspaces/{_workspaceUuid}/members");
        var result = await Client.PaginateOnce<UserPaginationEntity>(request);
        return result
            .Where(x => x.UserEntity.DisplayName.MatchesSearch(context.SearchString))
            .Select(x => new DataSourceItem(x.UserEntity.Uuid, x.UserEntity.DisplayName)).ToList();
    }
}