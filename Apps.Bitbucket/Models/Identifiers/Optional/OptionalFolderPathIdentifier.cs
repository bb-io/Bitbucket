using Apps.Bitbucket.Handlers.FileFolder;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;

namespace Apps.Bitbucket.Models.Identifiers.Optional;

public class OptionalFolderPathIdentifier
{
    [Display("Folder path"), FileDataSource(typeof(FolderPickerDataHandler))]
    public string? FolderPath { get; set; }
}