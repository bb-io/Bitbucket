using Apps.Bitbucket.Handlers;
using Apps.Bitbucket.Handlers.FileFolder;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;

namespace Apps.Bitbucket.Models.Identifiers;

public class FilePathIdentifier
{
    [Display("File path"), FileDataSource(typeof(FilePickerDataHandler))]
    public string FilePath { get; set; } = string.Empty;
}