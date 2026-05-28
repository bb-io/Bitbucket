using Apps.Bitbucket.Api.Request;
using Apps.Bitbucket.Constants;
using Apps.Bitbucket.Extensions;
using Apps.Bitbucket.Helper;
using Apps.Bitbucket.Models.Entities.Repository;
using Apps.Bitbucket.Models.Identifiers.Optional;
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
        [ActionParameter] OptionalWorkspaceIdentifier workspaceIdentifier) 
        : base(invocationContext)
    {
        if (Creds.HasConnectionType(ConnectionTypes.RepoAccessToken))
            throw new PluginMisconfigurationException(
                "'Repository access token' connection type is scoped to a single repository, " +
                "which is already set in your connection");

        var resolver = new IdentifierResolver(invocationContext.AuthenticationCredentialsProviders);
        _workspaceId = resolver.ResolveWorkspaceUuid(workspaceIdentifier.WorkspaceUuid);
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