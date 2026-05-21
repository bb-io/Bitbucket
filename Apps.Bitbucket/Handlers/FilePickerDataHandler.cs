using Apps.Bitbucket.Api.Request;
using Apps.Bitbucket.Extensions;
using Apps.Bitbucket.Helper;
using Apps.Bitbucket.Models.Entities.File;
using Apps.Bitbucket.Models.Identifiers;
using Apps.Bitbucket.Models.Identifiers.Optional;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;
using File = Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems.File;

namespace Apps.Bitbucket.Handlers;

public class FilePickerDataHandler : BitbucketInvocable, IAsyncFileDataSourceItemHandler
{
    private readonly string _workspaceId;
    private readonly string _repositoryId;
    private readonly string _branchName;
    
    public FilePickerDataHandler(
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
        _branchName = branchIdentifier.GetBranchName();
    }
    
    public async Task<IEnumerable<FolderPathItem>> GetFolderPathAsync(FolderPathDataSourceContext context, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(context.FileDataItemId))
            return [];

        var pathSegments = context.FileDataItemId.Split('/', StringSplitOptions.RemoveEmptyEntries);
    
        var pathItems = new List<FolderPathItem> { new() { Id = string.Empty, DisplayName = "Root" } };
        var cumulativePath = string.Empty;
        
        foreach (var segment in pathSegments)
        {
            cumulativePath = string.IsNullOrEmpty(cumulativePath) ? segment : $"{cumulativePath}/{segment}";
            pathItems.Add(new FolderPathItem { Id = cumulativePath, DisplayName = segment });
        }

        pathItems.RemoveAt(-1);
        return pathItems;
    }

    public async Task<IEnumerable<FileDataItem>> GetFolderContentAsync(FolderContentDataSourceContext context, CancellationToken ct)
    {
        string filePath = context.FolderId ?? string.Empty;
        
        var request = new BitbucketCloudRequest($"repositories/{_workspaceId}/{_repositoryId}/src/{_branchName}/{filePath}");
        var result = await Client.Paginate<FileEntity>(request);
        
        var listResult = result.ToList();
        var items = new List<FileDataItem>();
        
        var folders = listResult
            .Where(x => x.Type == "commit_directory")
            .Select(x => new Folder
            {
                Id = x.Path, 
                DisplayName = Path.GetFileName(x.Path),
                IsSelectable = false
            });
        
        var files = listResult
            .Where(x => x.Type == "commit_file")
            .Select(x => new File
            {
                Id = x.Path, 
                DisplayName = Path.GetFileName(x.Path),
                IsSelectable = true
            });
        
        items.AddRange(folders);
        items.AddRange(files);
        return items;
    }
}