using Apps.Bitbucket.Api.Request;
using Apps.Bitbucket.Extensions;
using Apps.Bitbucket.Models.Entities.Branch;
using Apps.Bitbucket.Models.Identifiers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Exceptions;
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
        List<string> missingInputs = [];
        
        if (string.IsNullOrEmpty(workspaceIdentifier.WorkspaceUuid))
            missingInputs.Add("Workspace UUID");
        
        if (string.IsNullOrEmpty(repositoryIdentifier.RepositoryUuid))
            missingInputs.Add("Repository UUID");

        if (missingInputs.Count != 0)
        {
            string missingInputsString = string.Join(", ", missingInputs);
            throw new PluginMisconfigurationException($"Please specify these inputs first: {missingInputsString}");
        }

        _workspaceId = workspaceIdentifier.WorkspaceUuid;
        _repositoryId = repositoryIdentifier.RepositoryUuid;
    }

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