using Apps.Bitbucket.Api.Request;
using Apps.Bitbucket.Extensions;
using Apps.Bitbucket.Helper;
using Apps.Bitbucket.Models.Entities.File;
using Apps.Bitbucket.Models.Identifiers;
using Apps.Bitbucket.Models.Identifiers.Optional;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Bitbucket.Handlers;

public class FileDataHandler : BitbucketInvocable, IAsyncDataSourceItemHandler
{
    private readonly string _workspaceId;
    private readonly string _repositoryId;
    private readonly string _branchName;
    
    public FileDataHandler(
        InvocationContext context,
        [ActionParameter] WorkspaceIdentifier workspaceIdentifier,
        [ActionParameter] RepositoryIdentifier repositoryIdentifier,
        [ActionParameter] OptionalBranchIdentifier branchIdentifier) 
        : base(context)
    {
        InputValidator.ThrowIfMissing(
            () => workspaceIdentifier.WorkspaceUuid,
            () => repositoryIdentifier.RepositoryUuid);

        _repositoryId = repositoryIdentifier.RepositoryUuid;
        _workspaceId = workspaceIdentifier.WorkspaceUuid;
        _branchName = string.IsNullOrEmpty(branchIdentifier.BranchName) ? "HEAD" : branchIdentifier.BranchName;
    }

    // https://developer.atlassian.com/cloud/bitbucket/rest/api-group-source/#api-repositories-workspace-repo-slug-src-commit-path-get
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken ct)
    {
        var request = new BitbucketCloudRequest($"repositories/{_workspaceId}/{_repositoryId}/src/{_branchName}/");
        var result = await Client.PaginateOnce<FileEntity>(request);
        return result
            .Where(x => x.Type == "commit_file")
            .Where(x => x.Path.MatchesSearch(context.SearchString))
            .Select(x => new DataSourceItem(x.Path, x.Path))
            .ToList();
    }
}