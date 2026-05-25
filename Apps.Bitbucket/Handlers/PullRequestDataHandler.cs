using Apps.Bitbucket.Api.Request;
using Apps.Bitbucket.Extensions;
using Apps.Bitbucket.Helper;
using Apps.Bitbucket.Models.Entities.PullRequest;
using Apps.Bitbucket.Models.Identifiers.Optional;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Bitbucket.Handlers;

public class PullRequestDataHandler : BitbucketInvocable, IAsyncDataSourceItemHandler
{
    private readonly string _workspaceId;
    private readonly string _repositoryId;
    
    public PullRequestDataHandler(
        InvocationContext context,
        [ActionParameter] OptionalWorkspaceIdentifier workspaceIdentifier,
        [ActionParameter] OptionalRepositoryIdentifier repositoryIdentifier) 
        : base(context)
    {
        var resolver = new IdentifierResolver(context.AuthenticationCredentialsProviders);
        
        _workspaceId = resolver.ResolveWorkspaceUuid(workspaceIdentifier.WorkspaceUuid);
        _repositoryId = resolver.ResolveRepositoryUuid(repositoryIdentifier.RepositoryUuid);
    }
    
    // https://developer.atlassian.com/cloud/bitbucket/rest/api-group-pullrequests/#api-repositories-workspace-repo-slug-pullrequests-get
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken ct)
    {
        var request = new BitbucketCloudRequest($"repositories/{_workspaceId}/{_repositoryId}/pullrequests")
            .AddBitbucketQuery(q =>
            {
                q.Contains("title", context.SearchString);
                q.In("state", ["OPEN", "MERGED", "DECLINED", "SUPERSEDED"]);
            });
        
        var result = await Client.PaginateOnce<PullRequestEntity>(request);
        return result.Select(x => new DataSourceItem(x.Id, $"{x.Title} (State: {x.State.ToLower()})")).ToList();
    }
}