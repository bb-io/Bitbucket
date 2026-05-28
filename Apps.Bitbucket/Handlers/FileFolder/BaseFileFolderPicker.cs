using Apps.Bitbucket.Api.Request;
using Apps.Bitbucket.Extensions;
using Apps.Bitbucket.Helper;
using Apps.Bitbucket.Models.Entities.File;
using Apps.Bitbucket.Models.Identifiers.Optional;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;
using File = Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems.File;

namespace Apps.Bitbucket.Handlers.FileFolder;

public class BaseFileFolderPicker : BitbucketInvocable
{
    private readonly string _workspaceId;
    private readonly string _repositoryId;
    private readonly string _branchName;

    protected BaseFileFolderPicker(InvocationContext context,
        OptionalWorkspaceIdentifier workspaceIdentifier,
        OptionalRepositoryIdentifier repositoryIdentifier,
        OptionalBranchIdentifier branchIdentifier) 
        : base(context)
    {
        var resolver = new IdentifierResolver(context.AuthenticationCredentialsProviders);
        
        _workspaceId = resolver.ResolveWorkspaceUuid(workspaceIdentifier.WorkspaceUuid);
        _repositoryId = resolver.ResolveRepositoryUuid(repositoryIdentifier.RepositoryUuid);
        _branchName = branchIdentifier.GetBranchName();
    }

    protected Task<IEnumerable<FolderPathItem>> GetFolderPathAsync(string? fileDataItemId)
    {
        if (string.IsNullOrWhiteSpace(fileDataItemId))
            return Task.FromResult(Enumerable.Empty<FolderPathItem>());

        var pathSegments = fileDataItemId.Split('/', StringSplitOptions.RemoveEmptyEntries);
    
        var pathItems = new List<FolderPathItem> { new() { Id = string.Empty, DisplayName = "Root" } };
        var cumulativePath = string.Empty;
        
        foreach (var segment in pathSegments)
        {
            cumulativePath = string.IsNullOrEmpty(cumulativePath) ? segment : $"{cumulativePath}/{segment}";
            pathItems.Add(new FolderPathItem { Id = cumulativePath, DisplayName = segment });
        }

        if (pathItems.Count > 1)
            pathItems.RemoveAt(pathItems.Count - 1);
        
        return Task.FromResult<IEnumerable<FolderPathItem>>(pathItems);
    }

    protected async Task<IEnumerable<FileDataItem>> GetFolderContentAsync(
        string? folderId, 
        bool filesAreSelectable, 
        bool foldersAreSelectable)
    {
        string filePath = folderId ?? string.Empty;
        
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
                IsSelectable = foldersAreSelectable
            });
        
        var files = listResult
            .Where(x => x.Type == "commit_file")
            .Select(x => new File
            {
                Id = x.Path, 
                DisplayName = Path.GetFileName(x.Path),
                IsSelectable = filesAreSelectable
            });
        
        items.AddRange(folders);
        items.AddRange(files);
        return items;
    }
}