using Apps.Bitbucket.Models.Identifiers;
using Apps.Bitbucket.Models.Identifiers.Optional;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;

namespace Apps.Bitbucket.Handlers.FileFolder;

public class FilePickerDataHandler(
    InvocationContext context,
    [ActionParameter] WorkspaceIdentifier workspaceIdentifier,
    [ActionParameter] RepositoryIdentifier repositoryIdentifier,
    [ActionParameter] OptionalBranchIdentifier branchIdentifier)
    : BaseFileFolderPicker(context, workspaceIdentifier, repositoryIdentifier, branchIdentifier), 
        IAsyncFileDataSourceItemHandler
{
    public Task<IEnumerable<FolderPathItem>> GetFolderPathAsync(FolderPathDataSourceContext context, CancellationToken ct)
    {
        return base.GetFolderPathAsync(context.FileDataItemId);
    }

    public async Task<IEnumerable<FileDataItem>> GetFolderContentAsync(FolderContentDataSourceContext context, CancellationToken ct)
    {
        return await base.GetFolderContentAsync(context.FolderId, filesAreSelectable: true, foldersAreSelectable: false);
    }
}